using ColorMixer.Tests;
using ColorMixer.Tests.Attributes;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using ReactiveUI;
using System.Reactive.Concurrency;
using Xunit;

namespace ViewModels
{
    public class Connection
    {
        private readonly IKernel kernel;

        public Connection()
        {
            RxApp.MainThreadScheduler = Scheduler.Immediate;

            kernel = new StandardKernel();

            kernel.Bind<IConnectionViewModel>()
                  .To<ConnectionViewModel>()
                  .InSingletonScope(); // system under test
        }

        [Fact]
        public void SetsActivator()
            => kernel.Get<IConnectionViewModel>()
                     .Activator.Should().NotBeNull();

        [Theory]
        [InlineAutoNSubstituteData(nameof(IConnectionViewModel.From))]
        public void SetsOutProperties(string property,
                                      IOutConnectorViewModel initial,
                                      IOutConnectorViewModel expected)
            => kernel.Get<IConnectionViewModel>()
                     .ShouldSetProperty(property, initial, expected);

        [Theory]
        [InlineAutoNSubstituteData(nameof(IConnectionViewModel.To))]
        public void SetsInProperties(string property,
                                     IInConnectorViewModel initial,
                                     IInConnectorViewModel expected)
            => kernel.Get<IConnectionViewModel>()
                     .ShouldSetProperty(property, initial, expected);
    }
}