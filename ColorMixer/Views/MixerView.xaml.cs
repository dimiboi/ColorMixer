using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace ColorMixer.Views
{
    public partial class MixerView : UserControl, IViewFor<IMixerViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IMixerViewModel),
                                        typeof(MixerView),
                                        new PropertyMetadata(null));

        public MixerView()
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

                this // ViewModel.Rectangles -> Rectangles.ItemsSource
                    .OneWayBind(ViewModel,
                        vm => vm.Rectangles,
                        v => v.Rectangles.ItemsSource)
                    .DisposeWith(disposables);

                this // ViewModel.Rectangles -> Rectangles.ItemsSource
                    .OneWayBind(ViewModel,
                        vm => vm.Connections,
                        v => v.Connections.ItemsSource)
                    .DisposeWith(disposables);
            });
        }

        public IMixerViewModel ViewModel
        {
            get { return (IMixerViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IMixerViewModel)value; }
        }
    }
}