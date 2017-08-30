using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands
{
    public class AddObligationToAccount : IDomainCommand
    {
        public AddObligationToAccount()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public AddObligationToAccount(string accountNumber, Obligation obligation) : this()
        {
            AccountNumber = accountNumber;
            Obligation = obligation;
        }

        public DateTime RequestedOn()
        {
            return _RequestedOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }

        public string AccountNumber { get; private set; }
        public Obligation Obligation { get; private set; }
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
    }
}
