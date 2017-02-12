using Ploeh.AutoFixture.Xunit2;
using Xunit;
using Xunit.Sdk;

namespace ColorMixer.Tests.Attributes
{
    public class InlineAutoNSubstituteDataAttribute : CompositeDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
            : base(new DataAttribute[]
            {
                new InlineDataAttribute(values),
                new AutoNSubstituteDataAttribute()
            })
        {
        }
    }
}