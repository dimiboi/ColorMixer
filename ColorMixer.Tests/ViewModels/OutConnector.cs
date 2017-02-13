using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using ReactiveUI;
using System.Reactive.Linq;
using Xunit;

namespace ViewModels
{
    public class OutConnector
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;
        private readonly INode node;

        public OutConnector()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel, ReactiveObject>();
            node = Substitute.For<INode>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<IOutConnectorViewModel>()
                  .To<OutConnectorViewModel>()
                  .WithPropertyValue(nameof(IOutConnectorViewModel.Node),
                                     node); // system under test

            mixer.ConnectingConnector = Arg.Do<IConnector>(
                _ => mixer.RaisePropertyChanged(nameof(mixer.ConnectingConnector)));
        }

        [Fact]
        public void OutDirection()
            => kernel.Get<IOutConnectorViewModel>()
                     .Direction.Should().Be(ConnectorDirection.Output);

        [Theory]
        [InlineAutoNSubstituteData(false, ConnectorDirection.Output)]
        [InlineAutoNSubstituteData(true, ConnectorDirection.Input)]
        public async void Disabled_WhenNotConnectableTo(bool expected,
                                                        ConnectorDirection connectingDirection,
                                                        INode connectingNode)
        {
            // Arrange

            var connectingConnector = Substitute.For<IConnector>();

            connectingConnector.Direction
                               .Returns(connectingDirection);

            connectingConnector.Node
                               .Returns(connectingNode);

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            mixer.ConnectingConnector = connectingConnector;

            var actual = await connector.WhenAnyValue(vm => vm.IsEnabled)
                                        .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineAutoData(ConnectorDirection.Output)]
        [InlineAutoData(ConnectorDirection.Input)]
        public async void Disabled_WhenConnectingSource(ConnectorDirection connectingDirection)
        {
            // Arrange

            var connectingConnector = Substitute.For<IConnector>();

            connectingConnector.Direction
                               .Returns(connectingDirection);

            connectingConnector.Node
                               .Returns(node);

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            mixer.ConnectingConnector = connectingConnector;

            var isEnabled = await connector.WhenAnyValue(vm => vm.IsEnabled)
                                           .FirstAsync();
            // Assert

            isEnabled.Should().BeFalse();
        }

        [Fact]
        public async void Enabled_WhenNotConnecting()
        {
            // Arrange

            var connectingConnector = Substitute.For<IConnector>();

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            mixer.ConnectedConnector = connectingConnector;
            mixer.ConnectingConnector = null;

            var isEnabled = await connector.WhenAnyValue(vm => vm.IsEnabled)
                                           .FirstAsync();
            // Assert

            isEnabled.Should().BeTrue();
        }

        [Theory]
        [InlineAutoNSubstituteData(true)]
        [InlineData(false, default(IInConnectorViewModel))]
        public async void SetsIsConnected(bool expected, IInConnectorViewModel connectedTo)
        {
            // Arrange

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            connector.ConnectedTo = connectedTo;

            var isConnected = await connector.WhenAnyValue(vm => vm.IsConnected)
                                             .FirstAsync();
            // Assert

            isConnected.Should().Be(expected);
        }

        [Fact]
        public async void ConnectorCommand_InvokesGetOutConnectorInteraction()
        {
            // Arrange

            var input = default(IOutConnectorViewModel);
            var output = Substitute.For<IInConnectorViewModel>();

            interactions.GetInConnector
                        .RegisterHandler(i =>
                        {
                            input = i.Input;
                            i.SetOutput(output);
                        });

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            await connector.ConnectorCommand
                           .Execute();

            // Assert

            input.Should().Be(connector);

            connector.ConnectedTo.Should().Be(output);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async void ConnectorCommand_CanExecute(bool expected, bool isNodeBeingAdded)
        {
            // Arrange

            var connector = kernel.Get<IOutConnectorViewModel>();

            // Act

            connector.Activator
                     .Activate();

            mixer.IsNodeBeingAdded
                 .Returns(isNodeBeingAdded);

            mixer.RaisePropertyChanged(nameof(mixer.IsNodeBeingAdded));

            var actual = await connector.ConnectorCommand
                                        .CanExecute
                                        .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }
    }
}