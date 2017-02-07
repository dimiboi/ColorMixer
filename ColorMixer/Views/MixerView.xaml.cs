using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

                this // ViewModel.AddResultNodeCommand -> AddResultNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.AddResultNodeCommand,
                        v => v.AddResultNodeButton.Command)
                    .DisposeWith(disposables);

                ViewModel // Handle new node point request by ViewModel
                    .GetNewNodePoint
                    .RegisterHandler(async interaction =>
                    {
                        try
                        {
                            Nodes.Cursor = Cursors.Cross;

                            var sequence = Observable.Merge(
                                Nodes.Events()
                                     .MouseLeftButtonDown // left button picks a point
                                     .Select(e => new Point?(e.GetPosition(Nodes))),
                                Nodes.Events()
                                     .MouseRightButtonDown // right button cancels addition
                                     .Select(e => default(Point?)),
                                ViewModel.MainWindowKeyDown
                                         .Where(e => e.Key == Key.Escape) // Esc cancels addition
                                         .Select(e => default(Point?)));

                            var point = await sequence.FirstAsync(); // whichever comes first

                            interaction.SetOutput(point);
                        }
                        finally
                        {
                            Nodes.Cursor = Cursors.Arrow;
                        }
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