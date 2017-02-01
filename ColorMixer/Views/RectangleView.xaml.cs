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

                Thumb
                    .Events()
                    .DragDelta
                    .InvokeCommand(this, v => v.ViewModel.DragDelta)
                    .DisposeWith(disposables);
            });
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
    }
}