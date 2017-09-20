using System;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class ObligationAddedToAccount : IEvent
    {
        public ObligationAddedToAccount()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public ObligationAddedToAccount(string accountNumber, Obligation obligation) : this()
        {
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