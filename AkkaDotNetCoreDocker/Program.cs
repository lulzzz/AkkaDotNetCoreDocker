using System;
using Akka.Actor;
using Akka.Configuration;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates;

namespace AkkaDotNetCoreDocker
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainConfig = ConfigurationFactory.Default();
            mainConfig = mainConfig.WithFallback(getConfiguration());

            using (var system = ActorSystem.Create("demo-system", mainConfig))
            {
                var supervisor = system.ActorOf(Props.Create<AccountActorSupervisor>(), name: "demo-supervisor");
                supervisor.Tell(new SuperviorStartUp(
                    clientAccountsFilePath: @"../SampleData/Raintree.txt",
                    obligationsFilePath: @"../SampleData/Obligations/Raintree.txt"
                    ));

                Console.WriteLine("Hello World!");
                Console.ReadLine();

            }
        }


        public static Config getConfiguration()
        {
            return ConfigurationFactory.ParseString($@" 

			    #akka.actor.debug.receive = on 
                #akka.actor.debug.autoreceive = on
                #akka.actor.debug.lifecycle = on
                #akka.actor.debug.event-stream = on
                #akka.actor.debug.unhandled = on
                
                akka {{ loglevel = DEBUG }}
                akka.loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
                
                akka.actor.serializers {{ hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""}}
                akka.actor.serialization-bindings {{ ""System.Object"" = hyperion }}
                 
                akka.persistence.journal.plugin = ""akka.persistence.journal.sqlite""
                
                akka.persistence.journal.sqlite.class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""
                akka.persistence.journal.sqlite.plugin-dispatcher = ""akka.actor.default-dispatcher""
                akka.persistence.journal.sqlite.connection-timeout = 30s
                akka.persistence.journal.sqlite.table-name = event_journal
                akka.persistence.journal.sqlite.metadata-table-name = journal_metadata
                akka.persistence.journal.sqlite.auto-initialize = on
                akka.persistence.journal.sqlite.timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                akka.persistence.journal.sqlite.connection-string = ""Data Source=../../../akka_demo.db""          
                
                akka.persistence.snapshot-store.sqlite.connection-string = ""Data Source=../../../akka_demo.db""
                akka.persistence.snapshot-store.plugin = ""akka.persistence.snapshot-store.sqlite""
                akka.persistence.snapshot-store.sqlite.class = ""Akka.Persistence.Sqlite.Snapshot.SqliteSnapshotStore, Akka.Persistence.Sqlite""
                akka.persistence.snapshot-store.sqlite.plugin-dispatcher = ""akka.actor.default-dispatcher""
                akka.persistence.snapshot-store.sqlite.connection-timeout = 30s
                akka.persistence.snapshot-store.sqlite.table-name = snapshot_store
                akka.persistence.snapshot-store.sqlite.auto-initialize = on
            ");
        }
    }
}
