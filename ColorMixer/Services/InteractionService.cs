using ColorMixer.Model;
using ReactiveUI;
using System.Reactive;
using System.Windows.Media;

namespace ColorMixer.Services
{
    public interface IInteractionService
    {
        Interaction<INode, Unit> DeleteNode { get; }

        Interaction<Color, Color> GetNodeColor { get; }

        Interaction<OperationType, OperationType> GetNodeOperation { get; }
    }

    public class InteractionService : IInteractionService
    {
        public Interaction<INode, Unit> DeleteNode { get; }
            = new Interaction<INode, Unit>();

        public Interaction<Color, Color> GetNodeColor { get; }
            = new Interaction<Color, Color>();

        public Interaction<OperationType, OperationType> GetNodeOperation { get; }
            = new Interaction<OperationType, OperationType>();
    }
}