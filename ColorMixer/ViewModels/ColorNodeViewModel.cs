using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ColorMixer.ViewModels
{
    public interface IColorNodeViewModel : INode
    {
        IConnectorViewModel Connector { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class ColorNodeViewModel : Node, IColorNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IConnectorViewModel connector;

        public ColorNodeViewModel(IInteractionService interactions = null,
                                  IConnectorViewModel connector = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.connector = connector ?? Locator.Current.GetService<IConnectorViewModel>();

            this.WhenActivated(disposables =>
            {
                EditNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    Color = await this.interactions
                                      .GetNodeColor
                                      .Handle(Color);
                })
                .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel Connector => connector;

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}