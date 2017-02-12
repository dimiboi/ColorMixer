using ColorMixer.Dialogs;
using ColorMixer.Services;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ColorMixer
{
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : MetroWindow, IViewFor<IMainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(IMainWindowViewModel),
                                        typeof(MainWindow),
                                        new PropertyMetadata(null));

        private readonly IInteractionService interactions;

        public MainWindow() : this(null)
        {
        }

        public MainWindow(IInteractionService interactions)
        {
            InitializeComponent();

            ViewModel = Application.Current // get the bootstrapper from App resources
                                   .FindResource("MainWindowViewModel") as IMainWindowViewModel;

            this.interactions = interactions ?? Locator.Current.GetService<IInteractionService>();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // ViewModel -> DataContext
                    .WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this // KeyDown -> ViewModel.KeyDown
                    .WhenAnyValue(v => v.ViewModel)
                    .Select(_ => this.Events().KeyDown)
                    .BindTo(ViewModel, vm => vm.KeyDown)
                    .DisposeWith(disposables);

                this // Handle GetNodeColor interaction
                    .interactions
                    .GetNodeColor
                    .RegisterHandler(async interaction =>
                    {
                        var dialog = new ColorDialog
                        {
                            Color = interaction.Input // dialog edits what the node sent here
                        };

                        await this.ShowMetroDialogAsync(dialog);
                        var isAccepted = await dialog.WaitUntilClosedAsync();
                        await this.HideMetroDialogAsync(dialog);

                        if (isAccepted)
                        {
                            interaction.SetOutput(dialog.Color); // return new color
                        }
                        else
                        {
                            interaction.SetOutput(interaction.Input); // return old color
                        }
                    })
                    .DisposeWith(disposables);

                this
                    .interactions
                    .GetNodeOperation
                    .RegisterHandler(async interaction =>
                    {
                        var dialog = new OperationDialog
                        {
                            Operation = interaction.Input // dialog edits what the node sent here
                        };

                        await this.ShowMetroDialogAsync(dialog);
                        var isAccepted = await dialog.WaitUntilClosedAsync();
                        await this.HideMetroDialogAsync(dialog);

                        if (isAccepted)
                        {
                            interaction.SetOutput(dialog.Operation); // return new operation
                        }
                        else
                        {
                            interaction.SetOutput(interaction.Input); // return old operation
                        }
                    })
                    .DisposeWith(disposables);
            });
        }

        public IMainWindowViewModel ViewModel
        {
            get { return (IMainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IMainWindowViewModel)value; }
        }
    }
}