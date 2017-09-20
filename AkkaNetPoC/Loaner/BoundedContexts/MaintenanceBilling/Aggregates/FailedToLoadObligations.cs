namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FailedToLoadObligations
    {
        public FailedToLoadObligations(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}