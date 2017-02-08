using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IResultNodeViewModel : INode
    {
        IInputConnectorViewModel Input { get; }
    }

    public class ResultNodeViewModel : Node, IResultNodeViewModel
    {
        private readonly IInputConnectorViewModel input;

        public ResultNodeViewModel(IInputConnectorViewModel input = null)
        {
            this.input = input ?? Locator.Current.GetService<IInputConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public IInputConnectorViewModel Input => input;
    }
}