using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class AnObligationMustBeActiveForBilling : IAccountBusinessRule
    {
        private AccountState AccountState;
        private List<IEvent> EventsGenerated;
        private string DetailsGenerated;
        private ImmutableList<InvoiceLineItem> LineItems;

        public bool Success { get; internal set; }

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
            foreach (var item in AccountState.Obligations)
            {
                if (item.Value.Status == ObligationStatus.Active)
                {
                    obligationToUse = item.Value;

                }
            }
            if (obligationToUse != null)
            {
                this.EventsGenerated
                       .Add(new ObligationUsedForBilling(
                            obligationNumber: obligationToUse.ObligationNumber,
                            message: "Active obligation used for billing"
                       ));
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
