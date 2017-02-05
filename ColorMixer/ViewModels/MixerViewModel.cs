using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IReadOnlyReactiveList<INodeViewModel> Nodes { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
        ReactiveCommand<Unit, Unit> StartAddingColorNodeCommand { get; }
        ReactiveCommand<Point, Unit> EndAddingColorNodeCommand { get; }
        bool IsAddingNode { get; set; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly ReactiveList<NodeViewModel> nodes;
        private readonly ReactiveList<ConnectionViewModel> connections;

        private bool isAddingNode;

        public MixerViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            nodes = new ReactiveList<NodeViewModel>();
            connections = new ReactiveList<ConnectionViewModel>();

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                StartAddingColorNodeCommand = ReactiveCommand.Create(() =>
                {
                    IsAddingNode = true;
                },
                this.WhenAnyValue(vm => vm.IsAddingNode, b => !b)).DisposeWith(disposables);

                EndAddingColorNodeCommand = ReactiveCommand.Create<Point>(p =>
                {
                    nodes.Add(new NodeViewModel
                    {
                        X = p.X,
                        Y = p.Y,
                        Width = 150,
                        Height = 150,
                        Color = Colors.Red
                    });

                    IsAddingNode = false;
                },
                this.WhenAnyValue(vm => vm.IsAddingNode)).DisposeWith(disposables);
            });

            /*var node1 = new NodeViewModel
            {
                X = 10,
                Y = 10,
                Width = 150,
                Height = 150,
                Color = Colors.Red
            };

            var node2 = new NodeViewModel
            {
                X = 150,
                Y = 150,
                Width = 150,
                Height = 150,
                Color = Colors.Red
            };

            var node3 = new NodeViewModel
            {
                X = 300,
                Y = 100,
                Width = 150,
                Height = 150,
                Color = Colors.Red
            };

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);

            connections.Add(new ConnectionViewModel
            {
                From = node1,
                To = node2
            });

            connections.Add(new ConnectionViewModel
            {
                From = node2,
                To = node3
            });

            connections.Add(new ConnectionViewModel
            {
                From = node3,
                To = node1
            });*/
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IReadOnlyReactiveList<INodeViewModel> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;

        public ReactiveCommand<Unit, Unit> StartAddingColorNodeCommand { get; private set; }

        public ReactiveCommand<Point, Unit> EndAddingColorNodeCommand { get; private set; }

        public bool IsAddingNode
        {
            get { return isAddingNode; }
            set { this.RaiseAndSetIfChanged(ref isAddingNode, value); }
        }
    }
}