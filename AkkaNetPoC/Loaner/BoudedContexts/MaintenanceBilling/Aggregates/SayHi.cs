namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SayHi
    {
        public SayHi(string message)
        {
            Message = message;
        }
        public string Message
        {
            get;
        }
    }
}
