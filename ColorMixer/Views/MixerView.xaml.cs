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

        public static readonly DependencyProperty IsAddingNodeProperty =
            DependencyProperty.Register("IsAddingNode",
                                        typeof(bool),
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

                Nodes
                    .Events()
                    .MouseLeftButtonDown
                    //.Where(_ => IsAddingNode)
                    .Select(e => e.GetPosition(Nodes))
                    .InvokeCommand(ViewModel.AddColorNodeCommand)
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

                /*this // ViewModel.AddColorNodeCommand -> AddColorNodeButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.AddColorNodeCommand,
                        v => v.AddColorNodeButton.Command)
                    .DisposeWith(disposables);*/
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

        public bool IsAddingNode
        {
            get { return (bool)GetValue(IsAddingNodeProperty); }
            set { SetValue(IsAddingNodeProperty, value); }
        }
    }
}