using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        IOutConnectorViewModel From { get; set; }
        IInConnectorViewModel To { get; set; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private IOutConnectorViewModel from;
        private IInConnectorViewModel to;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IOutConnectorViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public IInConnectorViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }
    }
}