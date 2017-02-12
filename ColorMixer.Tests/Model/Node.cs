using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using Xunit;

namespace Model
{
    public class TestNode : Node
    {
        public TestNode(IInteractionService interactions,
                        IMixerViewModel mixer)
            : base(interactions, mixer)
        {
        }
    }

    public class NodeModel
    {
        private readonly IKernel kernel;
        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;

        public NodeModel()
        {
            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<INode>()
                  .To<TestNode>(); // system under test
        }

        [Fact]
        public void SetsActivator()
        {
            // Act

            var connector = kernel.Get<INode>();

            // Assert

            connector.Activator.Should().NotBeNull();
        }

        [Fact]
        public void SetsDefaultWidth()
        {
            // Act

            var node = kernel.Get<INode>();

            // Assert

            node.Width.Should().Be(Node.DefaultWidth);
        }

        [Fact]
        public void SetsDefaultHeight()
        {
            // Act

            var node = kernel.Get<INode>();

            // Assert

            node.Width.Should().Be(Node.DefaultHeight);
        }

        [Fact]
        public void SetsDefaultColor()
        {
            // Act

            var node = kernel.Get<INode>();

            // Assert

            node.Color.Should().Be(Node.DefaultColor);
        }

        [Fact]
        public void SetsTitleFromColor()
        {
            // Arrange

            var color = new Color
            {
                R = 12,
                G = 34,
                B = 56
            };

            var title = $"R: {color.R} / G: {color.G} / B {color.B}";

            // Act

            var node = kernel.Get<INode>();

            node.Activator.Activate();
            node.Color = color;

            // Assert

            node.Title.Should().Be(title);
        }

        [Fact]
        public async void DeleteNodeCommand_InvokesDeleteNodeInteraction()
        {
            // Arrange

            INode input = null;

            interactions.DeleteNode
                        .RegisterHandler(i =>
                        {
                            input = i.Input;
                            i.SetOutput(Unit.Default);
                        });
            // Act

            var node = kernel.Get<INode>();

            node.Activator
                .Activate();

            await node.DeleteNodeCommand
                      .Execute();

            // Assert

            input.Should().Be(node);
        }

        [Fact]
        public async void DeleteNodeCommand_CanExecute()
        {
            // Arrange

            mixer.IsNodeBeingAdded
                 .Returns(false);

            mixer.ConnectingConnector
                 .Returns(default(IConnector));

            // Act

            var node = kernel.Get<INode>();

            node.Activator
                .Activate();

            var canExecute = await node.DeleteNodeCommand
                                       .CanExecute
                                       .FirstAsync();
            // Assert

            canExecute.Should().BeTrue();
        }

        [Fact]
        public async void DeleteNodeCommand_CannotExecute_WhenNodeIsBeingAdded()
        {
            // Arrange

            mixer.IsNodeBeingAdded
                 .Returns(true);

            mixer.ConnectingConnector
                 .Returns(default(IConnector));

            // Act

            var node = kernel.Get<INode>();

            node.Activator
                .Activate();

            var canExecute = await node.DeleteNodeCommand
                                       .CanExecute
                                       .FirstAsync();
            // Assert

            canExecute.Should().BeFalse();
        }

        [Fact]
        public async void DeleteNodeCommand_CannotExecute_WhenConnectionIsCreated()
        {
            // Arrange

            mixer.IsNodeBeingAdded
                 .Returns(false);

            mixer.ConnectingConnector
                 .Returns(Substitute.For<IConnector>());

            // Act

            var node = kernel.Get<INode>();

            node.Activator
                .Activate();

            var canExecute = await node.DeleteNodeCommand
                                       .CanExecute
                                       .FirstAsync();
            // Assert

            canExecute.Should().BeFalse();
        }

        [Theory]
        [InlineAutoData(nameof(Node.X))]
        [InlineAutoData(nameof(Node.Y))]
        [InlineAutoData(nameof(Node.Width))]
        [InlineAutoData(nameof(Node.Height))]
        public void SetsDoubleProperties(string property, double initial, double expected)
            => kernel.Get<INode>()
                     .ShouldSetProperty(property, initial, expected);

        [Theory]
        [InlineAutoData(nameof(Node.Color))]
        public void SetsColorProperties(string property,
                                   byte initialR, byte initialG, byte initialB,
                                   byte expectedR, byte expectedG, byte expectedB)
            => kernel.Get<INode>()
                     .ShouldSetProperty(property,
                                        Color.FromRgb(initialR, initialG, initialB),
                                        Color.FromRgb(expectedR, expectedG, expectedB));
    }
}