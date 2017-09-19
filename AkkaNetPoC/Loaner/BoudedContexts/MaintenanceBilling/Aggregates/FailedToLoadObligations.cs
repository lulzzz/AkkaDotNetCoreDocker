namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FailedToLoadObligations
    {
        public FailedToLoadObligations(string message)
        {
            this.Message = message;
        }
        public string Message { get; private set; }
    }
}