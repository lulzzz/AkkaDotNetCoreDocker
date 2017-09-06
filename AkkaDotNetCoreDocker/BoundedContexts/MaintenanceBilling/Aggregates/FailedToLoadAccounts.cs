namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FailedToLoadAccounts
    {
        public FailedToLoadAccounts(string message)
        {
            this.Message = message;
        }
        public string Message { get; private set; }
    }
}