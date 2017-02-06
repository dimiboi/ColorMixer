using ReactiveUI;
using System.Reactive;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface INodeViewModelBase : IReactiveObject, ISupportsActivation
    {
        string Title { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Color Color { get; set; }
        ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
    }

    public abstract class NodeViewModelBase : ReactiveObject, INodeViewModelBase
    {
        public abstract ViewModelActivator Activator { get; }
        public abstract Color Color { get; set; }
        public abstract ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
        public abstract double Height { get; set; }
        public abstract string Title { get; }
        public abstract double Width { get; set; }
        public abstract double X { get; set; }
        public abstract double Y { get; set; }
    }
}