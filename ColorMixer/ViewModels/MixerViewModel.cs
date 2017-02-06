using ColorMixer.Extensions;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IMixerViewModel : IReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        IObservable<KeyEventArgs> MainWindowKeyDown { get; }
        IReadOnlyReactiveList<INodeViewModel> Nodes { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
        ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; }
        Interaction<Unit, Point?> GetNewNodePoint { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMainWindowViewModel mainWindow;

        private readonly ReactiveList<INodeViewModel> nodes;
        private readonly ReactiveList<IConnectionViewModel> connections;

        public MixerViewModel(IInteractionService interactions = null,
                              IMainWindowViewModel mainWindow = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mainWindow = mainWindow ?? Locator.Current.GetService<IMainWindowViewModel>();

            nodes = new ReactiveList<INodeViewModel>();
            connections = new ReactiveList<IConnectionViewModel>();

            HostScreen = this.mainWindow;
            Activator = new ViewModelActivator();
            GetNewNodePoint = new Interaction<Unit, Point?>();

            this.WhenActivated(disposables =>
            {
                this.interactions.DeleteNode.RegisterHandler(interaction =>
                {
                    var node = interaction.Input;

                    connections.RemoveRange(connections.Where(c => c.To == node ||
                                                                   c.From == node));
                    nodes.Remove(node);

                    interaction.SetOutput(Unit.Default);
                })
                .DisposeWith(disposables);

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
                            Color = Colors.SteelBlue
                        });
                    }
                })
                .DisposeWith(disposables);
            });
        }

        private void CreateData()
        {
            nodes.Add(new NodeViewModel
            {
                X = 10,
                Y = 10,
                Width = 150,
                Height = 150,
                Color = Colors.SteelBlue
            });

            nodes.Add(new NodeViewModel
            {
                X = 200,
                Y = 200,
                Width = 150,
                Height = 150,
                Color = Colors.SteelBlue
            });

            connections.Add(new ConnectionViewModel
            {
                From = nodes.First(),
                To = nodes.Last()
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IObservable<KeyEventArgs> MainWindowKeyDown => mainWindow.KeyDown;

        public IReadOnlyReactiveList<INodeViewModel> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;

        public ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; private set; }

        public Interaction<Unit, Point?> GetNewNodePoint { get; private set; }
    }
}