using System;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ObligationAddedToAccount : IEvent
    {
        
        public ObligationAddedToAccount(string accountNumber, Obligation obligation)  
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            Obligation = obligation;
            AccountNumber = accountNumber;
        }

        public Obligation Obligation { get; }
        public string AccountNumber { get; }
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