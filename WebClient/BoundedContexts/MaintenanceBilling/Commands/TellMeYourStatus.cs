namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class TellMeYourStatus
    {

        public string Message { get; }
        public TellMeYourStatus(){
            
        }
        public TellMeYourStatus(string message)
        {
            Message = message;
        }
    }
}