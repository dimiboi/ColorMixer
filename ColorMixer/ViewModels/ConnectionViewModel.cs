using ReactiveUI;

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

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

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