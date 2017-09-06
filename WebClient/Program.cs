using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Akka.Actor;
using Akka.Configuration;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates;
using WebClient.ActorManagement;

namespace WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("*********************************************");
            Console.WriteLine("*********** Alfredo's Akka Demo! ************");
            Console.WriteLine("*********************************************");


            /* run the web site */
            BuildWebHost(args).Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
    }
}
