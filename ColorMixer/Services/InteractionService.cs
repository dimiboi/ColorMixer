using ColorMixer.Model;
using ColorMixer.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Windows;
using System.Windows.Media;

namespace ColorMixer.Services
{
    public interface IInteractionService
    {
        Interaction<Unit, Point?> GetNewNodePoint { get; }

        Interaction<INode, Unit> DeleteNode { get; }

        Interaction<Color, Color> GetNodeColor { get; }

        Interaction<OperationType, OperationType> GetNodeOperation { get; }

        Interaction<IInConnectorViewModel, IOutConnectorViewModel> GetOutConnector { get; }

        Interaction<IOutConnectorViewModel, IInConnectorViewModel> GetInConnector { get; }
    }

    public class InteractionService : IInteractionService
    {
        public Interaction<Unit, Point?> GetNewNodePoint
        { get; } = new Interaction<Unit, Point?>();

        public Interaction<INode, Unit> DeleteNode
        { get; } = new Interaction<INode, Unit>();

        public Interaction<Color, Color> GetNodeColor
        { get; } = new Interaction<Color, Color>();

        public Interaction<OperationType, OperationType> GetNodeOperation
        { get; } = new Interaction<OperationType, OperationType>();

        public Interaction<IInConnectorViewModel, IOutConnectorViewModel> GetOutConnector
        { get; } = new Interaction<IInConnectorViewModel, IOutConnectorViewModel>();

        public Interaction<IOutConnectorViewModel, IInConnectorViewModel> GetInConnector
        { get; } = new Interaction<IOutConnectorViewModel, IInConnectorViewModel>();
    }
}