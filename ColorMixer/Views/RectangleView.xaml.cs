using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorMixer.Views
{
    public partial class RectangleView : UserControl, IViewFor<IRectangleViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IRectangleViewModel),
                                        typeof(RectangleView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerWidthProperty =
            DependencyProperty.Register("ContainerWidth",
                                        typeof(double),
                                        typeof(RectangleView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerHeightProperty =
            DependencyProperty.Register("ContainerHeight",
                                        typeof(double),
                                        typeof(RectangleView),
                                        new PropertyMetadata(null));

        public RectangleView()
        {
            InitializeComponent();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // ViewModel -> DataContext
                    .WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                Thumb // Thumb.DragDelta -> OnThumbDragDelta()
                    .Events()
                    .DragDelta
                    .Subscribe(e => OnThumbDragDelta(e))
                    .DisposeWith(disposables);
            });
        }

        public double ContainerWidth
        {
            get { return (double)GetValue(ContainerWidthProperty); }
            set { SetValue(ContainerWidthProperty, value); }
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(ContainerHeightProperty); }
            set { SetValue(ContainerHeightProperty, value); }
        }

        public IRectangleViewModel ViewModel
        {
            get { return (IRectangleViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IRectangleViewModel)value; }
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            var x = ViewModel.X + e.HorizontalChange;
            var y = ViewModel.Y + e.VerticalChange;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            x = x + ViewModel.Width > ContainerWidth ? ContainerWidth - ViewModel.Width : x;
            y = y + ViewModel.Height > ContainerHeight ? ContainerHeight - ViewModel.Height : y;

            ViewModel.X = x;
            ViewModel.Y = y;
        }
    }
}