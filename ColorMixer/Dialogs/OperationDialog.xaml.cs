using ColorMixer.Model;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ColorMixer.Dialogs
{
    [ExcludeFromCodeCoverage]
    public partial class OperationDialog : CustomDialog, IActivatable
    {
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register("Operation",
                                        typeof(OperationType),
                                        typeof(OperationDialog),
                                        new PropertyMetadata(OperationType.Addition));

        public OperationDialog()
        {
            InitializeComponent();

            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // Operation -> AdditionRadioButton.IsChecked
                    .WhenAnyValue(v => v.Operation)
                    .Select(o => o == OperationType.Addition)
                    .BindTo(this, v => v.AdditionRadioButton.IsChecked)
                    .DisposeWith(disposables);

                this // Operation -> SubtractionRadioButton.IsChecked
                    .WhenAnyValue(v => v.Operation)
                    .Select(o => o == OperationType.Subtraction)
                    .BindTo(this, v => v.SubtractionRadioButton.IsChecked)
                    .DisposeWith(disposables);

                this // AdditionRadioButton.IsChecked -> Operation
                    .WhenAnyValue(v => v.AdditionRadioButton.IsChecked)
                    .Where(v => v == true)
                    .Select(_ => OperationType.Addition)
                    .BindTo(this, v => v.Operation)
                    .DisposeWith(disposables);

                this // SubtractionRadioButton.IsChecked -> Operation
                    .WhenAnyValue(v => v.SubtractionRadioButton.IsChecked)
                    .Where(v => v == true)
                    .Select(_ => OperationType.Subtraction)
                    .BindTo(this, v => v.Operation)
                    .DisposeWith(disposables);
            });
        }

        public async Task<bool> WaitUntilClosedAsync()
        {
            var isAccepted = await
                Observable.Merge(
                    AcceptButton.Events()
                                .Click
                                .Select(_ => true),
                    CancelButton.Events()
                                .Click
                                .Select(_ => false)).FirstAsync();
            return isAccepted;
        }

        public OperationType Operation
        {
            get { return (OperationType)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }
    }
}