﻿using ReactiveUI;
using System.Reactive.Disposables;

namespace ColorMixer.ViewModels
{
    public interface IConnectionViewModel : IReactiveObject, ISupportsActivation
    {
        IOutputConnectorViewModel From { get; set; }
        IInputConnectorViewModel To { get; set; }
    }

    public class ConnectionViewModel : ReactiveObject, IConnectionViewModel
    {
        private IOutputConnectorViewModel from;
        private IInputConnectorViewModel to;

        public ConnectionViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                Disposable.Empty.DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; private set; }

        public IOutputConnectorViewModel From
        {
            get { return from; }
            set { this.RaiseAndSetIfChanged(ref from, value); }
        }

        public IInputConnectorViewModel To
        {
            get { return to; }
            set { this.RaiseAndSetIfChanged(ref to, value); }
        }
    }
}