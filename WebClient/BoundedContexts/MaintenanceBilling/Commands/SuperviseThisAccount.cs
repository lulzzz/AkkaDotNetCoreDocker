using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SuperviseThisAccount : IDomainCommand
    {
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
        public Account Account { get; }

        public SuperviseThisAccount()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }
        public SuperviseThisAccount(Account account) : this()
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
    }
}