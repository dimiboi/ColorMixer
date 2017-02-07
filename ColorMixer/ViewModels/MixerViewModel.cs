using ColorMixer.Model;
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
        IReadOnlyReactiveList<INode> Nodes { get; }
        IReadOnlyReactiveList<IConnectionViewModel> Connections { get; }
        ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; }
        ReactiveCommand<Unit, Unit> AddOperationNodeCommand { get; }
        ReactiveCommand<Unit, Unit> AddResultNodeCommand { get; }
        Interaction<Unit, Point?> GetNewNodePoint { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMainWindowViewModel mainWindow;

        private readonly ReactiveList<INode> nodes;
        private readonly ReactiveList<IConnectionViewModel> connections;

        public MixerViewModel(IDependencyResolver resolver = null,
                              IInteractionService interactions = null,
                              IMainWindowViewModel mainWindow = null)
        {
            resolver = resolver ?? Locator.Current;

            this.interactions = interactions ?? resolver.GetService<IInteractionService>();
            this.mainWindow = mainWindow ?? resolver.GetService<IMainWindowViewModel>();

            nodes = new ReactiveList<INode>();
            connections = new ReactiveList<IConnectionViewModel>();

            HostScreen = this.mainWindow;
            Activator = new ViewModelActivator();
            GetNewNodePoint = new Interaction<Unit, Point?>();

            this.WhenActivated(disposables =>
            {
                this.interactions.DeleteNode.RegisterHandler(interaction =>
                {
                    var node = interaction.Input;

                    /*connections.RemoveRange(connections.Where(c => c.To == node.Connector ||
                                                                   c.From == node.Connector));*/
                    nodes.Remove(node);

                    interaction.SetOutput(Unit.Default);
                })
                .DisposeWith(disposables);

                AddColorNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var point = await GetNewNodePoint.Handle(Unit.Default);

                    if (!point.HasValue) // user cancelled point selection
                    {
                        return;
                    }

                    var node = resolver.GetService<IColorNodeViewModel>();

                    node.X = point.Value.X;
                    node.Y = point.Value.Y;

                    nodes.Add(node);
                })
                .DisposeWith(disposables);

                AddResultNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var point = await GetNewNodePoint.Handle(Unit.Default);

                    if (!point.HasValue) // user cancelled point selection
                    {
                        return;
                    }

                    var node = resolver.GetService<IResultNodeViewModel>();

                    node.X = point.Value.X;
                    node.Y = point.Value.Y;

                    nodes.Add(node);
                })
                .DisposeWith(disposables);

                AddOperationNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var point = await GetNewNodePoint.Handle(Unit.Default);

                    if (!point.HasValue) // user cancelled point selection
                    {
                        return;
                    }

                    var node = resolver.GetService<IOperationNodeViewModel>();

                    node.X = point.Value.X;
                    node.Y = point.Value.Y;

                    nodes.Add(node);
                })
                .DisposeWith(disposables);
            });
        }

        private void CreateData()
        {
            nodes.Add(new ColorNodeViewModel
            {
                X = 10,
                Y = 10,
                Width = 150,
                Height = 150,
                Color = Colors.SteelBlue
            });

            nodes.Add(new ResultNodeViewModel
            {
                X = 200,
                Y = 200,
                Width = 150,
                Height = 150,
                Color = Colors.SteelBlue
            });

            connections.Add(new ConnectionViewModel
            {
                From = (nodes.First() as IColorNodeViewModel).Connector,
                To = (nodes.Last() as IResultNodeViewModel).Connector
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IObservable<KeyEventArgs> MainWindowKeyDown => mainWindow.KeyDown;

        public IReadOnlyReactiveList<INode> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;

        public ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> AddOperationNodeCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> AddResultNodeCommand { get; private set; }

        public Interaction<Unit, Point?> GetNewNodePoint { get; private set; }
    }
}