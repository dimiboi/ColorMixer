﻿using MahApps.Metro.Controls;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;

namespace ColorMixer
{
    public partial class MainWindow : MetroWindow, IViewFor<IAppBootstrapper>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IAppBootstrapper),
                                        typeof(MainWindow),
                                        new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // ViewModel -> DataContext
                    .WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);
            });

            ViewModel = new AppBootstrapper();
        }

        public IAppBootstrapper ViewModel
        {
            get { return (IAppBootstrapper)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IAppBootstrapper)value; }
        }
    }
}