namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
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