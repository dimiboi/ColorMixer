using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        IConnectorViewModel From { get; set; }
        IConnectorViewModel To { get; set; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private IConnectorViewModel from;
        private IConnectorViewModel to;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IConnectorViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public IConnectorViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }
    }
}