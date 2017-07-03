using ColorMixer.Model;
using ColorMixer.Tests;
using ColorMixer.Tests.Attributes;
using FluentAssertions;
using Ninject;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows;
using Xunit;

namespace Model
{
    public class TestConnector : Connector
    {
        public override ReactiveCommand<Unit, Unit> ConnectorCommand { get; protected set; }

        public override ConnectorDirection Direction { get; }

        public override bool IsConnected { get; }

        public override bool IsEnabled { get; }
    }

    public class ConnectorModel
    {
        private readonly IKernel kernel;

        public ConnectorModel()
        {
            RxApp.MainThreadScheduler = Scheduler.Immediate;

            kernel = new StandardKernel();

            kernel.Bind<IConnector>()
                  .To<TestConnector>()
                  .InSingletonScope(); // system under test
        }

        [Fact]
        public void SetsActivator()
            => kernel.Get<IConnector>()
                     .Activator.Should().NotBeNull();

        [Theory]
        [InlineAutoNSubstituteData(nameof(Connector.Node))]
        public void SetsNodeProperties(string property, INode initial, INode expected)
            => kernel.Get<IConnector>()
                     .ShouldSetProperty(property, initial, expected);

        [Theory]
        [InlineAutoNSubstituteData(nameof(Connector.ConnectionPoint))]
        public void SetsPointProperties(string property, Point initial, Point expected)
            => kernel.Get<IConnector>()
                     .ShouldSetProperty(property, initial, expected);
    }
}