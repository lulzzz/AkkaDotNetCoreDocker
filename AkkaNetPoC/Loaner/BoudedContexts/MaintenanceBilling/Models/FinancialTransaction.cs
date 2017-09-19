using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class FinancialTransaction : ITransaction
    {

        public FinancialTransaction()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public FinancialTransaction(FinancialConcept concept, double amount) : this()
        {
            FinancialConcept = concept;
            TransactionAmount = amount;
        }

        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }

        public double TransactionAmount { get; private set; }
        public FinancialConcept FinancialConcept { get; private set; }
        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }

    }
}
