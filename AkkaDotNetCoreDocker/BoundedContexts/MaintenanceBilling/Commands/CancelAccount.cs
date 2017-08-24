using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands
{
    public class CancelAccount : IDomainCommand
    {
        public CancelAccount()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public CancelAccount(Account account) : this()
        {
            Account = account;
        }

        public DateTime RequestedOn()
        {
            return _RequestedOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }

        public Account Account { get; private set; }
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
    }
}
