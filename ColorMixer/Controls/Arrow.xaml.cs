using Microsoft.Expression.Media;
using ReactiveUI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace ColorMixer.Controls
{
    [ExcludeFromCodeCoverage]
    public partial class Arrow : UserControl, IActivatable
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From",
                                        typeof(Point),
                                        typeof(Arrow),
                                        new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To",
                                        typeof(Point),
                                        typeof(Arrow),
                                        new PropertyMetadata(default(Point)));

        public Arrow()
        {
            InitializeComponent();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // Sets the margin of the arrow box
                    .WhenAnyValue(v => v.From,
                                  v => v.To,
                                  (from, to) => new Thickness
                                  {
                                      Left = Math.Min(from.X, to.X),
                                      Top = Math.Min(from.Y, to.Y),
                                      Right = LineArrow.Margin.Right,
                                      Bottom = LineArrow.Margin.Bottom
                                  })
                    .BindTo(this, v => v.LineArrow.Margin)
                    .DisposeWith(disposables);

                this // Sets the bend amount of the arrow
                    .WhenAnyValue(v => v.From,
                                  v => v.To,
                                  (from, to) => from.X > to.X
                                                ? from.Y > to.Y
                                                  ? -0.5
                                                  : 0.5
                                                : from.Y > to.Y
                                                  ? 0.5
                                                  : -0.5)
                    .BindTo(this, v => v.LineArrow.BendAmount)
                    .DisposeWith(disposables);

                this // Sets the start corner of the arrow
                    .WhenAnyValue(v => v.From,
                                  v => v.To,
                                  (from, to) => from.X > to.X
                                                ? from.Y > to.Y
                                                  ? CornerType.BottomRight
                                                  : CornerType.TopRight
                                                : from.Y > to.Y
                                                  ? CornerType.BottomLeft
                                                  : CornerType.TopLeft)
                    .BindTo(this, v => v.LineArrow.StartCorner)
                    .DisposeWith(disposables);

                this // Sets the width of the arrow box
                    .WhenAnyValue(v => v.From,
                                  v => v.To,
                                  (from, to) => Math.Abs(to.X - from.X))
                    .BindTo(this, v => v.LineArrow.Width)
                    .DisposeWith(disposables);

                this // Sets the height of the arrow box
                    .WhenAnyValue(v => v.From,
                                  v => v.To,
                                  (from, to) => Math.Abs(to.Y - from.Y))
                    .BindTo(this, v => v.LineArrow.Height)
                    .DisposeWith(disposables);
            });
        }

        public Point From
        {
            get { return (Point)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public Point To
        {
            get { return (Point)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
    }
}