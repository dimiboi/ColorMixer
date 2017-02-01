using ReactiveUI;
using Splat;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        public MixerViewModel(IScreen screen = null)
        {
            Activator = new ViewModelActivator();
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";
    }
}