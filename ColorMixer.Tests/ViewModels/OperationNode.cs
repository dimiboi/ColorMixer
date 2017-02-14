using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using ReactiveUI;
using System.Reactive.Linq;
using Xunit;

namespace ViewModels
{
    public class OperationNode
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;
        private readonly IInConnectorViewModel inputA;
        private readonly IInConnectorViewModel inputB;
        private readonly IOutConnectorViewModel output;

        public OperationNode()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel, ReactiveObject>();
            inputA = Substitute.For<IInConnectorViewModel, ReactiveObject>();
            inputB = Substitute.For<IInConnectorViewModel, ReactiveObject>();
            output = Substitute.For<IOutConnectorViewModel, ReactiveObject>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<IOutConnectorViewModel>()
                  .ToConstant(output);

            kernel.Bind<IOperationNodeViewModel>()
                  .To<OperationNodeViewModel>()
                  .InSingletonScope()
                  .WithConstructorArgument("inputA", inputA)
                  .WithConstructorArgument("inputB", inputB); // system under test
        }

        [Fact]
        public void SetsInputA()
            => kernel.Get<IOperationNodeViewModel>()
                     .InputA.Should().Be(inputA);

        [Fact]
        public void SetsInputB()
            => kernel.Get<IOperationNodeViewModel>()
                     .InputB.Should().Be(inputB);

        [Fact]
        public void SetsOutput()
            => kernel.Get<IOperationNodeViewModel>()
                     .Output.Should().Be(output);

        [Fact]
        public void SetsInputANode()
            => kernel.Get<IOperationNodeViewModel>()
                     .InputA.Received().Node = kernel.Get<IOperationNodeViewModel>();

        [Fact]
        public void SetsInputBNode()
            => kernel.Get<IOperationNodeViewModel>()
                     .InputB.Received().Node = kernel.Get<IOperationNodeViewModel>();

        [Fact]
        public void SetsOutputNode()
            => kernel.Get<IOperationNodeViewModel>()
                     .Output.Received().Node = kernel.Get<IOperationNodeViewModel>();

        [Fact]
        public void SetsDefaultOperation()
            => kernel.Get<IOperationNodeViewModel>()
                     .Operation.Should().Be(OperationNodeViewModel.DefaultOperation);

        [Fact]
        public async void EditNodeCommand_InvokesGetNodeOperationInteraction()
        {
            // Arrange

            var isInvoked = false;
            var input = default(OperationType);
            var output = OperationType.Subtraction;

            interactions.GetNodeOperation
                        .RegisterHandler(i =>
                        {
                            input = i.Input;
                            isInvoked = true;
                            i.SetOutput(output);
                        });

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            await node.EditNodeCommand
                      .Execute();

            // Assert

            isInvoked.Should().BeTrue();
            input.Should().Be(OperationNodeViewModel.DefaultOperation);
            node.Operation.Should().Be(output);
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

            var node = kernel.Get<IOperationNodeViewModel>();

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