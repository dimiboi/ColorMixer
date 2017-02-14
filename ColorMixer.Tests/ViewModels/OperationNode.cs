using ColorMixer.Extensions;
using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Media;
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

            inputA.ConnectedTo = Arg.Do<IOutConnectorViewModel>(
                _ => inputA.RaisePropertyChanged(nameof(inputA.ConnectedTo)));

            inputB.ConnectedTo = Arg.Do<IOutConnectorViewModel>(
                _ => inputB.RaisePropertyChanged(nameof(inputB.ConnectedTo)));
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

        [Theory]
        [InlineAutoData(OperationType.Addition)]
        [InlineAutoData(OperationType.Subtraction)]
        public async void SetsColor_WhenConnected(OperationType operation,
                                                  byte rA, byte gA, byte bA,
                                                  byte rB, byte gB, byte bB)
        {
            // Arrange

            var colorA = Color.FromRgb(rA, gA, bA);
            var colorB = Color.FromRgb(rB, gB, bB);

            var expected = default(Color);

            if (operation == OperationType.Addition)
            {
                expected = colorA.Add(colorB);
            }
            else if (operation == OperationType.Subtraction)
            {
                expected = colorA.Subtract(colorB);
            }

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            await node.SetOperation(operation, interactions);

            inputA.ConnectToNodeWithColor(colorA);
            inputB.ConnectToNodeWithColor(colorB);

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenADisconnected(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
        {
            // Arrange

            var colorA = Color.FromRgb(rA, gA, bA);
            var colorB = Color.FromRgb(rB, gB, bB);

            var expected = Node.DefaultColor;

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            inputA.ConnectToNodeWithColor(colorA);
            inputB.ConnectToNodeWithColor(colorB);
            inputA.ConnectedTo = null;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenBDisconnected(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
        {
            // Arrange

            var colorA = Color.FromRgb(rA, gA, bA);
            var colorB = Color.FromRgb(rB, gB, bB);

            var expected = Node.DefaultColor;

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            inputA.ConnectToNodeWithColor(colorA);
            inputB.ConnectToNodeWithColor(colorB);
            inputB.ConnectedTo = null;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenBothDisconnected(byte rA, byte gA, byte bA,
                                                                byte rB, byte gB, byte bB)
        {
            // Arrange

            var colorA = Color.FromRgb(rA, gA, bA);
            var colorB = Color.FromRgb(rB, gB, bB);

            var expected = Node.DefaultColor;

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            inputA.ConnectToNodeWithColor(colorA);
            inputB.ConnectToNodeWithColor(colorB);
            inputA.ConnectedTo = null;
            inputB.ConnectedTo = null;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();

            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenOperartionChanged(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
        {
            // Arrange

            var colorA = Color.FromRgb(rA, gA, bA);
            var colorB = Color.FromRgb(rB, gB, bB);

            var expectedBefore = colorA.Add(colorB);
            var expectedAfter = colorA.Subtract(colorB);

            var node = kernel.Get<IOperationNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            inputA.ConnectToNodeWithColor(colorA);
            inputB.ConnectToNodeWithColor(colorB);

            var before = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();

            await node.SetOperation(OperationType.Subtraction, interactions);

            var after = await node.WhenAnyValue(vm => vm.Color)
                                  .FirstAsync();
            // Assert

            before.Should().Be(expectedBefore);
            after.Should().Be(expectedAfter);
        }

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