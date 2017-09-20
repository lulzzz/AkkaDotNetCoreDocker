using Akka.Actor;

namespace Loaner.BoundedContexts.MaintenanceBilling.Commands
{
    public class AskToBeSupervised
    {
        public AskToBeSupervised(IActorRef whoAmIAsking)
        {
            MyNewParent = whoAmIAsking;
        }

        public IActorRef MyNewParent { get; }
    }
}