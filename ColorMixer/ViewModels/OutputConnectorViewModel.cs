using ColorMixer.Model;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IOutputConnectorViewModel : IConnector
    {
    }

    public class OutputConnectorViewModel : Connector, IOutputConnectorViewModel
    {
        public OutputConnectorViewModel()
        {
            this.WhenActivated(disposables =>
            {
                Disposable.Empty
                          .DisposeWith(disposables);
            });
        }

        public override ConnectorDirection Direction => ConnectorDirection.Output;
    }
}