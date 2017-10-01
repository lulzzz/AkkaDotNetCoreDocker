using System;

namespace Loaner
{
    using Akka.Actor;
    using Akka.Configuration;
    using Akka.Routing;
    using api.ActorManagement;
    using BoundedContexts.MaintenanceBilling.Aggregates;
    using BoundedContexts.Test;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nancy.Owin;
    using NLog.Extensions.Logging;
    using NLog.Web;

    public class Startup
    {
        private readonly IConfiguration config;
        public Startup(IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");

            ActorSystemRefs.ActorSystem = ActorSystem.Create("demo-system", GetConfiguration());
            LoanerActors.LittleActor = ActorSystemRefs.ActorSystem.ActorOf(Props.Create<LittleActor>(),"LittleActor");
            LoanerActors.AccountSupervisor = ActorSystemRefs.ActorSystem.ActorOf(Props.Create<AccountActorSupervisor>().WithRouter(new ConsistentHashingPool(10)),"demo-supervisor");
            Console.WriteLine($"Supervisor's name is: {LoanerActors.AccountSupervisor.Path.Name}");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(config, appConfig);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new DemoBootstrapper(appConfig)));
            
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            //add NLog.Web
            app.AddNLogWeb();
        }
    
        public static Config GetConfiguration()
        {
            var hocon = @" 
               akka 
               {
                    actor 
                    {
                     serializers 
                        {
                          hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion"" 
                        }
                        serialization-bindings 
                        {
                            ""System.Object"" = hyperion
                        }
                    }
                }
                #akka.suppress-json-serializer-warning = on
                
                akka.actor.debug.lifecycle = on
                akka.actor.debug.unhandled = on
                
                akka.loglevel = DEBUG
                
                akka.loggers=[""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                     
            ## Postgresql         
            akka.persistence{
	        journal {
		        plugin = ""akka.persistence.journal.postgresql""

                postgresql {
			        # qualified type name of the PostgreSql persistence journal actor
			        class = ""Akka.Persistence.PostgreSql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""

                    # dispatcher used to drive journal actor
                    plugin-dispatcher = ""akka.actor.default-dispatcher""

                    # connection string used for database access
                    connection-string = ""Server=localhost;Port=5433;Database=akka;User Id=alfredherr;Password=Testing.123;""

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
			        connection-string = ""Server=localhost;Port=5433;Database=akka;User Id=alfredherr;Password=Testing.123;""

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

                ## SqLite
                #akka.persistence.journal.plugin = ""akka.persistence.journal.sqlite""
                #akka.persistence.journal.sqlite.class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""
                #akka.persistence.journal.sqlite.plugin-dispatcher = ""akka.actor.default-dispatcher""
                #akka.persistence.journal.sqlite.connection-timeout = 30s
                #akka.persistence.journal.sqlite.table-name = event_journal
                #akka.persistence.journal.sqlite.metadata-table-name = journal_metadata
                #akka.persistence.journal.sqlite.auto-initialize = on
                #akka.persistence.journal.sqlite.timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                #akka.persistence.journal.sqlite.connection-string = ""Data Source=../../../akka_demo.db""
                #
                #akka.persistence.snapshot-store.plugin = ""akka.persistence.snapshot-store.sqlite""
                #akka.persistence.snapshot-store.sqlite.connection-string = ""Data Source=../../../akka_demo.db""
                #akka.persistence.snapshot-store.sqlite.class = ""Akka.Persistence.Sqlite.Snapshot.SqliteSnapshotStore, Akka.Persistence.Sqlite""
                #akka.persistence.snapshot-store.sqlite.plugin-dispatcher = ""akka.actor.default-dispatcher""
                #akka.persistence.snapshot-store.sqlite.connection-timeout = 30s
                #akka.persistence.snapshot-store.sqlite.table-name = snapshot_store
                #akka.persistence.snapshot-store.sqlite.auto-initialize = on

                #akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                #akka.remote.log-remote-lifecycle-events = DEBUG
                #akka.remote.dot-netty.tcp.hostname = ""127.0.0.1""
                #akka.remote.dot-netty.tcp.port = 0
                #akka.cluster.seed-nodes = [""akka.tcp://demo-system@127.0.0.1:4053""] 
                #akka.cluster.roles = [concord]

#my-dispatcher {
#    type = ForkJoinDispatcher
#        throughput = 100000
#        dedicated-thread-pool {
#            thread-count = 1 
#            threadtype = background
#        }
#}
#akka.actor.deployment { 
#    /demo-supervisor {
#        dispatcher = my-dispatcher
#    }
#}

           ";
            return ConfigurationFactory.ParseString(hocon);

        }
    }
}
