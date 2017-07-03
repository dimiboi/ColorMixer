using ColorMixer.Model;
using ColorMixer.Services;
using ColorMixer.Tests;
using ColorMixer.ViewModels;
using FluentAssertions;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Windows.Media;
using Xunit;

namespace ViewModels
{
    public class ResultNode
    {
        private readonly IKernel kernel;

        private readonly IInteractionService interactions;
        private readonly IMixerViewModel mixer;
        private readonly IInConnectorViewModel input;

        public ResultNode()
        {
            RxApp.MainThreadScheduler = Scheduler.Immediate;

            kernel = new StandardKernel();

            interactions = new InteractionService();
            mixer = Substitute.For<IMixerViewModel>();
            input = Substitute.For<IInConnectorViewModel, ReactiveObject>();

            kernel.Bind<IInteractionService>()
                  .ToConstant(interactions);

            kernel.Bind<IMixerViewModel>()
                  .ToConstant(mixer);

            kernel.Bind<IInConnectorViewModel>()
                  .ToConstant(input);

            kernel.Bind<IResultNodeViewModel>()
                  .To<ResultNodeViewModel>()
                  .InSingletonScope(); // system under test

            input.ConnectedTo = Arg.Do<IOutConnectorViewModel>(
                _ => input.RaisePropertyChanged(nameof(input.ConnectedTo)));
        }

        [Fact]
        public void SetsInput()
            => kernel.Get<IResultNodeViewModel>()
                     .Input.Should().Be(input);

        [Fact]
        public void SetsInputNode()
            => kernel.Get<IResultNodeViewModel>()
                     .Input.Received().Node = kernel.Get<IResultNodeViewModel>();

        [Theory]
        [AutoData]
        public async void SetsColor_WhenConnected(byte r, byte g, byte b)
            => await kernel.Get<IResultNodeViewModel>().ShouldUpdateColor(
                Node.DefaultColor,
                Color.FromRgb(r, g, b),
                node => { },
                node => input.ConnectToNodeWithColor(Color.FromRgb(r, g, b)));

        [Theory]
        [AutoData]
        public async void SetsDefaultColor_WhenDisconnected(byte r, byte g, byte b)
            => await kernel.Get<IResultNodeViewModel>().ShouldUpdateColor(
                Color.FromRgb(r, g, b),
                Node.DefaultColor,
                node => input.ConnectToNodeWithColor(Color.FromRgb(r, g, b)),
                node => input.ConnectedTo = null);

        [Theory]
        [AutoData]
        public async void UpdatesColor_WhenSourceColorChanges(byte r1, byte g1, byte b1,
                                                              byte r2, byte g2, byte b2)
            => await kernel.Get<IResultNodeViewModel>().ShouldUpdateColor(
                Color.FromRgb(r1, g1, b1),
                Color.FromRgb(r2, g2, b2),
                node => input.ConnectToNodeWithColor(Color.FromRgb(r1, g1, b1)),
                node => input.ConnectedTo.Node.Color = Color.FromRgb(r2, g2, b2));
    }
}