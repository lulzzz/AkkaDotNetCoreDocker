using System;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{

    public class AccountCancelled : IEvent
    {
        public AccountStatus AccountStatus { get; private set; }
        public string AccountNumber { get; private set; }
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

        public AccountCancelled()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public AccountCancelled(string accountNumber, AccountStatus status) : this()
        {
            AccountStatus = status;
            AccountNumber = accountNumber;
        }
    }
}
