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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling
{
    /**
      * We are sumulating the boarding of accounts from scratch. 
     */
    public class BoardAccountActor : ReceiveActor
    {
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);


        public BoardAccountActor()
        {
            Receive<SimulateBoardingOfAccounts>(client => StartUpHandler(client, client.ClientAccountFilePath, client.ObligationsFilePath));
        }


        private void StartUpHandler(SimulateBoardingOfAccounts client, string accountsFilePath, string obligationsFilePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            int counter = 0;
            var accounts = GetAccountsForClient(accountsFilePath);
            var obligations = GetObligationsForClient(obligationsFilePath);
            foreach (var account in accounts)
            {

                var domainAccount = new Account(account.Key);
                /**
                  * This is best done with a queue at the supervisor perhaps 
                  * so we're not blocking and waiting... but for this demo it's just fine.
                  */
                TheReferenceToThisActor reference = Sender.Ask<TheReferenceToThisActor>(new SuperviseThisAccount(domainAccount)).Result;
                if ((counter++ % 100) == 0)
                {
                    _log.Info($"Boarded {counter} accounts. It's been boarding accounts for {stopwatch.ElapsedMilliseconds / 1000} minutes.");
                }
                foreach (var obligation in obligations)
                {
                    if (obligation.Value == account.Key)
                    {
                        reference.address.Tell(new AddObligationToAccount(account.Key, new Obligation(obligation.Key)));
                    }
                }
                stopwatch.Stop();

                _log.Debug($"Boarded account {account.Key} ... done.");
            }
        }



        /* Auxiliary methods */
        public Dictionary<string, string> GetObligationsForClient(string obligationsFilePath)
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
                Sender.Tell(new FailedToLoadObligations(e.Message));
            }
            return dictionary;
        }

        private Dictionary<string, string> GetAccountsForClient(string clientsFilePath)
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
                Sender.Tell(new FailedToLoadAccounts(e.Message));
            }
            return dictionary;
        }

    }
}
