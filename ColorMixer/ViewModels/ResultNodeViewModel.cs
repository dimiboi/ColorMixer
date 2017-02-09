using ColorMixer.Model;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IResultNodeViewModel : INode
    {
        IInConnectorViewModel Input { get; }
    }

    public class ResultNodeViewModel : Node, IResultNodeViewModel
    {
        private readonly IMixerViewModel mixer;
        private readonly IInConnectorViewModel input;

        public ResultNodeViewModel(IMixerViewModel mixer = null,
                                   IInConnectorViewModel input = null)
        {
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();
            this.input = input ?? Locator.Current.GetService<IInConnectorViewModel>();

            this.input.Node = this;

            this.WhenActivated(disposables =>
            {
                this // Handle the connection
                    .WhenAnyValue(vm => vm.Input.ConnectedTo)
                    .Select(i => i != null
                                 ? i.Node.Color
                                 : Colors.Black)
                    .BindTo(this, vm => vm.Color)
                    .DisposeWith(disposables);

                this // Handle the color
                    .WhenAnyValue(vm => vm.Input.ConnectedTo.Node.Color)
                    .Where(_ => Input?.ConnectedTo?.Node != null) // when a node is connected
                    .BindTo(this, vm => vm.Color)
                    .DisposeWith(disposables);
            });
        }

        public IInConnectorViewModel Input => input;
    }
}