using ColorMixer.Model;
using ColorMixer.Services;
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
    public class ResultNode
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;
        private readonly IInConnectorViewModel input;

        public ResultNode()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel>();
            input = Substitute.For<IInConnectorViewModel, ReactiveObject>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<IInConnectorViewModel>()
                  .ToConstant(input);

            kernel.Bind<IResultNodeViewModel>()
                  .To<ResultNodeViewModel>()
                  .InSingletonScope(); // system under test

            input.ConnectedTo = Arg.Do<IOutConnectorViewModel>(
                _ => input.RaisePropertyChanged(nameof(input.ConnectedTo)));
        }

        [Fact]
        public void SetsInput()
            => kernel.Get<IResultNodeViewModel>()
                     .Input.Should().Be(input);

        [Fact]
        public void SetsInputNode()
            => kernel.Get<IResultNodeViewModel>()
                     .Input.Received().Node = kernel.Get<IResultNodeViewModel>();

        [Fact]
        public async void SetsColor_WhenConnected()
        {
            // Arrange

            var expected = Colors.Pink;

            var source = Substitute.For<INode>();
            source.Color.Returns(expected);

            var connector = Substitute.For<IOutConnectorViewModel>();
            connector.Node.Returns(source);

            var node = kernel.Get<IResultNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            input.ConnectedTo = connector;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Fact]
        public async void SetsDefaultColor_WhenDisconnected()
        {
            // Arrange

            var expected = Node.DefaultColor;

            var source = Substitute.For<INode>();
            source.Color.Returns(Colors.Pink);

            var connector = Substitute.For<IOutConnectorViewModel>();
            connector.Node.Returns(source);

            var node = kernel.Get<IResultNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            input.ConnectedTo = connector;
            input.ConnectedTo = null;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }

        [Fact]
        public async void UpdatesColor_WhenSourceColorChanges()
        {
            // Arrange

            var expected = Colors.Pink;

            var source = Substitute.For<INode, ReactiveObject>();
            source.Color.Returns(Colors.SteelBlue);

            var connector = Substitute.For<IOutConnectorViewModel>();
            connector.Node.Returns(source);

            input.ConnectedTo.Returns(connector);

            source.Color = Arg.Do<Color>(
                _ => source.RaisePropertyChanged(nameof(source.Color)));

            var node = kernel.Get<IResultNodeViewModel>();

            // Act

            node.Activator
                .Activate();

            source.Color = expected;

            var actual = await node.WhenAnyValue(vm => vm.Color)
                                   .FirstAsync();
            // Assert

            actual.Should().Be(expected);
        }
    }
}