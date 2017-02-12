using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace ColorMixer.Views
{
    [ExcludeFromCodeCoverage]
    public partial class ConnectionView : UserControl, IViewFor<IConnectionViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IConnectionViewModel),
                                        typeof(ConnectionView),
                                        new PropertyMetadata(null));

        public ConnectionView()
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

                this // ViewModel.From.ConnectionPoint -> Arrow.From
                    .Bind(ViewModel,
                        vm => vm.From.ConnectionPoint,
                        v => v.Arrow.From)
                    .DisposeWith(disposables);

                this // ViewModel.To.ConnectionPoint -> Arrow.To
                    .Bind(ViewModel,
                        vm => vm.To.ConnectionPoint,
                        v => v.Arrow.To)
                    .DisposeWith(disposables);
            });
        }

        public IConnectionViewModel ViewModel
        {
            get { return (IConnectionViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IConnectionViewModel)value; }
        }
    }
}