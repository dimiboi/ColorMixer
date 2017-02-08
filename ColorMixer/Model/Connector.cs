using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;

namespace ColorMixer.Model
{
    public interface IConnector : IReactiveObject, ISupportsActivation
    {
        bool IsEnabled { get; }
        Point ConnectionPoint { get; set; }
        ConnectorDirection Direction { get; }
        ReactiveCommand<Unit, Unit> ConnectorCommand { get; }
    }

    public abstract class Connector : ReactiveObject, IConnector
    {
        private Point connectionPoint;

        public Connector()
        {
            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public abstract bool IsEnabled { get; }

        public Point ConnectionPoint
        {
            get { return connectionPoint; }
            set { this.RaiseAndSetIfChanged(ref connectionPoint, value); }
        }

        public abstract ConnectorDirection Direction { get; }

        public abstract ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }
    }
}