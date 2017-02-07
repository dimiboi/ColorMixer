using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorMixer.Model
{
    public interface INode : IReactiveObject, ISupportsActivation
    {
        string Title { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Color Color { get; set; }
        ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
    }

    public abstract class Node : ReactiveObject, INode
    {
        private ObservableAsPropertyHelper<string> title;
        private double x;
        private double y;
        private double width;
        private double height;
        private Color color;

        public Node(IDependencyResolver resolver = null,
                    IInteractionService interactions = null)
        {
            resolver = resolver ?? Locator.Current;
            interactions = interactions ?? resolver.GetService<IInteractionService>();

            this.WhenActivated(disposables =>
            {
                title = this // Color -> Title
                    .WhenAnyValue(vm => vm.Color,
                                  c => $"R: {c.R} / G: {c.G} / B {c.B}")
                    .ToProperty(this, vm => vm.Title)
                    .DisposeWith(disposables);

                DeleteNodeCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    await interactions.DeleteNode.Handle(this);
                })
                .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public string Title => title.Value;

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

        public ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; private set; }
    }
}