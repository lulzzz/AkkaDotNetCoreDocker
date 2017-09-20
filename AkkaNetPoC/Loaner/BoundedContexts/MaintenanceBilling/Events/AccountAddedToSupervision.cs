using System;
using Loaner.BoundedContexts.MaintenanceBilling.Events;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    internal class AccountAddedToSupervision : IEvent
    {
        public AccountAddedToSupervision()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            AccountNumber = "";
        }

        public AccountAddedToSupervision(string accountNumber) : this()
        {
            AccountNumber = accountNumber;
        }

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