using System;
using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class LoadAllAccounts : IDomainCommand
    {
        public LoadAllAccounts()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
            AccountNumbers = new List<string>();
        }

        public LoadAllAccounts(List<string> accountsList) : this()
        {
            AccountNumbers = accountsList;
        }

        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
        public List<string> AccountNumbers { get; }

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