using ColorMixer;
using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace ViewModels
{
    public class Mixer
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMainWindowViewModel window;
        private readonly INodeFactory nodeFactory;
        private readonly IConnectionFactory connectionFactory;
        private readonly IObservable<KeyEventArgs> keyDown;

        public Mixer()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            window = Substitute.For<IMainWindowViewModel>();
            nodeFactory = Substitute.For<INodeFactory>();
            connectionFactory = Substitute.For<IConnectionFactory>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMainWindowViewModel>()
                  .ToConstant(window);

            kernel.Bind<INodeFactory>()
                  .ToConstant(nodeFactory);

            kernel.Bind<IConnectionFactory>()
                  .ToConstant(connectionFactory);

            kernel.Bind<IMixerViewModel>()
                  .To<MixerViewModel>()
                  .InSingletonScope(); // system under test

            keyDown = new Subject<KeyEventArgs>();
            window.KeyDown.Returns(keyDown);

            nodeFactory.Create<IColorNodeViewModel>()
                       .Returns(ci => Substitute.For<IColorNodeViewModel>());

            nodeFactory.Create<IOperationNodeViewModel>()
                       .Returns(ci => Substitute.For<IOperationNodeViewModel>());

            nodeFactory.Create<IResultNodeViewModel>()
                       .Returns(ci => Substitute.For<IResultNodeViewModel>());

            connectionFactory.Create()
                             .Returns(ci => kernel.Get<IConnectionViewModel>());
        }

        [Fact]
        public void SetsActivator()
            => kernel.Get<IMixerViewModel>()
                     .Activator.Should().NotBeNull();

        [Fact]
        public void SetsUrlPathSegment()
            => kernel.Get<IMixerViewModel>()
                     .UrlPathSegment.Should().Be(MixerViewModel.DefaultUrlPathSegment);

        [Fact]
        public void SetsHostScreen()
            => kernel.Get<IMixerViewModel>()
                     .HostScreen.Should().Be(window);

        [Fact]
        public void SetsNodes()
            => kernel.Get<IMixerViewModel>()
                     .Nodes.Should().NotBeNull();

        [Fact]
        public void SetsConnections()
            => kernel.Get<IMixerViewModel>()
                     .Connections.Should().NotBeNull();

        [Fact]
        public void SetsMainWindowKeyDown()
        {
            // Arrange

            var mixer = kernel.Get<IMixerViewModel>();

            // Act

            mixer.Activator
                 .Activate();

            // Assert

            mixer.MainWindowKeyDown.Should().Be(keyDown);
        }

        [Theory]
        [AutoData]
        public async void AddColorNodeCommand_AddsColorNode(Point output)
            => await kernel.Get<IMixerViewModel>().ShouldAddNode<IColorNodeViewModel>(
                output,
                interactions,
                nodeFactory,
                async node => await node.AddColorNodeCommand
                                        .Execute());

        [Fact]
        public async void AddColorNodeCommand_HandlesCancellation()
            => await kernel.Get<IMixerViewModel>().ShouldNotAddNode<IColorNodeViewModel>(
                interactions,
                nodeFactory,
                async node => await node.AddColorNodeCommand
                                        .Execute());

        [Theory]
        [AutoData]
        public async void AddOperationNodeCommand_AddsOperationNode(Point output)
            => await kernel.Get<IMixerViewModel>().ShouldAddNode<IOperationNodeViewModel>(
                output,
                interactions,
                nodeFactory,
                async node => await node.AddOperationNodeCommand
                                        .Execute());

        [Fact]
        public async void AddOperationNodeCommand_HandlesCancellation()
            => await kernel.Get<IMixerViewModel>().ShouldNotAddNode<IOperationNodeViewModel>(
                interactions,
                nodeFactory,
                async node => await node.AddOperationNodeCommand
                                        .Execute());

        [Theory]
        [AutoData]
        public async void AddResultNodeCommand_AddsResultNode(Point output)
            => await kernel.Get<IMixerViewModel>().ShouldAddNode<IResultNodeViewModel>(
                output,
                interactions,
                nodeFactory,
                async node => await node.AddResultNodeCommand
                                        .Execute());

        [Fact]
        public async void AddResultNodeCommand_HandlesCancellation()
            => await kernel.Get<IMixerViewModel>().ShouldNotAddNode<IResultNodeViewModel>(
                interactions,
                nodeFactory,
                async node => await node.AddResultNodeCommand
                                        .Execute());

        [Theory]
        [InlineAutoNSubstituteData(nameof(IMixerViewModel.ConnectingConnector))]
        [InlineAutoNSubstituteData(nameof(IMixerViewModel.ConnectedConnector))]
        public void SetsConnectorProperties(string property,
                                            IConnector initial,
                                            IConnector expected)
            => kernel.Get<IMixerViewModel>()
                     .ShouldSetProperty(property, initial, expected);
    }
}