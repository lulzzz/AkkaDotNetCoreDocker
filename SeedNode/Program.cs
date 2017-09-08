using System;
using Akka.Actor;
using Akka.Configuration;
using SeedNode.Actors;

namespace SeedNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainConfig = ConfigurationFactory.Default();
            mainConfig = mainConfig.WithFallback(getConfiguration());

            var system  = ActorSystem.Create("demo-system", mainConfig);

            var seed = system.ActorOf(Props.Create<SeedNodeActor>(),"clusterListener");
            seed.Tell("How are you?");

            Console.ReadLine();
        }
        public static Config getConfiguration()
        {
            return ConfigurationFactory.ParseString($@" 

                akka.actor.debug.lifecycle = on
                akka.actor.debug.unhandled = on
                
                akka.loglevel = DEBUG
                akka.loggers=[""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                
                akka.actor.serializers {{ hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""}}
                akka.actor.serialization-bindings {{ ""System.Object"" = hyperion }}
                 
                akka.suppress-json-serializer-warning = on
                
                akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                akka.remote.log-remote-lifecycle-events = DEBUG
                akka.remote.dot-netty.tcp.hostname = ""localhost""
                akka.remote.dot-netty.tcp.port = 4053
                akka.cluster.seed-nodes = [""akka.tcp://demo-system@localhost:4053""] 

           ");
        }
    }
}
