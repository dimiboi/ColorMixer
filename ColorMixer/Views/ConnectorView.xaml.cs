using ColorMixer.Model;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorMixer.Views
{
    public partial class ConnectorView : UserControl, IViewFor<IConnector>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IConnector),
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

                this // ViewModel.IsEnabled -> IsEnabled
                    .OneWayBind(ViewModel,
                        vm => vm.IsEnabled,
                        v => v.IsEnabled)
                    .DisposeWith(disposables);

                this // ViewModel.IsConnected -> ConnectorButton.Content
                    .WhenAnyValue(v => v.ViewModel.IsConnected)
                    .Select(ic => ic ? "\xECCC" : string.Empty)
                    .BindTo(this, v => v.ConnectorButton.Content)
                    .DisposeWith(disposables);

                this // ViewModel.ConnectorCommand -> ConnectorButton.Command
                    .OneWayBind(ViewModel,
                        vm => vm.ConnectorCommand,
                        v => v.ConnectorButton.Command)
                    .DisposeWith(disposables);

                this // Transform coordinates of the connector to the coordinates of the container
                    .Events()
                    .LayoutUpdated
                    .Where(_ => Container != null) // while Container is set
                    .Where(_ => Container.IsAncestorOf(this)) // while it's still the ancestor
                    .Select(_ => this.TransformToAncestor(Container) // get transform to Container
                                     .Transform(new Point(ActualWidth / 2, // transform the center
                                                          ActualHeight / 2)))
                    .BindTo(this, v => v.ConnectionPoint) // set ConnectionPoint
                    .DisposeWith(disposables);

                this // ViewModel.ConnectionPoint <-> ConnectionPoint
                    .Bind(ViewModel,
                        vm => vm.ConnectionPoint,
                        v => v.ConnectionPoint)
                    .DisposeWith(disposables);
            });
        }

        public IConnector ViewModel
        {
            get { return (IConnector)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IConnector)value; }
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