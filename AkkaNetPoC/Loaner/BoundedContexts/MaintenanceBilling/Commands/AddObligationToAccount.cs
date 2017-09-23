using System;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Commands
{
    public class AddObligationToAccount : IDomainCommand
    {
      
        public AddObligationToAccount(string accountNumber, Obligation obligation)  
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
            AccountNumber = accountNumber;
            Obligation = obligation;
        }

        public string AccountNumber { get; }
        public Obligation Obligation { get; }
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }

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