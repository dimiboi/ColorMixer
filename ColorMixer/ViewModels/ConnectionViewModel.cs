using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        INodeViewModel From { get; set; }
        INodeViewModel To { get; set; }

        double X { get; }
        double Y { get; }
        double Width { get; }
        double Height { get; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private INodeViewModel from;
        private INodeViewModel to;

        private readonly ObservableAsPropertyHelper<double> x;
        private readonly ObservableAsPropertyHelper<double> y;
        private readonly ObservableAsPropertyHelper<double> width;
        private readonly ObservableAsPropertyHelper<double> height;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            x = this.WhenAnyValue(v => v.From.Connector.ConnectionPoint.X)
                    .ToProperty(this, v => v.X);

            y = this.WhenAnyValue(v => v.From.Connector.ConnectionPoint.Y)
                    .ToProperty(this, v => v.Y);

            width = this.WhenAnyValue(v => v.From.Connector.ConnectionPoint.X,
                                      v => v.To.Connector.ConnectionPoint.X,
                                      (from, to) => to - from)
                        .ToProperty(this, v => v.Width);

            height = this.WhenAnyValue(v => v.From.Connector.ConnectionPoint.Y,
                                      v => v.To.Connector.ConnectionPoint.Y,
                                      (from, to) => to - from)
                         .ToProperty(this, v => v.Height);

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public INodeViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public INodeViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }

        public double X => x.Value;

        public double Y => y.Value;

        public double Width => width.Value;

        public double Height => height.Value;
    }
}