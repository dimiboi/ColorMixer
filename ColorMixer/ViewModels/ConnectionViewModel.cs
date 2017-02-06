using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        IColorNodeViewModel From { get; set; }
        IColorNodeViewModel To { get; set; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private IColorNodeViewModel from;
        private IColorNodeViewModel to;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IColorNodeViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public IColorNodeViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }
    }
}