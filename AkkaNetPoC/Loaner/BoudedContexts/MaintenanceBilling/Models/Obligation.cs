﻿using System;
using System.Collections.Immutable;

namespace Loaner.BoundedContexts.MaintenanceBilling.Models
{
    public class Obligation
    {

        public Obligation()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            ObligationNumber = new Guid().ToString();
            Transactions = ImmutableDictionary.Create<Guid, FinancialTransaction>();
            Buckets = ImmutableDictionary.Create<FinancialConcept, double>();
        }
        public Obligation(string obligationNumber) : this()
        {
            this.ObligationNumber = obligationNumber;
            this.Status = ObligationStatus.Active;
        }
        public Obligation(string obligationNumber,ObligationStatus status) : this()
        {
            this.ObligationNumber = obligationNumber;
            this.Status = status;
        }
        public ImmutableDictionary<FinancialConcept, double> PostTransaction(FinancialTransaction occurred)
        {
            this.Transactions.Add(new Guid(), occurred);
            return UpdateBuckets(occurred);
        }
        public ImmutableDictionary<Guid, FinancialTransaction> GetTransactions()
        {
            return this.Transactions;
        }

        public ImmutableDictionary<FinancialConcept, double> GetBucketBalances()
        {
            return this.Buckets;
        }
        public ImmutableDictionary<FinancialConcept, double> UpdateBuckets(FinancialTransaction occurred)
        {
            return Buckets.SetItem(occurred.FinancialConcept, occurred.TransactionAmount);
        }
        public Obligation SetStatus(ObligationStatus status){
            this.Status = status;
            return this;
        }
        public ObligationStatus Status { get; private set; }
        private ImmutableDictionary<Guid, FinancialTransaction> Transactions { get; set; }
        private ImmutableDictionary<FinancialConcept, double> Buckets { get; set; }
        public string ObligationNumber { get; private set; }
        double CurrentBalance { get; set; }
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
