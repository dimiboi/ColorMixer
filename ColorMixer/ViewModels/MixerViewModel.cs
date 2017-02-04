using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IReadOnlyReactiveList<INodeViewModel> Nodes { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly ReactiveList<NodeViewModel> nodes;
        private readonly ReactiveList<ConnectionViewModel> connections;

        public MixerViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            nodes = new ReactiveList<NodeViewModel>();
            connections = new ReactiveList<ConnectionViewModel>();

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });

            var from = new NodeViewModel
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100,
                Color = Colors.Red
            };

            var to = new NodeViewModel
            {
                X = 150,
                Y = 150,
                Width = 100,
                Height = 100,
                Color = Colors.Red
            };

            nodes.Add(from);
            nodes.Add(to);

            connections.Add(new ConnectionViewModel
            {
                From = from,
                To = to
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IReadOnlyReactiveList<INodeViewModel> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;
    }
}