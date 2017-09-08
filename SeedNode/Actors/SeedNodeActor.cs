using System;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace SeedNode.Actors
{
    public class SeedNodeActor : ReceiveActor
    {
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);

       // Cluster cluster = Cluster.Get(Context.System);

        public SeedNodeActor(){
           
            Receive<ClusterEvent.MemberUp>(m => _log.Info("Member is Up: {0}", m.Member));
            Receive<ClusterEvent.UnreachableMember>(m=> _log.Info("Member detected as unreachable: {0}", m.Member));
            Receive<ClusterEvent.MemberRemoved>(m=> _log.Info("Member is Removed: {0}", m.Member));
            Receive<string>(s =>
            {
                _log.Info(s);
                var answer = Context.ActorSelection($"akka://demo-system/user/demo-supervisor").Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(5)).Result;
                _log.Info(answer.Message);
            });
        }
        protected override void PreStart()
        {
        //    cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        }

        /// <summary>
        /// Re-subscribe on restart
        /// </summary>
        protected override void PostStop()
        {
        //    cluster.Unsubscribe(Self);
        }
          
    }
}