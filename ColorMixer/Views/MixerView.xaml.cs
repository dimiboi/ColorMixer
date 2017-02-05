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

                this // IsAddingNode -> Nodes.Cursor
                    .WhenAnyValue(v => v.ViewModel.IsAddingNode,
                                  v => v ? Cursors.Cross : Cursors.Arrow)
                    .BindTo(this, v => v.Nodes.Cursor)
                    .DisposeWith(disposables);

                this // IsAddingNode -> Nodes.Opacity
                    .WhenAnyValue(v => v.ViewModel.IsAddingNode,
                                  v => v ? 0.5 : 1)
                    .BindTo(this, v => v.Nodes.Opacity)
                    .DisposeWith(disposables);

                this // ViewModel.AddColorNodeCommand -> StartAddingColorNodeCommand.Command
                    .OneWayBind(ViewModel,
                        vm => vm.StartAddingColorNodeCommand,
                        v => v.AddColorNodeButton.Command)
                    .DisposeWith(disposables);

                Nodes // Invoke EndAddingColorNodeCommand when Nodes canvas clicked in adding mode
                    .Events()
                    .MouseLeftButtonDown
                    .Where(_ => ViewModel.IsAddingNode)
                    .Select(e => e.GetPosition(Nodes))
                    .InvokeCommand(ViewModel.EndAddingColorNodeCommand)
                    .DisposeWith(disposables);

                Nodes // Cancel adding on right click on Nodes canvas
                    .Events()
                    .MouseRightButtonDown
                    .Where(_ => ViewModel.IsAddingNode)
                    .Select(_ => false)
                    .BindTo(ViewModel, vm => vm.IsAddingNode)
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