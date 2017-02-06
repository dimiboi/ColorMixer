using ColorMixer.ViewModels;
using ColorMixer.Views;
using ReactiveUI;
using Splat;
using System;
using System.Windows.Input;

namespace ColorMixer
{
    public interface IMainWindowViewModel : IReactiveObject, IScreen
    {
        IObservable<KeyEventArgs> KeyDown { get; set; }
    }

    public class MainWindowViewModel : ReactiveObject, IMainWindowViewModel
    {
        private IObservable<KeyEventArgs> keyDown;

        public MainWindowViewModel(IMutableDependencyResolver resolver = null,
                                   RoutingState router = null)
        {
            Router = router ?? new RoutingState();
            resolver = resolver ?? Locator.CurrentMutable;

            RegisterDependencies(resolver);

            Router.Navigate.Execute(resolver.GetService<IMixerViewModel>());
        }

        public RoutingState Router { get; private set; }

        private void RegisterDependencies(IMutableDependencyResolver resolver)
        {
            // Screen

            resolver.RegisterConstant(this,
                                      typeof(IScreen));

            resolver.RegisterConstant(this,
                                      typeof(IMainWindowViewModel));
            // ViewModels

            resolver.RegisterConstant(new MixerViewModel(),
                                      typeof(IMixerViewModel));

            resolver.Register(() => new NodeViewModel(),
                                    typeof(INodeViewModel));

            resolver.Register(() => new ConnectorViewModel(),
                                    typeof(IConnectorViewModel));

            resolver.Register(() => new ConnectionViewModel(),
                                    typeof(IConnectionViewModel));
            // Views

            resolver.RegisterConstant(new MixerView(),
                                      typeof(IViewFor<MixerViewModel>));

            resolver.Register(() => new NodeView(),
                                    typeof(IViewFor<NodeViewModel>));

            resolver.Register(() => new ConnectorView(),
                                    typeof(IViewFor<ConnectorViewModel>));

            resolver.Register(() => new ConnectionView(),
                                    typeof(IViewFor<ConnectionViewModel>));
        }

        public IObservable<KeyEventArgs> KeyDown
        {
            get { return keyDown; }
            set { this.RaiseAndSetIfChanged(ref keyDown, value); }
        }
    }
}