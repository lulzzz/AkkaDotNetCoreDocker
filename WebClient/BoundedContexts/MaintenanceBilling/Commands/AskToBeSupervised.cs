using Akka.Actor;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands
{
    public class AskToBeSupervised
    {
        public AskToBeSupervised(IActorRef whoAmIAsking)
        {
            MyNewParent = whoAmIAsking;
        }

        public IActorRef MyNewParent { get; private set; }
    }
}
