using ColorMixer.Extensions;
using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        IConnector ConnectingConnector { get; }
        IConnector ConnectedConnector { get; }
    }

    public class MixerViewModel : ReactiveObject, IMixerViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMainWindowViewModel mainWindow;

        private readonly ReactiveList<INode> nodes;
        private readonly ReactiveList<IConnectionViewModel> connections;

        private IConnector connectingConnector;
        private IConnector connectedConnector;

        public MixerViewModel(IInteractionService interactions = null,
                              IMainWindowViewModel mainWindow = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mainWindow = mainWindow ?? Locator.Current.GetService<IMainWindowViewModel>();

            HostScreen = this.mainWindow;

            nodes = new ReactiveList<INode>();
            connections = new ReactiveList<IConnectionViewModel>();

            this.WhenActivated(disposables =>
            {
                this
                    .interactions
                    .GetInConnector
                    .RegisterHandler(i => HandleConnectionRequest(i))
                    .DisposeWith(disposables);

                this
                    .interactions
                    .GetOutConnector
                    .RegisterHandler(i => HandleConnectionRequest(i))
                    .DisposeWith(disposables);

                this.interactions.DeleteNode.RegisterHandler(interaction =>
                {
                    var node = interaction.Input;
                    var connected = connections.Where(c => c.To.Node == node ||
                                                           c.From.Node == node);
                    foreach (var connection in connected)
                    {
                        connection.To.ConnectedTo = null;
                        connection.From.ConnectedTo = null;
                    }

                    connections.RemoveRange(connected);
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

                    var node = Locator.Current.GetService<IColorNodeViewModel>();

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

                    var node = Locator.Current.GetService<IResultNodeViewModel>();

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

                    var node = Locator.Current.GetService<IOperationNodeViewModel>();

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
                From = (nodes.First() as IColorNodeViewModel).Output,
                To = (nodes.Last() as IResultNodeViewModel).Input
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment => "Mixer";

        public IObservable<KeyEventArgs> MainWindowKeyDown => mainWindow.KeyDown;

        public IReadOnlyReactiveList<INode> Nodes => nodes;

        public IReadOnlyReactiveList<IConnectionViewModel> Connections => connections;

        public ReactiveCommand<Unit, Unit> AddColorNodeCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> AddOperationNodeCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> AddResultNodeCommand { get; private set; }

        public Interaction<Unit, Point?> GetNewNodePoint { get; }
            = new Interaction<Unit, Point?>();

        public IConnector ConnectingConnector
        {
            get { return connectingConnector; }
            private set { this.RaiseAndSetIfChanged(ref connectingConnector, value); }
        }

        public IConnector ConnectedConnector
        {
            get { return connectedConnector; }
            private set { this.RaiseAndSetIfChanged(ref connectedConnector, value); }
        }

        private async Task HandleConnectionRequest<TSrc, TDst>(
            InteractionContext<TSrc, TDst> interaction) where TDst : class, IConnector
                                                        where TSrc : class, IConnector
        {
            if (ConnectingConnector == null) // connection initiated
            {
                ConnectingConnector = interaction.Input;

                var secondConnector = await this.WhenAnyValue(vm => vm.ConnectedConnector)
                                                .Skip(1) // ignore initial value
                                                .FirstAsync();

                interaction.SetOutput(secondConnector as TDst);
            }
            else // the second connector
            {
                ConnectedConnector = interaction.Input;
                interaction.SetOutput(ConnectingConnector as TDst);

                var connection = Locator.Current.GetService<IConnectionViewModel>();

                if (ConnectingConnector.Direction == ConnectorDirection.Input &&
                    ConnectedConnector.Direction == ConnectorDirection.Output)
                {
                    connection.From = ConnectedConnector as IOutConnectorViewModel;
                    connection.To = ConnectingConnector as IInConnectorViewModel;
                }
                else if (ConnectingConnector.Direction == ConnectorDirection.Output &&
                         ConnectedConnector.Direction == ConnectorDirection.Input)
                {
                    connection.From = ConnectingConnector as IOutConnectorViewModel;
                    connection.To = ConnectedConnector as IInConnectorViewModel;
                }

                if (connection.From != null &&
                    connection.To != null)
                {
                    connections.Add(connection);
                }

                ConnectingConnector = null;
                ConnectedConnector = null;
            }
        }
    }
}