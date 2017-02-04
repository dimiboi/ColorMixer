using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;

namespace ColorMixer.ViewModels
{
    public interface IConnectorViewModel : IReactiveObject, ISupportsActivation
    {
        Point ConnectionPoint { get; set; }
    }

    public class ConnectorViewModel : ReactiveObject, IConnectorViewModel
    {
        private Point connectionPoint;

        public ConnectorViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public Point ConnectionPoint
        {
            get { return connectionPoint; }
            set { this.RaiseAndSetIfChanged(ref connectionPoint, value); }
        }
    }
}