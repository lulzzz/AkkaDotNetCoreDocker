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
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        /* Actor's state */
        Dictionary<string, IActorRef> _accounts = new Dictionary<string, IActorRef>();

        public AccountActorSupervisor()
        {
            /* Messages this actor understands */
            Receive<AboutMe>(me => Console.WriteLine($"About me: {me.Me}"));
            Receive<SuperviorStartUp>(client =>
            {
                _log.Debug($"Client: {client.ClientAccountFilePath}");
                StartUp(client, client.ClientAccountFilePath, client.ObligationsFilePath);
            });
            Receive<TellMeYourStatus>(asking => Sender.Tell(new TellMeYourStatus($"{Self.Path.Name} I am alive! I have {_accounts.Count} accounts.")));

            /* Example of custom error handling, also using messages */
            Receive<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));
            Receive<FailedToLoadObligations>(m => Self.Tell(typeof(Stop)));
        }

        /**
         * We are sumulating the boarding of accounts from scratch. 
        */
        private void StartUp(SuperviorStartUp client, string accountsFilePath, string obligationsFilePath)
        {
            var accounts = GetAccountsUnderThisSuperVisor(accountsFilePath);
            var obligations = GetAccountsUnderThisSuperVisor(obligationsFilePath);


            foreach (var account in accounts)
            {
                _log.Debug($"Processing Account: {account.Key}");
                if (!_accounts.ContainsKey(account.Key))
                {
                    _log.Debug($"Account {account.Key} isn't in our registry, processing it...");

                    var accountActor = Context.ActorOf<AccountActor>(name: account.Key);
                    accountActor.Tell(new CreateAccount(account.Key));

                    foreach (var obligation in obligations)
                    {
                        if (obligation.Value == account.Key)
                        {
                            accountActor.Tell(new AddObligationToAccount(account.Key, new Obligation(obligation.Key)));
                        }
                    }

                    accountActor.Tell(new SayHi($"Hello {account.Key}"));

                    _accounts.Add(account.Key.ToString(), accountActor);
                    _log.Debug($"/Account {account.Key} ... done.");
                }
                else
                {
                    _log.Debug($"{account.Key} already registered. No action taken.");
                }
            }
        }

        /* Auxiliary methods */
        public Dictionary<string, string> GetObligationsUnderThisSuperVisor(string obligationsFilePath)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                _log.Debug($"Gonna try to open file {obligationsFilePath}");
                if (File.Exists(obligationsFilePath))
                {
                    string[] readText = File.ReadAllLines(obligationsFilePath);
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        dictionary.Add(line[0], line[1]);
                    }
                }
                _log.Debug($"Successfully processing file {obligationsFilePath}");
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
                _log.Debug($"Gonna try to open file {clientsFilePath}");
                if (File.Exists(clientsFilePath))
                {
                    string[] readText = File.ReadAllLines(clientsFilePath);
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        dictionary.Add(line[0], line[1]);
                    }
                }
                _log.Debug($"Successfully processing file {clientsFilePath}");
            }
            catch (Exception e)
            {
                Self.Tell(new FailedToLoadAccounts(e.Message));
            }
            return dictionary;
        }
    } 
}