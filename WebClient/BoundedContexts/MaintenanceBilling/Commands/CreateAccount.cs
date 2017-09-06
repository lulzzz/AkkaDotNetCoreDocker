using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands
{
    public class CreateAccount : IDomainCommand
    {
        public CreateAccount(string accountNumber)
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
            AccountNumber = accountNumber;
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
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
    }
}
