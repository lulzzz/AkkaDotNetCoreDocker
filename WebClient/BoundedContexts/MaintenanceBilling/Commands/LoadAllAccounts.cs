using System;
using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class LoadAllAccounts : IDomainCommand
    {
        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
        public List<string> AccountNumbers { get; }

        public LoadAllAccounts(){
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
            AccountNumbers = new List<string>();
        }
        public LoadAllAccounts(List<string> accountsList):this()
        {
            AccountNumbers = accountsList;  
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