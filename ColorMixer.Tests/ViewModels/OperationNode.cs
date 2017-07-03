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
using System.Reactive.Concurrency;
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
            RxApp.MainThreadScheduler = Scheduler.Immediate;

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
        [InlineAutoData(42)]
        [InlineAutoData(OperationType.Addition)]
        [InlineAutoData(OperationType.Subtraction)]
        public async void SetsColor_WhenBothConnected(OperationType operation,
                                                      byte rA, byte gA, byte bA,
                                                      byte rB, byte gB, byte bB)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                operation == OperationType.Addition
                ? Color.FromArgb(byte.MaxValue, rA, gA, bA)
                       .Add(Color.FromArgb(byte.MaxValue, rB, gB, bB))
                : operation == OperationType.Subtraction
                  ? Color.FromArgb(byte.MaxValue, rA, gA, bA)
                         .Subtract(Color.FromArgb(0, rB, gB, bB))
                  : Node.DefaultColor,
                node => { },
                async node =>
                {
                    await node.SetOperation(operation, interactions);

                    inputA.ConnectToNodeWithColor(Color.FromArgb(byte.MaxValue, rA, gA, bA));
                    inputB.ConnectToNodeWithColor(Color.FromArgb(byte.MaxValue, rB, gB, bB));
                });

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenAConnected(byte r, byte g, byte b)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Node.DefaultColor,
                node => { },
                node => inputA.ConnectToNodeWithColor(Color.FromRgb(r, g, b)));

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenBConnected(byte r, byte g, byte b)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Node.DefaultColor,
                node => { },
                node => inputB.ConnectToNodeWithColor(Color.FromRgb(r, g, b)));

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenADisconnected(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Node.DefaultColor,
                node => { },
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA, gA, bA));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB, gB, bB));
                    inputA.ConnectedTo = null;
                });

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenBDisconnected(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Node.DefaultColor,
                node => { },
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA, gA, bA));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB, gB, bB));
                    inputB.ConnectedTo = null;
                });

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenBothDisconnected(byte rA, byte gA, byte bA,
                                                                byte rB, byte gB, byte bB)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Node.DefaultColor,
                node => { },
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA, gA, bA));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB, gB, bB));
                    inputA.ConnectedTo = null;
                    inputB.ConnectedTo = null;
                });

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenOperartionChanged(byte rA, byte gA, byte bA,
                                                             byte rB, byte gB, byte bB)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Color.FromArgb(byte.MaxValue, rA, gA, bA)
                     .Add(Color.FromRgb(rB, gB, bB)),
                Color.FromArgb(byte.MaxValue, rA, gA, bA)
                     .Subtract(Color.FromArgb(0, rB, gB, bB)),
                async node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromArgb(byte.MaxValue, rA, gA, bA));
                    inputB.ConnectToNodeWithColor(Color.FromArgb(byte.MaxValue, rB, gB, bB));

                    await node.SetOperation(OperationType.Addition, interactions);
                },
                async node =>
                {
                    await node.SetOperation(OperationType.Subtraction, interactions);
                });

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenColorAChanged(byte rA1, byte gA1, byte bA1,
                                                         byte rB1, byte gB1, byte bB1,
                                                         byte rA2, byte gA2, byte bA2)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Color.FromRgb(rA1, gA1, bA1)
                     .Add(Color.FromRgb(rB1, gB1, bB1)),
                Color.FromRgb(rA2, gA2, bA2)
                     .Add(Color.FromRgb(rB1, gB1, bB1)),
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA1, gA1, bA1));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB1, gB1, bB1));
                },
                node =>
                {
                    inputA.ConnectedTo.Node.Color = Color.FromRgb(rA2, gA2, bA2);
                });

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenColorBChanged(byte rA1, byte gA1, byte bA1,
                                                         byte rB1, byte gB1, byte bB1,
                                                         byte rB2, byte gB2, byte bB2)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Color.FromRgb(rA1, gA1, bA1)
                     .Add(Color.FromRgb(rB1, gB1, bB1)),
                Color.FromRgb(rA1, gA1, bA1)
                     .Add(Color.FromRgb(rB2, gB2, bB2)),
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA1, gA1, bA1));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB1, gB1, bB1));
                },
                node =>
                {
                    inputB.ConnectedTo.Node.Color = Color.FromRgb(rB2, gB2, bB2);
                });

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenBothColorsChanged(byte rA1, byte gA1, byte bA1,
                                                             byte rB1, byte gB1, byte bB1,
                                                             byte rA2, byte gA2, byte bA2,
                                                             byte rB2, byte gB2, byte bB2)
            => await kernel.Get<IOperationNodeViewModel>().ShouldUpdateColor(
                Color.FromRgb(rA1, gA1, bA1)
                     .Add(Color.FromRgb(rB1, gB1, bB1)),
                Color.FromRgb(rA2, gA2, bA2)
                     .Add(Color.FromRgb(rB2, gB2, bB2)),
                node =>
                {
                    inputA.ConnectToNodeWithColor(Color.FromRgb(rA1, gA1, bA1));
                    inputB.ConnectToNodeWithColor(Color.FromRgb(rB1, gB1, bB1));
                },
                node =>
                {
                    inputA.ConnectedTo.Node.Color = Color.FromRgb(rA2, gA2, bA2);
                    inputB.ConnectedTo.Node.Color = Color.FromRgb(rB2, gB2, bB2);
                });

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