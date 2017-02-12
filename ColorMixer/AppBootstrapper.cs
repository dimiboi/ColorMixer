using ColorMixer.Services;
using ColorMixer.ViewModels;
using ColorMixer.Views;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace ColorMixer
{
    public interface IMainWindowViewModel : IReactiveObject, IScreen
    {
        IObservable<KeyEventArgs> KeyDown { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AppBootstrapper : ReactiveObject, IMainWindowViewModel
    {
        private IObservable<KeyEventArgs> keyDown;

        public AppBootstrapper() : this(null, null)
        {
        }

        public AppBootstrapper(IMutableDependencyResolver resolver = null,
                               RoutingState router = null)
        {
            resolver = resolver ?? Locator.CurrentMutable;
            Router = router ?? new RoutingState();

            RegisterDependencies(resolver);

            Router.Navigate.Execute(resolver.GetService<IMixerViewModel>());
        }

        public RoutingState Router { get; private set; }

        private void RegisterDependencies(IMutableDependencyResolver resolver)
        {
            // Services

            resolver.RegisterLazySingleton(() => new InteractionService(),
                                                 typeof(IInteractionService));
            // Screen

            resolver.RegisterConstant(this,
                                      typeof(IScreen));

            resolver.RegisterConstant(this,
                                      typeof(IMainWindowViewModel));
            // ViewModels

            resolver.Register(() => new ConnectionViewModel(),
                                    typeof(IConnectionViewModel));

            resolver.Register(() => new InConnectorViewModel(),
                                    typeof(IInConnectorViewModel));

            resolver.Register(() => new OutConnectorViewModel(),
                                    typeof(IOutConnectorViewModel));

            resolver.Register(() => new ColorNodeViewModel(),
                                    typeof(IColorNodeViewModel));

            resolver.Register(() => new OperationNodeViewModel(),
                                    typeof(IOperationNodeViewModel));

            resolver.Register(() => new ResultNodeViewModel(),
                                    typeof(IResultNodeViewModel));

            resolver.RegisterConstant(new MixerViewModel(),
                                      typeof(IMixerViewModel));
            // Views

            resolver.Register(() => new ConnectionView(),
                                    typeof(IViewFor<ConnectionViewModel>));

            resolver.Register(() => new ConnectorView(),
                                    typeof(IViewFor<InConnectorViewModel>));

            resolver.Register(() => new ConnectorView(),
                                    typeof(IViewFor<OutConnectorViewModel>));

            resolver.Register(() => new ColorNodeView(),
                                    typeof(IViewFor<ColorNodeViewModel>));

            resolver.Register(() => new OperationNodeView(),
                                    typeof(IViewFor<OperationNodeViewModel>));

            resolver.Register(() => new ResultNodeView(),
                                    typeof(IViewFor<ResultNodeViewModel>));

            resolver.RegisterConstant(new MixerView(),
                                      typeof(IViewFor<MixerViewModel>));
        }

        public IObservable<KeyEventArgs> KeyDown
        {
            get { return keyDown; }
            set { this.RaiseAndSetIfChanged(ref keyDown, value); }
        }
    }
}