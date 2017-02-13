using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Media;
using Xunit;

namespace ViewModels
{
    public class ColorNode
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;
        private readonly IOutConnectorViewModel output;

        public ColorNode()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel, ReactiveObject>();
            output = Substitute.For<IOutConnectorViewModel>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<IOutConnectorViewModel>()
                  .ToConstant(output);

            kernel.Bind<IColorNodeViewModel>()
                  .To<ColorNodeViewModel>(); // system under test
        }

        [Fact]
        public void SetsOutput()
            => kernel.Get<IColorNodeViewModel>()
                     .Output.Should().Be(output);

        [Fact]
        public void SetsOutputNode()
        {
            // Arrange

            var node = kernel.Get<IColorNodeViewModel>();

            // Assert

            output.Received().Node = node;
        }

        [Fact]
        public async void EditNodeCommand_InvokesDeleteNodeInteraction()
        {
            // Arrange

            var input = default(Color);
            var output = Colors.Pink;

            interactions.GetNodeColor
                        .RegisterHandler(i =>
                        {
                            input = i.Input;
                            i.SetOutput(output);
                        });

            var node = kernel.Get<IColorNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            await node.EditNodeCommand
                      .Execute();

            // Assert

            input.Should().Be(Node.DefaultColor);

            node.Color.Should().Be(output);
        }

        [Theory]
        [InlineData(true, false, default(IConnector))]
        [InlineData(false, true, default(IConnector))]
        [InlineAutoNSubstituteData(false, false)]
        public async void EditNodeCommand_CanExecute(bool expected,
                                                     bool isNodeBeingAdded,
                                                     IConnector connectingConnector)
        {
            // Arrange

            var node = kernel.Get<IColorNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            mixer.IsNodeBeingAdded
                 .Returns(isNodeBeingAdded);

            mixer.RaisePropertyChanged(nameof(mixer.IsNodeBeingAdded));

            mixer.ConnectingConnector
                 .Returns(connectingConnector);

            mixer.RaisePropertyChanged(nameof(mixer.ConnectingConnector));

            var actual = await node.EditNodeCommand
                                   .CanExecute
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }
    }
}