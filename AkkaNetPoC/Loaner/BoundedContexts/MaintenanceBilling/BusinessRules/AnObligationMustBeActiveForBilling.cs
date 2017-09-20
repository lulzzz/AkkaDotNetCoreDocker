using System.Collections.Generic;
using System.Collections.Immutable;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class AnObligationMustBeActiveForBilling : IAccountBusinessRule
    {
        private readonly AccountState AccountState;
        private string DetailsGenerated;
        private List<IEvent> EventsGenerated;
        private readonly ImmutableList<InvoiceLineItem> LineItems;

        public AnObligationMustBeActiveForBilling(AccountState accountState, ImmutableList<InvoiceLineItem> lineItems)
        {
            AccountState = accountState;
            LineItems = lineItems;
        }

        public bool Success { get; private set; }

        public string GetResultDetails()
        {
            return DetailsGenerated;
        }

        public List<IEvent> GetGeneratedEvents()
        {
            return EventsGenerated;
        }

        public AccountState GetGeneratedState()
        {
            return AccountState;
        }

        /* Rule logic goes here. */
        public void RunRule()
        {
            EventsGenerated = new List<IEvent>();
            Obligation obligationToUse = null;
            foreach (var item in AccountState.Obligations.Values.ToImmutableList())
                if (item.Status == ObligationStatus.Active)
                {
                    obligationToUse = item;
                    break;
                }
            if (obligationToUse != null)
            {
                foreach (var item in LineItems)
                {
                    var @event =
                        new ObligationAssessedConcept(obligationToUse.ObligationNumber, item.Item, item.TotalAmount);
                    EventsGenerated.Add(@event);
                }

                DetailsGenerated = "THIS WORKED";
                Success = true;
            }
            else
            {
                DetailsGenerated = "No Acttive obligations on this account.";
                Success = false;
            }
        }
    }
}