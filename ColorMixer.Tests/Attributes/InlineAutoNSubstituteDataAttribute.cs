using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace ColorMixer.Tests.Attributes
{
    public class InlineAutoNSubstituteDataAttribute : CompositeDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
            : base(new InlineDataAttribute(values), new AutoNSubstituteDataAttribute())
        {
        }
    }
}