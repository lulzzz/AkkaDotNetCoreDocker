namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FailedToLoadAccounts
    {
        public FailedToLoadAccounts(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}