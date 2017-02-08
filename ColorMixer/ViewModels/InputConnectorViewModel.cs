using ColorMixer.Model;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IInputConnectorViewModel : IConnector
    {
    }

    public class InputConnectorViewModel : Connector, IInputConnectorViewModel
    {
        public InputConnectorViewModel()
        {
            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public override ConnectorDirection Direction => ConnectorDirection.Input;
    }
}