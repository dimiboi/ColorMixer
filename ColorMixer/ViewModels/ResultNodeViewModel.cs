using ColorMixer.Model;
using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface IResultNodeViewModel : INode, IReactiveObject, ISupportsActivation
    {
        IConnectorViewModel Connector { get; }
    }

    public class ResultNodeViewModel : ReactiveObject, IColorNodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IConnectorViewModel connector;

        private double x;
        private double y;
        private double width;
        private double height;
        private Color color;

        private ObservableAsPropertyHelper<string> title;

        public ResultNodeViewModel(IInteractionService interactions = null,
                                   IConnectorViewModel connector = null)
        {
            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();
            this.connector = connector ?? Locator.Current.GetService<IConnectorViewModel>();

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                title = this // Color -> Title
                    .WhenAnyValue(vm => vm.Color,
                                  c => $"R: {c.R} / G: {c.G} / B {c.B}")
                    .ToProperty(this, vm => vm.Title)
                    .DisposeWith(disposables);

                DeleteNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    await this.interactions.DeleteNode.Handle(this);
                })
                .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public double X
        {
            get { return x; }
            set { this.RaiseAndSetIfChanged(ref x, value); }
        }

        public double Y
        {
            get { return y; }
            set { this.RaiseAndSetIfChanged(ref y, value); }
        }

        public double Width
        {
            get { return width; }
            set { this.RaiseAndSetIfChanged(ref width, value); }
        }

        public double Height
        {
            get { return height; }
            set { this.RaiseAndSetIfChanged(ref height, value); }
        }

        public Color Color
        {
            get { return color; }
            set { this.RaiseAndSetIfChanged(ref color, value); }
        }

        public string Title => title.Value;

        public IConnectorViewModel Connector => connector;

        public ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; private set; }
    }
}