using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorMixer.Views
{
    public partial class ConnectorView : UserControl, IViewFor<IConnectorViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IConnectorViewModel),
                                        typeof(ConnectorView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ConnectionPointProperty =
            DependencyProperty.Register("ConnectionPoint",
                                        typeof(Point),
                                        typeof(ConnectorView),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container",
                                        typeof(FrameworkElement),
                                        typeof(ConnectorView),
                                        new PropertyMetadata(null));

        public ConnectorView()
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

                this // ViewModel.ConnectionPoint <-> ConnectionPoint
                    .Bind(ViewModel,
                        vm => vm.ConnectionPoint,
                        v => v.ConnectionPoint)
                    .DisposeWith(disposables);

                this // LayoutUpdated -> ConnectionPoint
                    .Events()
                    .LayoutUpdated
                    .Where(_ => Container != null)
                    .Select(_ => this.TransformToAncestor(Container)
                                     .Transform(new Point(ActualWidth / 2, ActualHeight / 2)))
                    .BindTo(this, v => v.ConnectionPoint)
                    .DisposeWith(disposables);
            });
        }

        public IConnectorViewModel ViewModel
        {
            get { return (IConnectorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IConnectorViewModel)value; }
        }

        public Point ConnectionPoint
        {
            get { return (Point)GetValue(ConnectionPointProperty); }
            set { SetValue(ConnectionPointProperty, value); }
        }

        public FrameworkElement Container
        {
            get { return (FrameworkElement)GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }
    }
}