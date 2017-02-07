using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IResultNodeViewModel : INode
    {
        IConnectorViewModel Connector { get; }
    }

    public class ResultNodeViewModel : Node, IResultNodeViewModel
    {
        private readonly IConnectorViewModel connector;

        public ResultNodeViewModel(IConnectorViewModel connector = null)
        {
            this.connector = connector ?? Locator.Current.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel Connector => connector;
    }
}