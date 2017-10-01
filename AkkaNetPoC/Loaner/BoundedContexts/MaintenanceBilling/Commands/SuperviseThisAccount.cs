using System;
using Akka.Routing;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class SuperviseThisAccount : IDomainCommand ,  IConsistentHashable
    {
        public SuperviseThisAccount()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public SuperviseThisAccount(string portfolio, string accountNumber) : this()
        {
            AccountNumber = accountNumber;
            Portfolio = portfolio;
        }

        private Guid _UniqueGuid { get; }
        private DateTime _RequestedOn { get; }
        public string AccountNumber { get; }
        public string Portfolio { get; }

        object IConsistentHashable.ConsistentHashKey => Portfolio;

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