using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ObligationUsedForBilling : IEvent
    {
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

        public string ObligationNumber { get; }
        public string Message { get; }
        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }


        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }
    }
}