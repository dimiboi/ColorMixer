using ColorMixer.Model;
using ReactiveUI;
using System.Reactive;

namespace ColorMixer.Services
{
    public interface IInteractionService
    {
        Interaction<INode, Unit> DeleteNode { get; }
    }

    public class InteractionService : IInteractionService
    {
        public Interaction<INode, Unit> DeleteNode { get; } = new Interaction<INode, Unit>();
    }
}