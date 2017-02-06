using MahApps.Metro.Controls;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace ColorMixer
{
    public partial class MainWindow : MetroWindow, IViewFor<IMainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IMainWindowViewModel),
                                        typeof(MainWindow),
                                        new PropertyMetadata(null));

        public MainWindow()
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

                this // KeyDown -> ViewModel.KeyDown
                    .WhenAnyValue(v => v.ViewModel)
                    .Select(_ => this.Events().KeyDown)
                    .BindTo(ViewModel, vm => vm.KeyDown)
                    .DisposeWith(disposables);
            });

            ViewModel = new MainWindowViewModel();
        }

        public IMainWindowViewModel ViewModel
        {
            get { return (IMainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IMainWindowViewModel)value; }
        }
    }
}