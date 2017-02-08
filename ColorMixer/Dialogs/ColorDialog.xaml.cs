using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ColorMixer.Dialogs
{
    public partial class ColorDialog : CustomDialog, IActivatable
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color",
                                        typeof(Color),
                                        typeof(ColorDialog),
                                        new PropertyMetadata(Colors.Black));

        public ColorDialog()
        {
            InitializeComponent();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // Color -> ColorRectangle.Fill
                    .WhenAnyValue(v => v.Color)
                    .Select(c => new SolidColorBrush(c))
                    .BindTo(this, v => v.ColorRectangle.Fill)
                    .DisposeWith(disposables);

                this // Color.R -> RedSlider.Value
                    .WhenAnyValue(v => v.Color, c => c.R)
                    .BindTo(this, v => v.RedSlider.Value)
                    .DisposeWith(disposables);

                this // Color.G -> GreenSlider.Value
                    .WhenAnyValue(v => v.Color, c => c.G)
                    .BindTo(this, v => v.GreenSlider.Value)
                    .DisposeWith(disposables);

                this // Color.B -> BlueSlider.Value
                    .WhenAnyValue(v => v.Color, c => c.B)
                    .BindTo(this, v => v.BlueSlider.Value)
                    .DisposeWith(disposables);

                this // RedSlider.Value, GreenSlider.Value, BlueSlider.Value -> Color
                    .WhenAnyValue(v => v.RedSlider.Value,
                                  v => v.GreenSlider.Value,
                                  v => v.BlueSlider.Value,
                                  (r, g, b) => new { R = (byte)r, G = (byte)g, B = (byte)b })
                    .Select(c => new Color
                    {
                        R = c.R,
                        G = c.G,
                        B = c.B,
                        A = 255
                    })
                    .BindTo(this, v => v.Color)
                    .DisposeWith(disposables);
            });
        }

        public async Task WaitUntilClosed()
            => await DoneButton.Events()
                               .Click
                               .FirstAsync();

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}