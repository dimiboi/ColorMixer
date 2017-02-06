using ColorMixer.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace ColorMixer.Services
{
    public interface IInteractionService
    {
        Interaction<IColorNodeViewModel, Unit> DeleteNode { get; }
    }

    public class InteractionService : IInteractionService
    {
        public Interaction<IColorNodeViewModel, Unit> DeleteNode { get; }
            = new Interaction<IColorNodeViewModel, Unit>();
    }
}