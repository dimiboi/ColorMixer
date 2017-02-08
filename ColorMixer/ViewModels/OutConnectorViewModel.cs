using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IOutConnectorViewModel : IConnector
    {
        IInConnectorViewModel ConnectedTo { get; }
    }

    public class OutConnectorViewModel : Connector, IOutConnectorViewModel
    {
        private readonly IInteractionService interactions;

        private IInConnectorViewModel connectedTo;

        public OutConnectorViewModel(IInteractionService interactions = null)
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

        public override ConnectorDirection Direction => ConnectorDirection.Output;

        public override ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }

        public IInConnectorViewModel ConnectedTo
        {
            get { return connectedTo; }
            set { this.RaiseAndSetIfChanged(ref connectedTo, value); }
        }
    }
}