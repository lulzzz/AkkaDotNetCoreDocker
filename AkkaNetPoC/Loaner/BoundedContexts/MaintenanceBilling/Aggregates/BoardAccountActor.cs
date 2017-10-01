using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Monitoring;
using Akka.Routing;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    /**
      * We are sumulating the boarding of accounts from scratch. 
     */

    public class BoardAccountActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly Dictionary<string, Dictionary<string, string>> _accountsInPortfolio = new Dictionary<string, Dictionary<string, string>>();
        //private readonly Dictionary<string, string> _accountsInFile = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _obligationsInFile = new Dictionary<string, string>();

        public BoardAccountActor()
        {
            Receive<SimulateBoardingOfAccounts>(client => StartUpHandler(client, client.ClientAccountsFilePath,
                client.ObligationsFilePath));
            Receive<SpinUpAccountActor>(msg => SpinUpAccountActor(msg.Portfolio,msg.AccountNumber, msg.Obligations, msg.Supervisor));

            /* Example of custom error handling, also using messages */
            Receive<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));
            Receive<FailedToLoadObligations>(m => Self.Tell(typeof(Stop)));
            ReceiveAny(msg => _log.Error($"Unhandled message in {Self.Path.Name}. Message:{msg.ToString()}"));
        }

        private void StartUpHandler(SimulateBoardingOfAccounts client, string accountsFilePath,
            string obligationsFilePath)
        {
            var supervisor = Context.ActorSelection("/user/demo-supervisor").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            Monitor();
            var counter = 0;
            _log.Info($"Procesing boarding command... ");

            GetAccountsForClient(accountsFilePath,obligationsFilePath);
            
            var props = new RoundRobinPool(10).Props(Props.Create<BoardAccountActor>());
            var router = Context.ActorOf(props, $"Client-{client.ClientName}-router");

            foreach (var portfolioDic in _accountsInPortfolio)
            {
                var portfolio = portfolioDic.Key;
                var accounts = portfolioDic.Value;

                foreach (var account in accounts)
                {
                    var obligations = ImmutableDictionary.Create<string, string>();
                    //Pluck out all the obligations for this account
                    foreach (var item in _obligationsInFile)
                    {
                        if (item.Value == account.Key)
                        {
                            obligations = obligations.Add(item.Key, item.Value);
                        }

                    }
                    if (++counter % 1000 == 0)
                    {
                        _log.Info($"({counter}) Telling router {router.Path.Name} to spin up account {account.Key}... ");
                    }
                    //Then spint up the account
                    router.Tell(new SpinUpAccountActor(portfolio,account.Key, obligations.ToImmutableDictionary(), supervisor));
                }
            }
        }

        private void SpinUpAccountActor(string portfolio, string accountNumber, ImmutableDictionary<string, string> obligations,
            IActorRef supervisor)
        {
            Monitor();
            var props = Props.Create<AccountActor>();
            var accountActor = Context.ActorOf(props, accountNumber);
            accountActor.Tell(new CreateAccount(accountNumber));

            if (obligations != null && obligations.ContainsValue(accountNumber))
            {
                foreach (var obligation in obligations)
                {
                    if (obligation.Value == accountNumber)
                    {
                        var obli = new Obligation(obligation.Key);
                        /* maybe messing with business logic belongs in the reader? */
                        obli = obli.SetStatus(ObligationStatus.Active);
                        accountActor.Tell(new AddObligationToAccount(obligation.Value, obli));
                    }
                }
            }
            accountActor.Tell(new AskToBeSupervised(portfolio,supervisor));
        }

        /* Auxiliary methods */
        public void GetObligationsForClient(string obligationsFilePath)
        {
            try
            {
                _log.Info($"Gonna try to open file {obligationsFilePath}");
                if (File.Exists(obligationsFilePath))
                {
                    var readText = File.ReadAllLines(obligationsFilePath, Encoding.UTF8);
                    foreach (var row in readText)
                    {
                        if (row.Length > 11)
                        {
                            var line = row.Split('\t');
                            _obligationsInFile.Add(line[0], line[1]);
                        }
                    }
                }
                _log.Info($"Successfully processing file {obligationsFilePath}");
            }
            catch (Exception e)
            {
                Sender.Tell(new FailedToLoadObligations(e.Message));
            }
        }

        private void GetAccountsForClient(string clientsFilePath, string obligationsFilePath)
        {
            try
            {
                _log.Info($"Gonna try to open file {clientsFilePath}");
                if (File.Exists(clientsFilePath))
                {
                    var readText = File.ReadAllLines(clientsFilePath, Encoding.UTF8);
                    foreach (var row in readText)
                    {
                        if (row.Length > 11)
                        {
                            var line = row.Split('\t');
                            if (_accountsInPortfolio.ContainsKey(line[0]))
                            {
                                Dictionary<string,string> existingAccounts = _accountsInPortfolio[line[0]];
                                existingAccounts.Add(line[0], line[1]);
                                _accountsInPortfolio[line[0]] = existingAccounts;
                            }
                            else
                            {
                                Dictionary<string,string> accounts = new Dictionary<string,string>();
                                accounts.Add(line[0], line[1]);
                                _accountsInPortfolio.Add(line[0], accounts);
                            }
                        }
                    }
                }
                _log.Info($"Successfully processing file {clientsFilePath}");
            }
            catch (Exception e)
            {
                Sender.Tell(new FailedToLoadAccounts(e.Message));
            }

            GetObligationsForClient(obligationsFilePath);

        }

        private void Monitor()
        {
            Context.IncrementMessagesReceived();
        }

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
        }

        protected override void PreStart()
        {
            Context.IncrementActorCreated();
        }
    }


    internal class SpinUpAccountActor
    {
        public SpinUpAccountActor(string portfolio, string accountNumber, ImmutableDictionary<string, string> oligations,
            IActorRef supervisor)
        {
            Portfolio = portfolio;
            AccountNumber = accountNumber;
            Obligations = ImmutableDictionary.Create<string, string>();
            Obligations = oligations.ToImmutableDictionary();
            Supervisor = supervisor;
        }
        public string Portfolio { get; }
        public string AccountNumber { get; }
        public ImmutableDictionary<string, string> Obligations { get; }
        public IActorRef Supervisor { get; }
    }
}