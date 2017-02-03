using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectorViewModel : IReactiveObject, ISupportsActivation
    {
    }

    public class ConnectorViewModel : ReactiveObject, IConnectorViewModel
    {
        public ConnectorViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }
    }
}