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
        IOutputConnectorViewModel Output { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class ColorNodeViewModel : Node, IColorNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IOutputConnectorViewModel output;

        public ColorNodeViewModel(IInteractionService interactions = null,
                                  IOutputConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.output = output ?? Locator.Current.GetService<IOutputConnectorViewModel>();

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

        public IOutputConnectorViewModel Output => output;

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}