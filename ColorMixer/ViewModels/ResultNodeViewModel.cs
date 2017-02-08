using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IResultNodeViewModel : INode
    {
        IInConnectorViewModel Input { get; }
    }

    public class ResultNodeViewModel : Node, IResultNodeViewModel
    {
        private readonly IInConnectorViewModel input;

        public ResultNodeViewModel(IInConnectorViewModel input = null)
        {
            this.input = input ?? Locator.Current.GetService<IInConnectorViewModel>();

            this.input.Node = this;

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public IInConnectorViewModel Input => input;
    }
}