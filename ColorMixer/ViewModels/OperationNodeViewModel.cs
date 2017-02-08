using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
        private readonly IInConnectorViewModel inputA;
        private readonly IInConnectorViewModel inputB;
        private readonly IOutConnectorViewModel output;

        private OperationType operation;

        public OperationNodeViewModel(IInteractionService interactions = null,
                                      IMixerViewModel mixer = null,
                                      IInConnectorViewModel inputA = null,
                                      IInConnectorViewModel inputB = null,
                                      IOutConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();
            this.inputA = inputA ?? Locator.Current.GetService<IInConnectorViewModel>();
            this.inputB = inputB ?? Locator.Current.GetService<IInConnectorViewModel>();
            this.output = output ?? Locator.Current.GetService<IOutConnectorViewModel>();

            this.inputA.Node = this;
            this.inputB.Node = this;
            this.output.Node = this;

            this.WhenActivated(disposables =>
            {
                EditNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    Operation = await this.interactions
                                          .GetNodeOperation
                                          .Handle(Operation);
                },
                this.WhenAnyValue(vm => vm.mixer.IsNodeBeingAdded, b => !b))
                .DisposeWith(disposables);
            });
        }

        public IInConnectorViewModel InputA => inputA;

        public IInConnectorViewModel InputB => inputB;

        public IOutConnectorViewModel Output => output;

        public OperationType Operation
        {
            get { return operation; }
            set { this.RaiseAndSetIfChanged(ref operation, value); }
        }

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}