using System;
using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;

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
                _log.Info($"Client: {client.ClientAccountFilePath}");
                Console.WriteLine($"SuperviorStartUp on client: {client.ClientAccountFilePath} and {client.ObligationsFilePath}");
                StartUp(client.ClientAccountFilePath, client.ObligationsFilePath);
            });

            Receive<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));

            Receive<FailedToLoadObligations>(m => Self.Tell(typeof(Stop)));

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


        private void StartUp(string accountsFilePath, string obligationsFilePath)
        {
            var accounts = GetAccountsUnderThisSuperVisor(accountsFilePath);
            var obligations = GetAccountsUnderThisSuperVisor(obligationsFilePath);

            logger.Tell($"There were {accounts.Count} accounts loaded.");

            foreach (var account in accounts)
            {
                _log.Info($"Account: {account.Key}");
                logger.Tell($"Account: {account.Key}");

                var accountActor = Context.ActorOf<AccountActor>(account.Key.ToString());
         
                accountActor.Tell(new CreateAccount(account.Key));

                foreach (var obligation in obligations)
                {
                    if (obligation.Value == account.Key)
                    {
                        accountActor.Tell(new AddObligationToAccount(account.Key.ToString(), new Obligation(obligation.Key.ToString())));
                        logger.Tell($"Adding obligation# {obligation.Key} on account {account.Key}");
                    }
                }
                _accounts.Add(new Account(account.Key.ToString()), accountActor);
            }
        }
        public Dictionary<string, string> GetObligationsUnderThisSuperVisor(string obligationsFilePath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                _log.Info($"Gonna try to open file {obligationsFilePath}");
                logger.Tell($"Gonna try to open file {obligationsFilePath}");
                if (File.Exists(obligationsFilePath))
                {
                    string[] readText = File.ReadAllLines(obligationsFilePath);
                    logger.Tell($"{obligationsFilePath} has {readText.Length} lines");
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        dictionary.Add(line[0], line[1]);
                        logger.Tell($"Adding account #{line[0]} to dictionary");
                    }
                }
                _log.Info($"Successfully processing file {obligationsFilePath}");
                logger.Tell($"Successfully processing file {obligationsFilePath}");

            }
            catch (Exception e)
            {
                Self.Tell(new FailedToLoadObligations(e.Message));
            }
            return dictionary;
        }
        private Dictionary<string, string> GetAccountsUnderThisSuperVisor(string clientsFilePath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                _log.Info($"Gonna try to open file {clientsFilePath}");
                logger.Tell($"Gonna try to open file {clientsFilePath}");
                if (File.Exists(clientsFilePath))
                {
                    string[] readText = File.ReadAllLines(clientsFilePath);
                    logger.Tell($"{clientsFilePath} has {readText.Length} lines");
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        dictionary.Add(line[0], line[1]);
                        logger.Tell($"Adding account #{line[0]} to dictionary");
                    }
                }
                _log.Info($"Successfully processing file {clientsFilePath}");
                logger.Tell($"Successfully processing file {clientsFilePath}");

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
    public class FailedToLoadObligations
    {
        public FailedToLoadObligations(string message)
        {
            this.Message = message;
        }
        public string Message { get; private set; }
    }
    public class SuperviorStartUp
    {
        public SuperviorStartUp(string clientAccountsFilePath, string obligationsFilePath)
        {
            ClientAccountFilePath = clientAccountsFilePath;
            ObligationsFilePath = obligationsFilePath;
        }
        public string ClientAccountFilePath { get; private set; }
        public string ObligationsFilePath { get; private set; }
    }
}