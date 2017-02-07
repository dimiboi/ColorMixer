using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IColorNodeViewModel : INode
    {
        IConnectorViewModel Connector { get; }
    }

    public class ColorNodeViewModel : Node, IColorNodeViewModel
    {
        private readonly IConnectorViewModel connector;

        public ColorNodeViewModel(IDependencyResolver resolver = null,
                                  IConnectorViewModel connector = null)
        {
            resolver = resolver ?? Locator.Current;

            this.connector = connector ?? resolver.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel Connector => connector;
    }
}