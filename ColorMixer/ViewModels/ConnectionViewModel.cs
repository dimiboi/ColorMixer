using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        IRectangleViewModel From { get; set; }
        IRectangleViewModel To { get; set; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private IRectangleViewModel from;
        private IRectangleViewModel to;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IRectangleViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public IRectangleViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }
    }
}