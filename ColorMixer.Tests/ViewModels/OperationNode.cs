using ColorMixer.Services;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using ReactiveUI;
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
    }
}