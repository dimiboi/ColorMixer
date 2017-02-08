using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ColorMixer.ViewModels
{
    public interface IInConnectorViewModel : IConnector
    {
        IOutConnectorViewModel ConnectedTo { get; }
    }

    public class InConnectorViewModel : Connector, IInConnectorViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;

        private ObservableAsPropertyHelper<bool> isEnabled;
        private IOutConnectorViewModel connectedTo;

        public InConnectorViewModel(IInteractionService interactions = null,
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

                ConnectorCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    ConnectedTo = await this.interactions
                                            .GetOutConnector
                                            .Handle(this);
                })
                .DisposeWith(disposables);
            });
        }

        public override bool IsEnabled => isEnabled.Value;

        public override ConnectorDirection Direction => ConnectorDirection.Input;

        public override ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }

        public IOutConnectorViewModel ConnectedTo
        {
            get { return connectedTo; }
            private set { this.RaiseAndSetIfChanged(ref connectedTo, value); }
        }
    }
}