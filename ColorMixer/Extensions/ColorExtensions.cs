using System.Windows.Media;

namespace ColorMixer.Extensions
{
    public static class ColorExtensions
    {
        public static Color Add(this Color color, Color value)
            => new Color
            {
                R = color.R.Add(value.R),
                G = color.G.Add(value.G),
                B = color.B.Add(value.B),
                A = color.A.Add(value.A)
            };

        public static Color Subtract(this Color color, Color value)
            => new Color
            {
                R = color.R.Subtract(value.R),
                G = color.G.Subtract(value.G),
                B = color.B.Subtract(value.B),
                A = color.A.Subtract(value.A)
            };

        public static byte Add(this byte b, byte value)
            => (byte)(b + value < byte.MaxValue ? b + value : byte.MaxValue);

        public static byte Subtract(this byte b, byte value)
            => (byte)(b - value > byte.MinValue ? b - value : byte.MinValue);
    }
}