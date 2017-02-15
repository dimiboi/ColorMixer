using ColorMixer.ViewModels;
using Splat;
using System.Diagnostics.CodeAnalysis;

namespace ColorMixer.Services
{
    public interface IConnectionFactory
    {
        IConnectionViewModel Create();
    }

    [ExcludeFromCodeCoverage]
    public class ConnectionFactory : IConnectionFactory
    {
        public IConnectionViewModel Create()
            => Locator.Current.GetService<IConnectionViewModel>();
    }
}