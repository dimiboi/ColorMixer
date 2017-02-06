using ColorMixer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ColorMixer.Views
{
    public class NodeViewBase<T> : UserControl, IViewFor<T> where T : class, INodeViewModelBase
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(T),
                                        typeof(NodeViewBase<T>),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container",
                                        typeof(FrameworkElement),
                                        typeof(NodeViewBase<T>),
                                        new PropertyMetadata(null));

        public NodeViewBase()
        {
            IDisposable activation = null;
            activation = this.WhenActivated(disposables =>
            {
                activation
                    .DisposeWith(disposables);

                this // ViewModel -> DataContext
                    .WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this // Make sure node X stays within bounds
                    .WhenAnyValue(v => v.ViewModel.X, // node X changed
                                  v => v.ActualWidth, // node width changed
                                  v => v.Container.ActualWidth, // container resized
                                  (a, b, c) => new { X = a, Width = b, ContainerWidth = c })
                    .Select(e => e.X > 0
                                 ? e.X + e.Width > e.ContainerWidth
                                   ? e.ContainerWidth - e.Width
                                   : e.X
                                 : 0)
                    .BindTo(ViewModel, vm => vm.X)
                    .DisposeWith(disposables);

                this // Make sure node Y stays within bounds
                    .WhenAnyValue(v => v.ViewModel.Y, // node Y changed
                                  v => v.ActualHeight, // node height changed
                                  v => v.Container.ActualHeight, // container resized
                                  (a, b, c) => new { Y = a, Height = b, ContainerHeight = c })
                    .Select(e => e.Y > 0
                                 ? e.Y + e.Height > e.ContainerHeight
                                   ? e.ContainerHeight - e.Height
                                   : e.Y
                                 : 0)
                    .BindTo(ViewModel, vm => vm.Y)
                    .DisposeWith(disposables);

                this // Clear Container when node is deleted
                    .WhenAnyValue(v => v.ViewModel) // when ViewModel is set
                    .SelectMany(vm => vm.DeleteNodeCommand) // subscribe to command execution
                    .Subscribe(_ => Container = null) // clear the container when node is deleted
                    .DisposeWith(disposables);
            });
        }

        public T ViewModel
        {
            get { return (T)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (T)value; }
        }

        public FrameworkElement Container
        {
            get { return (FrameworkElement)GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }
    }
}