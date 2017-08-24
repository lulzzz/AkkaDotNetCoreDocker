using System;
using System.Collections.Generic;
using Akka.Actor;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    class AccountActorFaker : ReceiveActor
    {
        public List<IDomainCommand> DomainCommands = new List<IDomainCommand>();
        private readonly IActorRef logger;

        public AccountActorFaker()
        {
            logger = Context.ActorOf(Props.Create<LoggingActor>(), Self.Path.Name + "logger");

            Receive<MakeFakeData>(make =>
           {
               for (int i = 1; i <= make.NumberOfRecords; i++)
               {
                   DomainCommands.Add(new SettleFinancialConcept());
                   DomainCommands.Add(new AssessFinancialConcept());
				   DomainCommands.Add(new CancelAccount());

				   logger.Tell($"Added Domain Command {i}");

               };
               Sender.Tell(new SeedData(DomainCommands));
           });

            Receive<Echo>(s => Sender.Tell(s.Message));
        }
    }

    public class Echo
    {
        public Echo(string message)
        {
            Message = message;
        }
        public string Message { get; private set; }
    }
    public class SeedData
    {
        public List<IDomainCommand> DomainCommands { get; }

        public SeedData(List<IDomainCommand> domainCommands)
        {
            DomainCommands = domainCommands;
        }
    }


    class MakeFakeData
    {
        public MakeFakeData(int numberOfRecords)
        {
            NumberOfRecords = numberOfRecords;
        }

        public int NumberOfRecords { get; set; }
    }
}
