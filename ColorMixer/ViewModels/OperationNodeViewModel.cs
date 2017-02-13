using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IOperationNodeViewModel : INode
    {
        IInConnectorViewModel InputA { get; }
        IInConnectorViewModel InputB { get; }
        IOutConnectorViewModel Output { get; }
        OperationType Operation { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class OperationNodeViewModel : Node, IOperationNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;

        private IInConnectorViewModel inputA;
        private IInConnectorViewModel inputB;
        private IOutConnectorViewModel output;

        private OperationType operation;

        public OperationNodeViewModel(IInteractionService interactions = null,
                                      IMixerViewModel mixer = null,
                                      IInConnectorViewModel inputA = null,
                                      IInConnectorViewModel inputB = null,
                                      IOutConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();

            InputA = inputA ?? Locator.Current.GetService<IInConnectorViewModel>();
            InputB = inputB ?? Locator.Current.GetService<IInConnectorViewModel>();
            Output = output ?? Locator.Current.GetService<IOutConnectorViewModel>();

            InputA.Node = this;
            InputB.Node = this;
            Output.Node = this;

            this.WhenActivated(disposables =>
            {
                this // Handle the connections
                    .WhenAnyValue(vm => vm.InputA.ConnectedTo,
                                  vm => vm.InputB.ConnectedTo,
                                  vm => vm.Operation,
                                  (a, b, op) => new { A = a, B = b, Op = op })
                    .Select(i => i.A != null && i.B != null
                                 ? Execute(i.A.Node.Color, i.B.Node.Color, i.Op)
                                 : DefaultColor)
                    .BindTo(this, vm => vm.Color)
                    .DisposeWith(disposables);

                this // Handle the color
                    .WhenAnyValue(vm => vm.InputA.ConnectedTo.Node.Color,
                                  vm => vm.InputB.ConnectedTo.Node.Color,
                                  vm => vm.Operation,
                                  (a, b, op) => new { A = a, B = b, Op = op })
                    .Where(_ => InputA?.ConnectedTo?.Node != null && // when nodes are connected
                                InputB?.ConnectedTo?.Node != null)
                    .Select(i => Execute(i.A, i.B, i.Op))
                    .BindTo(this, vm => vm.Color)
                    .DisposeWith(disposables);

                EditNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    Operation = await this.interactions
                                          .GetNodeOperation
                                          .Handle(Operation);
                },
                this.WhenAnyValue(vm => vm.mixer.IsNodeBeingAdded,
                                  vm => vm.mixer.ConnectingConnector,
                                  (a, b) => !a && b == null))
                .DisposeWith(disposables);
            });
        }

        public IInConnectorViewModel InputA
        {
            get { return inputA; }
            private set { this.RaiseAndSetIfChanged(ref inputA, value); }
        }

        public IInConnectorViewModel InputB
        {
            get { return inputB; }
            private set { this.RaiseAndSetIfChanged(ref inputB, value); }
        }

        public IOutConnectorViewModel Output
        {
            get { return output; }
            private set { this.RaiseAndSetIfChanged(ref output, value); }
        }

        public OperationType Operation
        {
            get { return operation; }
            private set { this.RaiseAndSetIfChanged(ref operation, value); }
        }

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }

        private Color Execute(Color a, Color b, OperationType operation)
        {
            switch (operation)
            {
                case OperationType.Addition:
                    return new Color
                    {
                        R = (byte)(a.R + b.R < 255
                                   ? a.R + b.R
                                   : 255),
                        G = (byte)(a.G + b.G < 255
                                   ? a.G + b.G
                                   : 255),
                        B = (byte)(a.B + b.B < 255
                                   ? a.B + b.B
                                   : 255),
                        A = 255
                    };

                case OperationType.Subtraction:
                    return new Color
                    {
                        R = (byte)(a.R - b.R > 0
                                   ? a.R - b.R
                                   : 0),
                        G = (byte)(a.G - b.G > 0
                                   ? a.G - b.G
                                   : 0),
                        B = (byte)(a.B - b.B > 0
                                   ? a.B - b.B
                                   : 0),
                        A = 255
                    };

                default:
                    throw new ArgumentException($"Operation '{operation}' is unknown.",
                                                nameof(operation));
            }
        }
    }
}