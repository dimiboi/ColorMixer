using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.ViewModels;
using FluentAssertions;
using NSubstitute;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ColorMixer.Tests
{
    public static class TestExtensions
    {
        // Gets a property using reflection
        public static object GetProperty<T>(this T obj, string property)
            => obj.GetType()
                  .GetProperty(property)
                  .GetValue(obj, null);

        // Sets a property using reflection
        public static void SetProperty<T>(this T obj, string property, object value)
            => obj.GetType()
                  .GetProperty(property)
                  .SetValue(obj, value);

        public static void ShouldSetProperty<TClass, TProp>(this TClass obj, string property,
                                                                 TProp initial, TProp expected)
            where TClass : class, INotifyPropertyChanged
        {
            // Arrange

            var raised = default(string);

            obj.SetProperty(property, initial);
            obj.PropertyChanged += (s, e) => raised = e.PropertyName;

            // Act

            obj.SetProperty(property, expected);

            // Assert

            obj.GetProperty(property).Should().Be(expected);
            raised.Should().Be(property);
        }

        public static async Task ShouldUpdateColor<TNode>(this TNode node,
                                                          Color expectedBefore,
                                                          Color expectedAfter,
                                                          Action<TNode> before,
                                                          Action<TNode> after)
            where TNode : class, INode
        {
            // Act

            node.Activator
                .Activate();

            before(node);

            var actualBefore = await node.WhenAnyValue(vm => vm.Color)
                                         .FirstAsync();
            after(node);

            var actualAfter = await node.WhenAnyValue(vm => vm.Color)
                                        .FirstAsync();
            // Assert

            actualBefore.Should().Be(expectedBefore);
            actualAfter.Should().Be(expectedAfter);
        }

        public static async Task SetOperation(this IOperationNodeViewModel node,
                                                   OperationType operation,
                                                   IInteractionService interactions)
        {
            using (var handler = interactions.GetNodeOperation
                                             .RegisterHandler(i => i.SetOutput(operation)))
            {
                await node.EditNodeCommand
                          .Execute();
            }
        }

        public static void ConnectToNode(this IInConnectorViewModel input, INode node)
        {
            var output = Substitute.For<IOutConnectorViewModel>();
            output.Node.Returns(node);

            input.ConnectedTo = output;
        }

        public static void ConnectToNode(this IOutConnectorViewModel input, INode node)
        {
            var output = Substitute.For<IInConnectorViewModel>();
            output.Node.Returns(node);

            input.ConnectedTo = output;
        }

        public static void ConnectToNodeWithColor(this IInConnectorViewModel input,
                                                       Color color)
            => input.ConnectToNode(CreateNode(color));

        public static void ConnectToNodeWithColor(this IOutConnectorViewModel input,
                                                       Color color)
            => input.ConnectToNode(CreateNode(color));

        private static INode CreateNode(Color color)
        {
            var node = Substitute.For<INode>();

            node.Color = Arg.Do<Color>(
                _ => node.RaisePropertyChanged(nameof(node.Color)));

            node.Color = color;

            return node;
        }
    }
}