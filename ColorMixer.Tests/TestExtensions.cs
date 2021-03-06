﻿using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.ViewModels;
using FluentAssertions;
using NSubstitute;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        public static async Task ShouldAddNode<TNode>(this IMixerViewModel mixer,
                                                           Point point,
                                                           IInteractionService interactions,
                                                           INodeFactory nodeFactory,
                                                           Func<IMixerViewModel, Task> command)
            where TNode : INode
        {
            // Arrange

            var isInvoked = false;

            interactions.GetNewNodePoint
                        .RegisterHandler(i =>
                        {
                            isInvoked = true;
                            i.SetOutput(point);
                        });
            // Act

            mixer.Activator
                 .Activate();

            await command(mixer);

            var node = mixer.Nodes.Single();

            // Assert

            isInvoked.Should().BeTrue();

            nodeFactory.Received().Create<TNode>();

            node.Should().BeAssignableTo<TNode>();

            node.X.Should().Be(point.X);
            node.Y.Should().Be(point.Y);
        }

        public static async Task ShouldNotAddNode<TNode>(this IMixerViewModel mixer,
                                                         IInteractionService interactions,
                                                         INodeFactory nodeFactory,
                                                         Func<IMixerViewModel, Task> command)
            where TNode : INode
        {
            // Arrange

            var isInvoked = false;

            interactions.GetNewNodePoint
                        .RegisterHandler(i =>
                        {
                            isInvoked = true;
                            i.SetOutput(null);
                        });
            // Act

            mixer.Activator
                 .Activate();

            await command(mixer);

            // Assert

            isInvoked.Should().BeTrue();

            nodeFactory.DidNotReceive().Create<TNode>();

            mixer.Nodes.IsEmpty.Should().BeTrue();
        }

        public static async Task SetOperation(this IOperationNodeViewModel node,
                                                   OperationType operation,
                                                   IInteractionService interactions)
        {
            using (interactions.GetNodeOperation
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