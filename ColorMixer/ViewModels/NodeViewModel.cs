using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface INodeViewModel : IReactiveObject, ISupportsActivation
    {
        string Title { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Color Color { get; set; }

        IMixerViewModel Mixer { get; }
        IConnectorViewModel Connector { get; }

        ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
    }

    public class NodeViewModel : ReactiveObject, INodeViewModel
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private Color color;

        private readonly IMixerViewModel mixer;
        private readonly IConnectorViewModel connector;

        private ObservableAsPropertyHelper<string> title;

        public NodeViewModel(IMixerViewModel mixer = null,
                             IConnectorViewModel connector = null)
        {
            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();
            this.connector = connector ?? Locator.Current.GetService<IConnectorViewModel>();

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                title = this
                    .WhenAnyValue(vm => vm.Color,
                                  c => $"R: {c.R} / G: {c.G} / B {c.B}")
                    .ToProperty(this, vm => vm.Title)
                    .DisposeWith(disposables);

                DeleteNodeCommand = ReactiveCommand.Create(() =>
                {
                    Mixer
                        .DeleteNodeCommand
                        .Execute(this);
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

        public IMixerViewModel Mixer => mixer;

        public IConnectorViewModel Connector => connector;

        public ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; private set; }
    }
}