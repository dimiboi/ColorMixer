using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorMixer.Views
{
    public partial class NodeView : UserControl, IViewFor<INodeViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(INodeViewModel),
                                        typeof(NodeView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container",
                                        typeof(FrameworkElement),
                                        typeof(NodeView),
                                        new PropertyMetadata(null));

        public NodeView()
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

                this // ViewModel.Width <-> Thumb.Width
                    .Bind(ViewModel,
                        vm => vm.Width,
                        v => v.Thumb.Width)
                    .DisposeWith(disposables);

                this // ViewModel.Height <-> Thumb.Height
                    .Bind(ViewModel,
                        vm => vm.Height,
                        v => v.Thumb.Height)
                    .DisposeWith(disposables);

                this // Container -> Connector.Container
                    .WhenAnyValue(v => v.Container)
                    .BindTo(this, v => v.Connector.Container)
                    .DisposeWith(disposables);

                this // ViewModel.Connector -> Connector.ViewModel
                    .WhenAnyValue(v => v.ViewModel.Connector)
                    .BindTo(this, v => v.Connector.ViewModel)
                    .DisposeWith(disposables);

                Thumb // Thumb.DragDelta -> OnThumbDragDelta()
                    .Events()
                    .DragDelta
                    .Subscribe(e => OnThumbDragDelta(e))
                    .DisposeWith(disposables);

                this // Container.ActualWidth -> OnContainerWidthChanged()
                    .WhenAnyValue(v => v.Container.ActualWidth)
                    .Subscribe(w => OnContainerWidthChanged(w))
                    .DisposeWith(disposables);

                this // Container.Height -> OnContainerHeightChanged()
                    .WhenAnyValue(v => v.Container.ActualHeight)
                    .Subscribe(h => OnContainerHeightChanged(h))
                    .DisposeWith(disposables);
            });
        }

        public INodeViewModel ViewModel
        {
            get { return (INodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (INodeViewModel)value; }
        }

        public FrameworkElement Container
        {
            get { return (FrameworkElement)GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            var x = ViewModel.X + e.HorizontalChange;
            var y = ViewModel.Y + e.VerticalChange;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            x = x + ViewModel.Width > Container.ActualWidth
                    ? Container.ActualWidth - ViewModel.Width
                    : x;

            y = y + ViewModel.Height > Container.ActualHeight
                    ? Container.ActualHeight - ViewModel.Height
                    : y;

            ViewModel.X = x;
            ViewModel.Y = y;
        }

        private void OnContainerWidthChanged(double containerWidth)
        {
            ViewModel.X = ViewModel.X + ViewModel.Width > containerWidth
                          ? containerWidth - ViewModel.Width
                          : ViewModel.X;
        }

        private void OnContainerHeightChanged(double containerHeight)
        {
            ViewModel.Y = ViewModel.Y + ViewModel.Width > containerHeight
                          ? containerHeight - ViewModel.Height
                          : ViewModel.Y;
        }
    }
}