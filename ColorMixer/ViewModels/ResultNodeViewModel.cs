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

        public ResultNodeViewModel(IDependencyResolver resolver = null,
                                   IConnectorViewModel connector = null)
        {
            resolver = resolver ?? Locator.Current;
            connector = connector ?? resolver.GetService<IConnectorViewModel>();

            this.connector = connector;

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel Connector => connector;
    }
}