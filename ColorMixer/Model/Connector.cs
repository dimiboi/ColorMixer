using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;

namespace ColorMixer.Model
{
    public interface IConnector : IReactiveObject, ISupportsActivation
    {
        bool IsConnected { get; }
        bool IsEnabled { get; }
        INode Node { get; set; }
        Point ConnectionPoint { get; set; }
        ConnectorDirection Direction { get; }
        ReactiveCommand<Unit, Unit> ConnectorCommand { get; }
    }

    public abstract class Connector : ReactiveObject, IConnector
    {
        private INode node;
        private Point connectionPoint;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public abstract bool IsConnected { get; }

        public abstract bool IsEnabled { get; }

        public INode Node
        {
            get { return node; }
            set { this.RaiseAndSetIfChanged(ref node, value); }
        }

        public Point ConnectionPoint
        {
            get { return connectionPoint; }
            set { this.RaiseAndSetIfChanged(ref connectionPoint, value); }
        }

        public abstract ConnectorDirection Direction { get; }

        public abstract ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }
    }
}