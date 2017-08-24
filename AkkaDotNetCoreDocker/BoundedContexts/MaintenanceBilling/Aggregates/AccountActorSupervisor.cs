using System;
using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;


namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActorSupervisor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private readonly IActorRef logger;

        private Dictionary<Account, IActorRef> _accounts = new Dictionary<Account, IActorRef>();

        public AccountActorSupervisor()
        {

            logger = Context.ActorOf(Props.Create<LoggingActor>(), Self.Path.Name + "logger");

            Receive<SuperviorStartUp>(client =>
            {
                _log.Info($"Client: {client.Client}");
                _log.Info($"Client: {client.Client}");
                Console.WriteLine($"Client: {client.Client}");
                StartUp(client.Client);
            });
            Receive<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));
            Receive<AboutMe>(me => Console.WriteLine(me.Me));
            Receive<SeedData>(data =>
            {
                foreach (var account in _accounts)
                {
                    foreach (var datum in data.DomainCommands)
                    {
                        account.Value.Tell(datum);
                        logger.Tell($"Sent {account.Value.Path.Name} {datum.GetType()} command");
                    }
                };
            });
        }

        private void SayHi(IActorRef actor)
        {
            actor.Tell(new SayHi());
        }

        private void StartUp(string clientPath)
        {
            var accounts = GetAccountsUnderThisSuperVisor(clientPath);
            logger.Tell($"There were {accounts.Count} accounts loaded.");
            foreach (var account in accounts)
            {
                var accountActor = Context.ActorOf<AccountActor>(account.Key.ToString());
                _log.Info($"Account: {account}");
                logger.Tell($"Account: {account}");
                _accounts.Add(new Account(account.Key.ToString()), accountActor);
                SayHi(accountActor);

            }
        }

        private Dictionary<string, string> GetAccountsUnderThisSuperVisor(string clientPath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                _log.Info($"Gonna try to open file {clientPath}");
                logger.Tell($"Gonna try to open file {clientPath}");
                if (File.Exists(clientPath))
                {
                    string[] readText = File.ReadAllLines(clientPath);
                    logger.Tell($"{clientPath} has {readText.Length} lines");
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        dictionary.Add(line[0], line[1]);
                        logger.Tell($"Adding account #{line[0]} to dictionary");
                    }
                }
                _log.Info($"Successfully processing file {clientPath}");
                logger.Tell($"Successfully processing file {clientPath}");

            }
            catch (Exception e)
            {
                Self.Tell(new FailedToLoadAccounts(e.Message));
            }
            return dictionary;
        }


    }

    public class FailedToLoadAccounts
    {
        public FailedToLoadAccounts(string message)
        {
            this.Message = message;
        }
        public string Message { get; private set; }
    }
    public class SuperviorStartUp
    {
        public SuperviorStartUp(string clientName)
        {
            this.Client = clientName;
        }
        public string Client { get; private set; }
    }
}