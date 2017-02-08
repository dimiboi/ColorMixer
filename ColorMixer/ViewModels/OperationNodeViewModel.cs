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
        IInputConnectorViewModel InputA { get; }
        IInputConnectorViewModel InputB { get; }
        IOutputConnectorViewModel Output { get; }
        OperationType Operation { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class OperationNodeViewModel : Node, IOperationNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IInputConnectorViewModel inputA;
        private readonly IInputConnectorViewModel inputB;
        private readonly IOutputConnectorViewModel output;

        private OperationType operation;

        public OperationNodeViewModel(IInteractionService interactions = null,
                                      IInputConnectorViewModel inputA = null,
                                      IInputConnectorViewModel inputB = null,
                                      IOutputConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.inputA = inputA ?? Locator.Current.GetService<IInputConnectorViewModel>();
            this.inputB = inputB ?? Locator.Current.GetService<IInputConnectorViewModel>();
            this.output = output ?? Locator.Current.GetService<IOutputConnectorViewModel>();

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

        public IInputConnectorViewModel InputA => inputA;

        public IInputConnectorViewModel InputB => inputB;

        public IOutputConnectorViewModel Output => output;

        public OperationType Operation
        {
            get { return operation; }
            set { this.RaiseAndSetIfChanged(ref operation, value); }
        }

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}