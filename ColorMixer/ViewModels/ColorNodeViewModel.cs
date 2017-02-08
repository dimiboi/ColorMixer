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
        IOutConnectorViewModel Output { get; }
        ReactiveCommand<Unit, Unit> EditNodeCommand { get; }
    }

    public class ColorNodeViewModel : Node, IColorNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IOutConnectorViewModel output;

        public ColorNodeViewModel(IInteractionService interactions = null,
                                  IOutConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.output = output ?? Locator.Current.GetService<IOutConnectorViewModel>();

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

        public IOutConnectorViewModel Output => output;

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}