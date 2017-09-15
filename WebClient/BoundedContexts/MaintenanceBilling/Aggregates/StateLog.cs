using System;
namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class StateLog
    {
        public StateLog(string eventName, Guid id, DateTime date){
            EventName = eventName;
            EventId = id;
            EventDate = date;
        }
        public Guid EventId { get; private set; }
        public DateTime EventDate { get; private set; }
        public string EventName { get; private set; }
    }
}