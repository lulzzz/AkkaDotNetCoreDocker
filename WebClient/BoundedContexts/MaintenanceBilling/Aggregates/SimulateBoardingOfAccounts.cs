namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SimulateBoardingOfAccounts
    {
        public SimulateBoardingOfAccounts(string clientName, string clientAccountsFilePath, string obligationsFilePath)
        {
            ClientName = clientName;
            ClientAccountsFilePath = clientAccountsFilePath;
            ObligationsFilePath = obligationsFilePath;
        }

        public string ClientName { get; private set; }
        public string ClientAccountsFilePath { get; private set; }
        public string ObligationsFilePath { get; private set; }
    }
}