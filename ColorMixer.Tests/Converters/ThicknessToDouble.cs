using ColorMixer.Converters;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Xunit;

namespace Converters
{
    public class ThicknessToDouble
    {
        private static readonly IFixture fixture;

        private readonly IValueConverter converter;

        static ThicknessToDouble()
        {
            fixture = new Fixture();
        }

        public ThicknessToDouble()
        {
            converter = new ThicknessToDoubleConverter();
        }

        [Theory]
        [AutoData]
        public void Converts(double expected)
        {
            // Arrange

            var thickness = new Thickness(expected);

            // Act

            var actual = converter.Convert(thickness,
                                          typeof(double),
                                          null,
                                          CultureInfo.CurrentCulture);
            // Assert

            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public void Convert_ThrowsWhenFixtureIsNotUniform(double left, double top,
                                                          double right, double bottom)
        {
            // Arrange

            var thickness = new Thickness(left, top, right, bottom);

            // Act

            Action convert = () => converter.Convert(thickness,
                                                     typeof(double),
                                                     null,
                                                     CultureInfo.CurrentCulture);
            // Assert

            convert.ShouldThrow<InvalidOperationException>()
                   .WithMessage("Thickness must have uniform length on all sides.");
        }

        [Theory]
        [AutoData]
        public void ConvertsBack(double length)
        {
            // Arrange

            var expected = new Thickness(length);

            // Act

            var actual = converter.ConvertBack(length,
                                               typeof(double),
                                               null,
                                               CultureInfo.CurrentCulture);
            // Assert

            actual.Should().Be(expected);
        }
    }
}