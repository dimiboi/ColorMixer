using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IReadOnlyReactiveList<INodeViewModel> Nodes { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
        ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; }
        ReactiveCommand<INodeViewModel, Unit> DeleteNodeCommand { get; }
        Interaction<Unit, Point?> GetNewNodePoint { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly ReactiveList<INodeViewModel> nodes;
        private readonly ReactiveList<IConnectionViewModel> connections;

        public MixerViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            nodes = new ReactiveList<INodeViewModel>();
            connections = new ReactiveList<IConnectionViewModel>();

            Activator = new ViewModelActivator();
            GetNewNodePoint = new Interaction<Unit, Point?>();

            this.WhenActivated(disposables =>
            {
                AddColorNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var point = await GetNewNodePoint.Handle(Unit.Default);

                    if (point.HasValue) // a point has been selected
                    {
                        nodes.Add(new NodeViewModel
                        {
                            X = point.Value.X,
                            Y = point.Value.Y,
                            Width = 150,
                            Height = 150,
                            Color = Colors.Red
                        });
                    }
                })
                .DisposeWith(disposables);

                DeleteNodeCommand = ReactiveCommand.Create<INodeViewModel>(node =>
                {
                    foreach (var connection in connections.Where(c => c.To == node ||
                                                                      c.From == node)
                                                          .ToArray())
                    {
                        connections.Remove(connection);
                    }

                    nodes.Remove(node);
                })
                .DisposeWith(disposables);
            });
        }

        private void WhenActivated(Action<Action<IDisposable>> p)
        {
            throw new NotImplementedException();
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IReadOnlyReactiveList<INodeViewModel> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;

        public ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; private set; }

        public ReactiveCommand<INodeViewModel, Unit> DeleteNodeCommand { get; private set; }

        public Interaction<Unit, Point?> GetNewNodePoint { get; private set; }
    }
}