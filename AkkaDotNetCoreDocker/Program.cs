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
            var config = ConfigurationFactory.ParseString(@"
                akka {  
	          loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
	          stdout-loglevel = DEBUG
	          loglevel = DEBUG
	          log-config-on-start = on         
	        actor {               
		        debug {
				        receive = on # log any received message
				        autoreceive= on # log automatically received messages, e.g. PoisonPill
				        lifecycle = on # log actor lifecycle changes
				        fsm = on # log all LoggingFSMs for events, transitions and timers
				        event-stream = on # log subscription changes for Akka.NET event stream
				        unhandled = on # log unhandled messages sent to actors
			        }
			    serializers {
                 hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
              }
          serialization-bindings {
                ""System.Object"" = hyperion
                  }
       }
	    }       
      ﻿akka.persistence{

	      journal {
		      plugin = ""akka.persistence.journal.sqlite""

              sqlite {
		
			      # qualified type name of the SQLite persistence journal actor
			      class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""

                  # dispatcher used to drive journal actor
                plugin-dispatcher = ""akka.actor.default-dispatcher""

                  # connection string used for database access
                connection-string = ""Data Source=C:\\dev\\sqlitedb\\akka.db; Version=3;""
			
			      # connection string name for .config file used when no connection string has been provided
			      connection-string-name = """"

			      # default SQLite commands timeout
			      connection-timeout = 30s

			      # SQLite table corresponding with persistent journal
			      table-name = event_journal
			
			      # metadata table
			      metadata-table-name = journal_metadata

			      # should corresponding journal table be initialized automatically
			      auto-initialize = off

			      # timestamp provider used for generation of journal entries timestamps
			      timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""


                  circuit -breaker {
				      max-failures = 5
				      call-timeout = 20s
				      reset-timeout = 60s
			      }
		      }
	      }

	      snapshot-store {
		      plugin = ""akka.persistence.snapshot-store.sqlite""

              sqlite {
		
			      # qualified type name of the SQLite persistence journal actor
			      class = ""Akka.Persistence.Sqlite.Snapshot.SqliteSnapshotStore, Akka.Persistence.Sqlite""

                  # dispatcher used to drive journal actor
                plugin dispatcher = ""akka.actor.default-dispatcher""

                  # connection string used for database access
                connection-string = ""Data Source=C:\\dev\\sqlitedb\\akka.db; Version=3;""

			      # connection string name for .config file used when no connection string has been provided
			      connection-string-name = """"

			      # default SQLite commands timeout
			      connection-timeout = 30s
			
			      # SQLite table corresponding with persistent journal
			      table-name = snapshot_store

			      # should corresponding journal table be initialized automatically
			      auto-initialize = off

		      }
	      }
      }
 

");

            using (var system = ActorSystem.Create("demo-system",config))
            {

                var supervisor = system.ActorOf(Props.Create<AccountActorSupervisor>(), name: "demo-supervisor");
                supervisor.Tell(new SuperviorStartUp(
                    clientAccountsFilePath: @"C:\dev\AkkaDotNetCoreDocker\SampleData\Raintree.txt",
                    obligationsFilePath: @"C:\dev\AkkaDotNetCoreDocker\SampleData\Obligations\Raintree.txt"
                    ));

                //var faker = system.ActorOf(Props.Create<AccountActorFaker>(), name: "faker");
                //faker.Tell(new MakeFakeData(6), supervisor);

                Console.WriteLine("Hello World!");
                Console.ReadLine();

            }
        }
    }
}
