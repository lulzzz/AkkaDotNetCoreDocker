namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SimulateBoardingOfAccounts
    {
        public SimulateBoardingOfAccounts(string clientAccountsFilePath, string obligationsFilePath)
        {
            ClientAccountFilePath = clientAccountsFilePath;
            ObligationsFilePath = obligationsFilePath;
        }
        public string ClientAccountFilePath { get; private set; }
        public string ObligationsFilePath { get; private set; }
    }
}