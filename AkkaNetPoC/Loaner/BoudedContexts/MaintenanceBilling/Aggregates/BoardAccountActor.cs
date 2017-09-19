//
// BoardAccountActor.cs
//
// Author:
//       Alfredo Herrera <alfredherr@gmail.com>
//
// Copyright (c) 2017 Alfrdo Herrera
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Monitoring;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling
{
    /**
      * We are sumulating the boarding of accounts from scratch. 
     */

    public class BoardAccountActor : ReceiveActor
    {
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        Dictionary<string, IActorRef> AccountsBeingBoarded = new Dictionary<string, IActorRef>();
        private Dictionary<string, string> AccountsInFile = new Dictionary<string, string>();
        private Dictionary<string, string> ObligationsInFile = new Dictionary<string, string>();

        public BoardAccountActor()
        {
            Receive<SimulateBoardingOfAccounts>(client => this.StartUpHandler(client, client.ClientAccountsFilePath, client.ObligationsFilePath));
            Receive<SpinUpAccountActor>(msg => this.SpinUpAccountActor(msg.AccountNumber, msg.Obligations, msg.Supervisor));

            /* Example of custom error handling, also using messages */
            Receive<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));
            Receive<FailedToLoadObligations>(m => Self.Tell(typeof(Stop)));
            ReceiveAny(msg => _log.Error($"Unhandled message in {Self.Path.Name}. Message:{msg.ToString()}"));
        }

        private void StartUpHandler(SimulateBoardingOfAccounts client, string accountsFilePath, string obligationsFilePath)
        {
            var supervisor = Sender;
            this.Monitor();
            int counter = 0;
            _log.Info($"Procesing boarding command... ");

            GetAccountsForClient(accountsFilePath);
            GetObligationsForClient(obligationsFilePath);

            var props = new Akka.Routing.RoundRobinPool(200).Props(Props.Create<BoardAccountActor>());
            var router = Context.ActorOf(props, name: $"Client-{client.ClientName}-router");

            foreach (var account in AccountsInFile)
            {
                ImmutableDictionary<string, string> obligations = ImmutableDictionary.Create<string, string>();
                foreach (var item in this.ObligationsInFile)
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
                router.Tell(new SpinUpAccountActor(account.Key, obligations.ToImmutableDictionary(),supervisor));
            }
        }

        private void SpinUpAccountActor(string accountNumber, ImmutableDictionary<string, string> obligations,IActorRef supervisor)
        {
            this.Monitor();
            var props = Props.Create<AccountActor>();
            var accountActor = Context.ActorOf(props, name: accountNumber);
            accountActor.Tell(new CreateAccount(accountNumber));

            if (obligations.ContainsValue(accountNumber))
            {
                foreach (var obligation in obligations)
                {
                    if (obligation.Value == accountNumber)
                    {
                        var o = new Obligation(obligation.Key);
                        /* maybe messing with the  belongs in the reader? */
                        o = o.SetStatus(ObligationStatus.Active); 
                        accountActor.Tell(new AddObligationToAccount(obligation.Value, o));
                    }
                }
            }
            accountActor.Tell(new AskToBeSupervised(supervisor));

        }

        /* Auxiliary methods */
        public void GetObligationsForClient(string obligationsFilePath)
        {
            try
            {
                _log.Info($"Gonna try to open file {obligationsFilePath}");
                if (File.Exists(obligationsFilePath))
                {
                    string[] readText = File.ReadAllLines(obligationsFilePath);
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        ObligationsInFile.Add(line[0], line[1]);
                    }
                }
                _log.Info($"Successfully processing file {obligationsFilePath}");
            }
            catch (Exception e)
            {
                Sender.Tell(new FailedToLoadObligations(e.Message));
            }

        }

        private void GetAccountsForClient(string clientsFilePath)
        {
            try
            {
                _log.Info($"Gonna try to open file {clientsFilePath}");
                if (File.Exists(clientsFilePath))
                {
                    string[] readText = File.ReadAllLines(clientsFilePath);
                    foreach (var row in readText)
                    {
                        var line = row.Split('\t');
                        AccountsInFile.Add(line[0], line[1]);
                    }
                }
                _log.Info($"Successfully processing file {clientsFilePath}");
            }
            catch (Exception e)
            {
                Sender.Tell(new FailedToLoadAccounts(e.Message));
            }
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
        public SpinUpAccountActor(string accountNumber, ImmutableDictionary<string, string> oligations,IActorRef supervisor)
        {
            AccountNumber = accountNumber;
            Obligations = ImmutableDictionary.Create<string, string>();
            Obligations = oligations.ToImmutableDictionary();
            Supervisor = supervisor;
        }

        public string AccountNumber { get; private set; }
        public ImmutableDictionary<string, string> Obligations { get; private set; }
        public IActorRef Supervisor { get; private set; }
    }
}
