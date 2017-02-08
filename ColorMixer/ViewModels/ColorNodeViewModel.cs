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
        private readonly IMixerViewModel mixer;
        private readonly IOutConnectorViewModel output;

        public ColorNodeViewModel(IInteractionService interactions = null,
                                  IMixerViewModel mixer = null,
                                  IOutConnectorViewModel output = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();
            this.output = output ?? Locator.Current.GetService<IOutConnectorViewModel>();

            this.output.Node = this;

            this.WhenActivated(disposables =>
            {
                EditNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    Color = await this.interactions
                                      .GetNodeColor
                                      .Handle(Color);
                },
                this.WhenAnyValue(vm => vm.mixer.IsNodeBeingAdded, b => !b))
                .DisposeWith(disposables);
            });
        }

        public IOutConnectorViewModel Output => output;

        public ReactiveCommand<Unit, Unit> EditNodeCommand { get; private set; }
    }
}