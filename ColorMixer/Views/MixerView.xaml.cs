using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

                this // ViewModel.Nodes -> Nodes.ItemsSource
                    .OneWayBind(ViewModel,
                        vm => vm.Nodes,
                        v => v.Nodes.ItemsSource)
                    .DisposeWith(disposables);

                this // ViewModel.Connections -> Connections.ItemsSource
                    .OneWayBind(ViewModel,
                        vm => vm.Connections,
                        v => v.Connections.ItemsSource)
                    .DisposeWith(disposables);

                this // ViewModel.AddColorNodeCommand -> AddColorNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.AddColorNodeCommand,
                        v => v.AddColorNodeButton.Command)
                    .DisposeWith(disposables);

                ViewModel // Handle new node point request by ViewModel
                    .GetNewNodePoint
                    .RegisterHandler(async interation =>
                    {
                        var sequence = Observable.Merge(
                            Nodes.Events()
                                 .MouseLeftButtonDown // left button selects a point
                                 .Select(e => new Point?(e.GetPosition(Nodes))),
                            Nodes.Events()
                                 .MouseRightButtonDown // right button cancels selection
                                 .Select(e => default(Point?)));

                        var point = await sequence.FirstAsync();

                        interation.SetOutput(point);
                    })
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