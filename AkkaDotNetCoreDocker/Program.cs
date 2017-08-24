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
                    #loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog"",
                                ""AkkaNetCustomLogger.Loggers.ConsoleLogger, AkkaNetCustomLogger""]
                }
            akka 
                {  
                    stdout-loglevel = info
                    loglevel = info
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
         
            akka{
                    persistence{
                        journal {
                            plugin = ""akka.persistence.journal.postgresql""
                            postgresql {
                                # qualified type name of the PostgreSql persistence journal actor
                                class = ""Akka.Persistence.PostgreSql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""

                                # dispatcher used to drive journal actor
                                plugin-dispatcher = ""akka.actor.default-dispatcher""

                                # connection string used for database access
                                connection-string = ""postgresql://postgres@localhost/akka_demo&port=5432""

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # PostgreSql schema name to table corresponding with persistent journal
                                schema-name = public

                                # PostgreSql table corresponding with persistent journal
                                table-name = event_journal

                                # should corresponding journal table be initialized automatically
                                auto-initialize = on
                                
                                # timestamp provider used for generation of journal entries timestamps
                                timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                            
                                # metadata table
                                metadata-table-name = metadata

                                # defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
                                stored-as = BYTEA
                            }
                        }

                        snapshot-store {
                            plugin = ""akka.persistence.snapshot-store.postgresql""
                            postgresql {
                                # qualified type name of the PostgreSql persistence journal actor
                                class = ""Akka.Persistence.PostgreSql.Snapshot.PostgreSqlSnapshotStore, Akka.Persistence.PostgreSql""

                                # dispatcher used to drive journal actor
                                plugin-dispatcher = ""akka.actor.default-dispatcher""

                                # connection string used for database access
                                connection-string = ""postgresql://postgres@localhost/akka_demo&port=5432""
                                

                                # default SQL commands timeout
                                connection-timeout = 30s

                                # PostgreSql schema name to table corresponding with persistent journal
                                schema-name = public

                                # PostgreSql table corresponding with persistent journal
                                table-name = snapshot_store

                                # should corresponding journal table be initialized automatically
                                auto-initialize = on
                                
                                # defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
                                stored-as = BYTEA
                            }
                        }
                }
            } 
");

			using (var system = ActorSystem.Create("demo-system",config))
			{

				var supervisor = system.ActorOf(Props.Create<AccountActorSupervisor>(), name: "demo-supervisor");
                supervisor.Tell(new SuperviorStartUp( @"/Users/alfredherr/dev/AkkaDotNetCoreDocker/Raintree.txt" ));

				var faker = system.ActorOf(Props.Create<AccountActorFaker>(), name: "faker");
				faker.Tell(new MakeFakeData(6), supervisor);

				Console.WriteLine("Hello World!");
				Console.ReadLine();

			}
        }
    }
}
