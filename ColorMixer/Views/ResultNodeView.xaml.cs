using ColorMixer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorMixer.Views
{
    public partial class ResultNodeView : UserControl, IViewFor<IResultNodeViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IResultNodeViewModel),
                                        typeof(ResultNodeView),
                                        new PropertyMetadata(null));

        private readonly IMixerViewModel mixer;

        public ResultNodeView() : this(null)
        {
        }

        public ResultNodeView(IMixerViewModel mixer = null)
        {
            InitializeComponent();

            this.mixer = mixer ?? Locator.Current.GetService<IMixerViewModel>();

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

                this // ViewModel.Connector -> Connector.ViewModel
                    .WhenAnyValue(v => v.ViewModel.Input)
                    .BindTo(this, v => v.InputConnector.ViewModel)
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

                this // Make Thumb click-through when a node is being added
                    .WhenAnyValue(v => v.mixer.IsNodeBeingAdded, b => !b)
                    .BindTo(this, v => v.Thumb.IsHitTestVisible)
                    .DisposeWith(disposables);
            });
        }

        public IResultNodeViewModel ViewModel
        {
            get { return (IResultNodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IResultNodeViewModel)value; }
        }
    }
}