using System;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class ObligationSettledConcept : IEvent
    {
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

        public string ObligationNumber { get; }
        public FinancialConcept FinancialConcept { get; }
        public double Amount { get; }
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
    }
}