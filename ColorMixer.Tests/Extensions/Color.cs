using ColorMixer.Extensions;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using System.Windows.Media;
using Xunit;

namespace Extensions
{
    public class ColorExtensions
    {
        [Theory]
        [AutoData]
        public void AddsColor(byte r1, byte g1, byte b1, byte a1,
                              byte r2, byte g2, byte b2, byte a2)
            => Color.FromArgb(a1, r1, g1, b1)
                    .Add(Color.FromArgb(a2, r2, g2, b2))
                    .Should().Be(Color.FromArgb(a1.Add(a2),
                                                r1.Add(r2),
                                                g1.Add(g2),
                                                b1.Add(b2)));

        [Theory]
        [AutoData]
        public void SubtractsColor(byte r1, byte g1, byte b1, byte a1,
                                   byte r2, byte g2, byte b2, byte a2)
            => Color.FromArgb(a1, r1, g1, b1)
                    .Subtract(Color.FromArgb(a2, r2, g2, b2))
                    .Should().Be(Color.FromArgb(a1.Subtract(a2),
                                                r1.Subtract(r2),
                                                g1.Subtract(g2),
                                                b1.Subtract(b2)));

        [Theory]
        [InlineData(200, 100, 255)]
        [InlineData(100, 100, 200)]
        public void AddsByte_WithoutOverflow(byte a, byte b, byte expected)
            => a.Add(b).Should().Be(expected);

        [Theory]
        [InlineData(100, 200, 0)]
        [InlineData(200, 100, 100)]
        public void SubtractsByte_WithoutUnderflow(byte a, byte b, byte expected)
            => a.Subtract(b).Should().Be(expected);
    }
}