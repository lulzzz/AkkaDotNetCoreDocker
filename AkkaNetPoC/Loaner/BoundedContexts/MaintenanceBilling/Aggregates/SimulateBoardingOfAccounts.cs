﻿using Akka.Routing;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SimulateBoardingOfAccounts : IConsistentHashable
    {
        public SimulateBoardingOfAccounts(string clientName, string clientAccountsFilePath, string obligationsFilePath)
        {
            ClientName = clientName;
            ClientAccountsFilePath = clientAccountsFilePath;
            ObligationsFilePath = obligationsFilePath;
        }

        public string ClientName { get; }
        public string ClientAccountsFilePath { get; }
        public string ObligationsFilePath { get; }
        public object ConsistentHashKey => ClientName;
    }
}