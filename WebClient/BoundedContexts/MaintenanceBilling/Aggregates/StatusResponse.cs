namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class StatusResponse
    {
        readonly string Message;

        public StatusResponse(string message)
        {
            Message = message;
        }
    }
}