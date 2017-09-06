using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Akka.Actor;
using Akka.Configuration;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates;
using WebClient.ActorManagement;

namespace WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            /* run the actor system */
            var mainConfig = ConfigurationFactory.Default();
            mainConfig = mainConfig.WithFallback(getConfiguration());

            ActorSystemRefs.ActorSystem = ActorSystem.Create("demo-system", mainConfig);

            SystemActors.AccountSupervisor = ActorSystemRefs.ActorSystem.ActorOf(Props.Create<AccountActorSupervisor>(), name: "demo-supervisor");
            SystemActors.AccountSupervisor.Tell(new SuperviorStartUp(
                clientAccountsFilePath: @"/Users/alfredherr/dev/AkkaDotNetCoreDocker/AkkaDotNetCoreDocker/SampleData/Client-TVAMUVFREY.txt",
                obligationsFilePath: @"/Users/alfredherr/dev/AkkaDotNetCoreDocker/AkkaDotNetCoreDocker/SampleData/Obligations/Client-TVAMUVFREY.txt"
                ));
            
        }
        public static Config getConfiguration()
        {
            return ConfigurationFactory.ParseString($@" 

                akka.actor.debug.lifecycle = on
                akka.actor.debug.unhandled = on
                
                akka {{ loglevel = ERROR }}
                akka.loggers=[""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                
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
                akka.persistence.journal.sqlite.connection-string = ""Data Source=/Users/alfredherr/dev/AkkaDotNetCoreDocker/AkkaDotNetCoreDocker/akka_demo.db""          
                
                akka.persistence.snapshot-store.sqlite.connection-string = ""Data Source=/Users/alfredherr/dev/AkkaDotNetCoreDocker/AkkaDotNetCoreDocker/akka_demo.db""
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
