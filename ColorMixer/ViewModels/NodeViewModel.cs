﻿using ColorMixer.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        IConnectorViewModel Connector { get; }

        ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
    }

    public class NodeViewModel : ReactiveObject, INodeViewModel
    {
        private readonly IInteractionService interactions;
        private readonly IConnectorViewModel connector;

        private double x;
        private double y;
        private double width;
        private double height;
        private Color color;

        private ObservableAsPropertyHelper<string> title;

        public NodeViewModel(IInteractionService interactions = null,
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