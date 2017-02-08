using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IInConnectorViewModel : IConnector
    {
        IOutConnectorViewModel ConnectedTo { get; }
    }

    public class InConnectorViewModel : Connector, IInConnectorViewModel
    {
        private readonly IInteractionService interactions;

        private IOutConnectorViewModel connectedTo;

        public InConnectorViewModel(IInteractionService interactions = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();

            this.WhenActivated(disposables =>
            {
                ConnectorCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                })
                .DisposeWith(disposables);
            });
        }

        public override ConnectorDirection Direction => ConnectorDirection.Input;

        public override ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }

        public IOutConnectorViewModel ConnectedTo
        {
            get { return connectedTo; }
            set { this.RaiseAndSetIfChanged(ref connectedTo, value); }
        }
    }
}