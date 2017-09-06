using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class ObligationSettledConcept : IEvent
    {
        public string ObligationNumber { get; private set; }
        public FinancialConcept FinancialConcept { get; private set; }
        public double Amount { get; private set; }
        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }

        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }

        public ObligationSettledConcept()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public ObligationSettledConcept(string obligationNumber, FinancialConcept concept, double amount) : this()
        {
            ObligationNumber = obligationNumber;
            FinancialConcept = concept;
            Amount = amount;

        }
    }
}
