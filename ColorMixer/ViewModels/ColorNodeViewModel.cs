using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IColorNodeViewModel : INode
    {
        IConnectorViewModel Connector { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class ColorNodeViewModel : Node, IColorNodeViewModel
    {
        private readonly IConnectorViewModel connector;

        public ColorNodeViewModel(IConnectorViewModel connector = null)
        {
            this.connector = connector ?? Locator.Current.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                EditNodeCommand = ReactiveCommand.Create(() =>
                {
                })
                .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel Connector => connector;

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}