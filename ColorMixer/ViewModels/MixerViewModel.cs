using ReactiveUI;
using Splat;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IReadOnlyReactiveList<IRectangleViewModel> Rectangles { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly ReactiveList<RectangleViewModel> rectangles;

        public MixerViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            Activator = new ViewModelActivator();

            rectangles = new ReactiveList<RectangleViewModel>();

            rectangles.Add(new RectangleViewModel
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100,
                Color = Colors.Red
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IReadOnlyReactiveList<IRectangleViewModel> Rectangles => rectangles;
    }
}