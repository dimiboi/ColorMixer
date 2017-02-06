using ColorMixer.ViewModels;
using Microsoft.Expression.Media;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace ColorMixer.Views
{
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

                this // Sets the margin of the arrow box
                    .WhenAnyValue(v => v.ViewModel.From.ConnectionPoint,
                                  v => v.ViewModel.To.ConnectionPoint,
                                  (from, to) => new Thickness
                                  {
                                      Left = Math.Min(from.X, to.X),
                                      Top = Math.Min(from.Y, to.Y),
                                      Right = Arrow.Margin.Right,
                                      Bottom = Arrow.Margin.Bottom
                                  })
                    .BindTo(this, v => v.Arrow.Margin)
                    .DisposeWith(disposables);

                this // Sets the bend amount of the arrow
                    .WhenAnyValue(v => v.ViewModel.From.ConnectionPoint,
                                  v => v.ViewModel.To.ConnectionPoint,
                                  (from, to) => from.X > to.X
                                                ? from.Y > to.Y
                                                  ? -0.5
                                                  : 0.5
                                                : from.Y > to.Y
                                                  ? 0.5
                                                  : -0.5)
                    .BindTo(this, v => v.Arrow.BendAmount)
                    .DisposeWith(disposables);

                this // Sets the start corner of the arrow
                    .WhenAnyValue(v => v.ViewModel.From.ConnectionPoint,
                                  v => v.ViewModel.To.ConnectionPoint,
                                  (from, to) => from.X > to.X
                                                ? from.Y > to.Y
                                                  ? CornerType.BottomRight
                                                  : CornerType.TopRight
                                                : from.Y > to.Y
                                                  ? CornerType.BottomLeft
                                                  : CornerType.TopLeft)
                    .BindTo(this, v => v.Arrow.StartCorner)
                    .DisposeWith(disposables);

                this // Sets the width of the arrow box
                    .WhenAnyValue(v => v.ViewModel.From.ConnectionPoint,
                                  v => v.ViewModel.To.ConnectionPoint,
                                  (from, to) => Math.Abs(to.X - from.X))
                    .BindTo(this, v => v.Arrow.Width)
                    .DisposeWith(disposables);

                this // Sets the height of the arrow box
                    .WhenAnyValue(v => v.ViewModel.From.ConnectionPoint,
                                  v => v.ViewModel.To.ConnectionPoint,
                                  (from, to) => Math.Abs(to.Y - from.Y))
                    .BindTo(this, v => v.Arrow.Height)
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