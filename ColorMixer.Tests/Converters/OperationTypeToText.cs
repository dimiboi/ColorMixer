using ColorMixer.Converters;
using ColorMixer.Model;
using FluentAssertions;
using Ploeh.AutoFixture;
using System;
using System.Globalization;
using System.Windows.Data;
using Xunit;

namespace Converters
{
    public class OperationTypeToText
    {
        private static readonly IFixture fixture;

        private readonly IValueConverter converter;

        static OperationTypeToText()
        {
            fixture = new Fixture();
        }

        public OperationTypeToText()
        {
            converter = new OperationTypeToTextConverter(); // system under test
        }

        [Theory]
        [InlineData(OperationType.Addition, "\xE710")]
        [InlineData(OperationType.Subtraction, "\xE738")]
        public void Converts(OperationType operation, string expected)
        {
            // Act

            var actual = (string)converter.Convert(operation,
                                                   typeof(string),
                                                   null,
                                                   CultureInfo.CurrentCulture);
            // Assert

            actual.Should().Be(expected);
        }

        [Fact]
        public void Convert_ThrowsWhenOperationIsUnknown()
        {
            // Act

            var operation = int.MaxValue;

            Action convert = () => converter.Convert(operation,
                                                     typeof(string),
                                                     null,
                                                     CultureInfo.CurrentCulture);
            // Assert

            convert.ShouldThrow<InvalidOperationException>()
                   .WithMessage($"Unknown operation type '{operation}'.");
        }

        [Fact]
        public void DoesNotConvertBack()
        {
            // Act

            Action convert = () => converter.ConvertBack(fixture.Create<string>(),
                                                         typeof(OperationType),
                                                         null,
                                                         CultureInfo.CurrentCulture);
            // Assert

            convert.ShouldThrow<NotSupportedException>();
        }
    }
}