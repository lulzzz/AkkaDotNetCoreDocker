using System;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SaveAccountToSupervisorState : IDomainCommand
    {
        public SaveAccountToSupervisorState()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public SaveAccountToSupervisorState(string accountNumber) : this()
        {
            AccountNumber = accountNumber;
        }

        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
        public string AccountNumber { get; }

        public DateTime RequestedOn()
        {
            return _RequestedOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }
    }
}