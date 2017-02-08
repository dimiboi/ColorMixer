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
    public partial class OperationNodeView : UserControl, IViewFor<IOperationNodeViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IOperationNodeViewModel),
                                        typeof(OperationNodeView),
                                        new PropertyMetadata(null));

        public OperationNodeView()
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

                this // ViewModel.InputA -> InputAConnector.ViewModel
                    .WhenAnyValue(v => v.ViewModel.InputA)
                    .BindTo(this, v => v.InputAConnector.ViewModel)
                    .DisposeWith(disposables);

                this // ViewModel.InputB -> InputBConnector.ViewModel
                    .WhenAnyValue(v => v.ViewModel.InputB)
                    .BindTo(this, v => v.InputBConnector.ViewModel)
                    .DisposeWith(disposables);

                this // ViewModel.Output -> OutputConnector.ViewModel
                    .WhenAnyValue(v => v.ViewModel.Output)
                    .BindTo(this, v => v.OutputConnector.ViewModel)
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

                this // ViewModel.EditNodeCommand -> EditNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.EditNodeCommand,
                        v => v.EditNodeButton.Command)
                    .DisposeWith(disposables);

                this // ViewModel.DeleteNodeCommand -> DeleteNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.DeleteNodeCommand,
                        v => v.DeleteNodeButton.Command)
                    .DisposeWith(disposables);
            });
        }

        public IOperationNodeViewModel ViewModel
        {
            get { return (IOperationNodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IOperationNodeViewModel)value; }
        }
    }
}