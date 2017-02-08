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
        IConnectorViewModel InputA { get; }
        IConnectorViewModel InputB { get; }
        IConnectorViewModel Output { get; }
        OperationType Operation { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class OperationNodeViewModel : Node, IOperationNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IConnectorViewModel inputA;
        private readonly IConnectorViewModel inputB;
        private readonly IConnectorViewModel output;

        private OperationType operation;

        public OperationNodeViewModel(IInteractionService interactions = null,
                                      IConnectorViewModel inputA = null,
                                      IConnectorViewModel inputB = null,
                                      IConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.inputA = inputA ?? Locator.Current.GetService<IConnectorViewModel>();
            this.inputB = inputB ?? Locator.Current.GetService<IConnectorViewModel>();
            this.output = output ?? Locator.Current.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                EditNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    Operation = await this.interactions
                                          .GetNodeOperation
                                          .Handle(Operation);
                })
                .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel InputA => inputA;

        public IConnectorViewModel InputB => inputB;

        public IConnectorViewModel Output => output;

        public OperationType Operation
        {
            get { return operation; }
            set { this.RaiseAndSetIfChanged(ref operation, value); }
        }

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}