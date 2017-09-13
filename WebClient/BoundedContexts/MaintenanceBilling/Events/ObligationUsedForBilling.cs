using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class ObligationUsedForBilling : IEvent
    {
        public string ObligationNumber { get; private set; }
        public string Message { get; private set; }


        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }

        public ObligationUsedForBilling()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public ObligationUsedForBilling(string obligationNumber, string message) : this()
        {
            ObligationNumber = obligationNumber;
            Message = message;

        }
        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }

    }
}
