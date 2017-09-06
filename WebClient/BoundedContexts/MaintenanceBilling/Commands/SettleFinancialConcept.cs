using System;
using System.Collections.Immutable;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands
{
    public class SettleFinancialConcept : IDomainCommand
    {
        public SettleFinancialConcept()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public SettleFinancialConcept(string obligationNumber, FinancialConcept concept, double amount) : this()
        {
            ObligationNumber = obligationNumber;
            FinancialConcept = concept;
            Amount = amount;

        }
        public DateTime RequestedOn()
        {
            return this._RequestedOn;
        }

        public Guid UniqueGuid()
        {
            return this._UniqueGuid;
        }

        public string ObligationNumber { get; private set; }
        public double Amount { get; private set; }
        public FinancialConcept FinancialConcept { get; private set; }

        private DateTime _RequestedOn { get; }
        private Guid _UniqueGuid { get;  }


    }
}
