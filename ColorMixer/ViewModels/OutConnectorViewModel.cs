using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ColorMixer.ViewModels
{
    public interface IOutConnectorViewModel : IConnector
    {
        IInConnectorViewModel ConnectedTo { get; set; }
    }

    public class OutConnectorViewModel : Connector, IOutConnectorViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;

        private ObservableAsPropertyHelper<bool> isEnabled;
        private ObservableAsPropertyHelper<bool> isConnected;
        private IInConnectorViewModel connectedTo;

        public OutConnectorViewModel(IInteractionService interactions = null,
                                     IMixerViewModel mixer = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();

            this.WhenActivated(disposables =>
            {
                isEnabled = this // Disable when not connectable to
                    .WhenAnyValue(vm => vm.mixer.ConnectingConnector)
                    .Select(c => c == null || (c.Direction != Direction && c.Node != Node))
                    .ToProperty(this, vm => vm.IsEnabled)
                    .DisposeWith(disposables);

                isConnected = this // ConectedTo -> IsConnected
                    .WhenAnyValue(vm => vm.ConnectedTo)
                    .Select(ct => ct != null)
                    .ToProperty(this, vm => vm.IsConnected);

                ConnectorCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    ConnectedTo = await this.interactions
                                            .GetInConnector
                                            .Handle(this);
                })
                .DisposeWith(disposables);
            });
        }

        public override bool IsConnected => isConnected.Value;

        public override bool IsEnabled => isEnabled.Value;

        public override ConnectorDirection Direction => ConnectorDirection.Output;

        public override ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }

        public IInConnectorViewModel ConnectedTo
        {
            get { return connectedTo; }
            set { this.RaiseAndSetIfChanged(ref connectedTo, value); }
        }
    }
}