using ColorMixer.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace ColorMixer.Services
{
    public interface IInteractionService
    {
        Interaction<INodeViewModel, Unit> DeleteNode { get; }
    }

    public class InteractionService : IInteractionService
    {
        public Interaction<INodeViewModel, Unit> DeleteNode { get; }
            = new Interaction<INodeViewModel, Unit>();
    }
}