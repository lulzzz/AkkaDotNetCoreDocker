using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections;
using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models
{
    public class Obligation
    {
        
        public Obligation()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            ObligationNumber = new Guid().ToString();
        }
        public Obligation(string obligationNumber) : this()
        {
            this.ObligationNumber = obligationNumber;
        }

        public void PostTransaction(FinancialTransaction occurred)
        {
            this.Transactions.Add(new Guid(), occurred);
        }


        public ImmutableDictionary<Guid, FinancialTransaction> Transactions { get; private set; }


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
