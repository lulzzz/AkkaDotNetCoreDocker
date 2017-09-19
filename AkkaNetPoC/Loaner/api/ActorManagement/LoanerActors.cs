using Akka.Actor;

namespace Loaner.api.ActorManagement
{
    public static class LoanerActors
    {
        public static IActorRef AccountSupervisor = ActorRefs.Nobody;
        public static IActorRef LittleActor = ActorRefs.Nobody;
    }


}
