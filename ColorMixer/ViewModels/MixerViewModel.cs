using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IReadOnlyReactiveList<IRectangleViewModel> Rectangles { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly ReactiveList<RectangleViewModel> rectangles;
        private readonly ReactiveList<ConnectionViewModel> connections;

        public MixerViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            rectangles = new ReactiveList<RectangleViewModel>();
            connections = new ReactiveList<ConnectionViewModel>();

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });

            var from = new RectangleViewModel
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100,
                Color = Colors.Red
            };

            var to = new RectangleViewModel
            {
                X = 150,
                Y = 150,
                Width = 100,
                Height = 100,
                Color = Colors.Red
            };

            rectangles.Add(from);
            rectangles.Add(to);

            connections.Add(new ConnectionViewModel
            {
                From = from,
                To = to
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IReadOnlyReactiveList<IRectangleViewModel> Rectangles => rectangles;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;
    }
}