using System;
using System.Collections.Immutable;

namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class Obligation
    {
        public ObligationStatus Status { get; private set; }
        private ImmutableDictionary<Guid, FinancialTransaction> Transactions { get; }
        private ImmutableDictionary<FinancialConcept, double> Buckets { get; }
        public string ObligationNumber { get; }
        private double CurrentBalance { get; set; }
        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }
        

        public Obligation(string obligationNumber)  
        {
            ObligationNumber = obligationNumber;
            Status = ObligationStatus.Active;
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            Transactions = ImmutableDictionary.Create<Guid, FinancialTransaction>();
            Buckets = ImmutableDictionary.Create<FinancialConcept, double>();
        }

        public Obligation(string obligationNumber, ObligationStatus status)  
        {
            ObligationNumber = obligationNumber;
            Status = status;
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            Transactions = ImmutableDictionary.Create<Guid, FinancialTransaction>();
            Buckets = ImmutableDictionary.Create<FinancialConcept, double>();
        }
        public ImmutableDictionary<FinancialConcept, double> PostTransaction(FinancialTransaction occurred)
        {
            Transactions.Add(new Guid(), occurred);
            return UpdateBuckets(occurred);
        }

        public ImmutableDictionary<Guid, FinancialTransaction> GetTransactions()
        {
            return Transactions;
        }

        public ImmutableDictionary<FinancialConcept, double> GetBucketBalances()
        {
            return Buckets;
        }

        public ImmutableDictionary<FinancialConcept, double> UpdateBuckets(FinancialTransaction occurred)
        {
            return Buckets.SetItem(occurred.FinancialConcept, occurred.TransactionAmount);
        }

        public Obligation SetStatus(ObligationStatus status)
        {
            Status = status;
            return this;
        }

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