using System;
using System.Collections.Immutable;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Commands
{
    public class BillingAssessment : IDomainCommand
    {
        public BillingAssessment()
        {
            _RequestedOn = DateTime.Now;
            _UniqueGuid = Guid.NewGuid();
        }

        public BillingAssessment(string accountNumber, ImmutableList<InvoiceLineItem> lineItems) : this()
        {
            AccountNumber = accountNumber;
            LineItems = lineItems;

        }
        public DateTime RequestedOn()
        {
            return this._RequestedOn;
        }

        public Guid UniqueGuid()
        {
            return this._UniqueGuid;
        }

        
        private DateTime _RequestedOn { get; }
        private Guid _UniqueGuid { get; }
        public string AccountNumber { get; private set; }
        public ImmutableList<InvoiceLineItem> LineItems { get; private set; }
    }
}
