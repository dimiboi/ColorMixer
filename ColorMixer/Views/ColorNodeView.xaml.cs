using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorMixer.Views
{
    public partial class ColorNodeView : UserControl, IViewFor<IColorNodeViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IColorNodeViewModel),
                                        typeof(ColorNodeView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container",
                                        typeof(FrameworkElement),
                                        typeof(ColorNodeView),
                                        new PropertyMetadata(null));

        public ColorNodeView()
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

                this // ViewModel.Title -> TitleTextBlock.Text
                    .OneWayBind(ViewModel,
                        vm => vm.Title,
                        v => v.TitleTextBlock.Text)
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

                Thumb // Thumb.DragDelta.HorizontalChange -> ViewModel.X
                    .Events()
                    .DragDelta
                    .Select(e => ViewModel.X + e.HorizontalChange)
                    .BindTo(ViewModel, vm => vm.X)
                    .DisposeWith(disposables);

                Thumb // Thumb.DragDelta.VerticalChange -> ViewModel.Y
                    .Events()
                    .DragDelta
                    .Select(e => ViewModel.Y + e.VerticalChange)
                    .BindTo(ViewModel, vm => vm.Y)
                    .DisposeWith(disposables);

                this // ViewModel.DeleteNodeCommand -> DeleteNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.DeleteNodeCommand,
                        v => v.DeleteNodeButton.Command)
                    .DisposeWith(disposables);

                this // Clear Container when node is deleted
                    .WhenAnyValue(v => v.ViewModel) // when ViewModel is set
                    .SelectMany(vm => vm.DeleteNodeCommand) // subscribe to command execution
                    .Subscribe(_ => Container = null) // clear the container when node is deleted
                    .DisposeWith(disposables);
            });
        }

        public IColorNodeViewModel ViewModel
        {
            get { return (IColorNodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IColorNodeViewModel)value; }
        }

        public FrameworkElement Container
        {
            get { return (FrameworkElement)GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }
    }
}