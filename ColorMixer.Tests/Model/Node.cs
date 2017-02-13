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
            mixer = Substitute.For<IMixerViewModel, ReactiveObject>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<INode>()
                  .To<TestNode>(); // system under test
        }

        [Fact]
        public void SetsActivator()
            => kernel.Get<INode>()
                     .Activator.Should().NotBeNull();

        [Fact]
        public void SetsDefaultWidth()
            => kernel.Get<INode>()
                     .Width.Should().Be(Node.DefaultWidth);

        [Fact]
        public void SetsDefaultHeight()
            => kernel.Get<INode>()
                     .Width.Should().Be(Node.DefaultHeight);

        [Fact]
        public void SetsDefaultColor()
            => kernel.Get<INode>()
                     .Color.Should().Be(Node.DefaultColor);

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

            var node = kernel.Get<INode>();

            // Act

            node.Activator
                .Activate();

            node.Color = color;

            // Assert

            node.Title.Should().Be(title);
        }

        [Fact]
        public async void DeleteNodeCommand_InvokesDeleteNodeInteraction()
        {
            // Arrange

            var input = default(INode);

            interactions.DeleteNode
                        .RegisterHandler(i =>
                        {
                            input = i.Input;
                            i.SetOutput(Unit.Default);
                        });

            var node = kernel.Get<INode>();

            // Act

            node.Activator
                .Activate();

            await node.DeleteNodeCommand
                      .Execute();

            // Assert

            input.Should().Be(node);
        }

        [Theory]
        [InlineData(true, false, default(IConnector))]
        [InlineData(false, true, default(IConnector))]
        [InlineAutoNSubstituteData(false, false)]
        public async void DeleteNodeCommand_CanExecute(bool expected,
                                                       bool isNodeBeingAdded,
                                                       IConnector connectingConnector)
        {
            // Arrange

            var node = kernel.Get<INode>();

            // Act

            node.Activator
                .Activate();

            mixer.IsNodeBeingAdded
                 .Returns(isNodeBeingAdded);

            mixer.RaisePropertyChanged(nameof(mixer.IsNodeBeingAdded));

            mixer.ConnectingConnector
                 .Returns(connectingConnector);

            mixer.RaisePropertyChanged(nameof(mixer.ConnectingConnector));

            var canExecute = await node.DeleteNodeCommand
                                       .CanExecute
                                       .FirstAsync();
            // Assert

            canExecute.Should().Be(expected);
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