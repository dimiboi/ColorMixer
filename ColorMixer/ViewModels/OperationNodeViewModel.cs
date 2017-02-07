using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IOperationNodeViewModel : INode
    {
        IConnectorViewModel InputA { get; }
        IConnectorViewModel InputB { get; }
        IConnectorViewModel Output { get; }
        OperationType Operation { get; }
    }

    public class OperationNodeViewModel : Node, IOperationNodeViewModel
    {
        private readonly IConnectorViewModel inputA;
        private readonly IConnectorViewModel inputB;
        private readonly IConnectorViewModel output;

        private OperationType operation;

        public OperationNodeViewModel(IDependencyResolver resolver = null,
                                      IConnectorViewModel inputA = null,
                                      IConnectorViewModel inputB = null,
                                      IConnectorViewModel output = null)
        {
            resolver = resolver ?? Locator.Current;

            this.inputA = inputA ?? resolver.GetService<IConnectorViewModel>();
            this.inputB = inputB ?? resolver.GetService<IConnectorViewModel>();
            this.output = output ?? resolver.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
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
    }
}