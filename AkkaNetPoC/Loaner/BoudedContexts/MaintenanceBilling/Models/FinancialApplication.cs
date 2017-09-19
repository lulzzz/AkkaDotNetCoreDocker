using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class FinancialApplication : ITransaction
    {
        public FinancialApplication() { }

        public FinancialConcept Bucket { get; private set; }
        public double AppliedAmount { get; private set; }
        public double BalanceBefore { get; private set; }
        public double BalanceAfter { get; private set; }

        public DateTime OccurredOn()
        {
            throw new NotImplementedException();
        }

        public Guid UniqueGuid()
        {
            throw new NotImplementedException();
        }
    }
}
