using ReactiveUI;
using System.Reactive;
using System.Windows.Media;

namespace ColorMixer.ViewModels
{
    public interface INodeViewModel : IReactiveObject, ISupportsActivation
    {
        string Title { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Color Color { get; set; }
        ReactiveCommand<Unit, Unit> DeleteNodeCommand { get; }
    }
}