using System;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ObligationAssessedConcept : IEvent
    {
      
        public ObligationAssessedConcept(string obligationNumber, FinancialConcept concept, double amount, Guid id, DateTime when)  
        {
            ObligationNumber = obligationNumber;
            FinancialConcept = concept;
            Amount = amount;
            uniqueGuid = id;
            occurredOn = when;
        }

        public string ObligationNumber { get; }
        public FinancialConcept FinancialConcept { get; }
        public double Amount { get; }
        public DateTime occurredOn { get; }
        public Guid uniqueGuid { get;  }

        public DateTime OccurredOn()
        {
            return occurredOn;
        }

        public Guid UniqueGuid()
        {
            return uniqueGuid;
        }
    }
}