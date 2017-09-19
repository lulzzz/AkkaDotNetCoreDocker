using System.Collections.Generic;
using System.Collections.Immutable;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class AnObligationMustBeActiveForBilling : IAccountBusinessRule
    {
        private AccountState AccountState;
        private List<IEvent> EventsGenerated;
        private string DetailsGenerated;
        private ImmutableList<InvoiceLineItem> LineItems;

        public bool Success { get; private set; }

        public AnObligationMustBeActiveForBilling(AccountState accountState, ImmutableList<InvoiceLineItem> lineItems)
        {
            this.AccountState = accountState;
            this.LineItems = lineItems;
        }

        public string GetResultDetails()
        {
            return this.DetailsGenerated;
        }

        public List<IEvent> GetGeneratedEvents()
        {
            return this.EventsGenerated;
        }

        public AccountState GetGeneratedState()
        {
            return this.AccountState;
        }

        /* Rule logic goes here. */
        public void RunRule()
        {
            this.EventsGenerated = new List<IEvent>();
            Obligation obligationToUse = null;
            foreach (var item in AccountState.Obligations.Values.ToImmutableList())
            {
                if (item.Status == ObligationStatus.Active)
                {
                    obligationToUse = item;
                    break;
                }
            }
            if (obligationToUse != null)
            {
                foreach (var item in LineItems)
                {
                    var @event = new ObligationAssessedConcept(obligationToUse.ObligationNumber, item.Item, item.TotalAmount);
                    this.EventsGenerated.Add(@event);

                }

                this.DetailsGenerated = "THIS WORKED";
                this.Success = true;

            }
            else
            {
                this.DetailsGenerated = "No Acttive obligations on this account.";
                this.Success = false;
            }
        }


    }
}
