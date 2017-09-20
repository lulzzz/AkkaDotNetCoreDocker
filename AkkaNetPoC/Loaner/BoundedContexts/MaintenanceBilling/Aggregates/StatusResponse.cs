namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class StatusResponse
    {
        private readonly string Message;

        public StatusResponse(string message)
        {
            Message = message;
        }
    }
}