using ColorMixer.Converters;
using FluentAssertions;
using Ploeh.AutoFixture;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Xunit;

namespace Converters
{
    public class ColorToBrush
    {
        private static readonly IFixture fixture;

        private readonly IValueConverter converter;

        static ColorToBrush()
        {
            fixture = new Fixture();
        }

        public ColorToBrush()
        {
            converter = new ColorToBrushConverter(); // system under test
        }

        [Fact]
        public void Converts()
        {
            // Arrange

            var color = Color.FromRgb(fixture.Create<byte>(),
                                      fixture.Create<byte>(),
                                      fixture.Create<byte>());
            // Act

            var brush = (SolidColorBrush)converter.Convert(color, typeof(SolidColorBrush),
                                                           null, CultureInfo.CurrentCulture);
            // Assert

            brush.Color.Should().Be(color);
        }

        [Fact]
        public void ConvertsBack()
        {
            // Arrange

            var brush = new SolidColorBrush();

            brush.Color = Color.FromRgb(fixture.Create<byte>(),
                                        fixture.Create<byte>(),
                                        fixture.Create<byte>());
            // Act

            var color = (Color)converter.ConvertBack(brush, typeof(Color),
                                                     null, CultureInfo.CurrentCulture);
            // Assert

            color.Should().Be(brush.Color);
        }
    }
}