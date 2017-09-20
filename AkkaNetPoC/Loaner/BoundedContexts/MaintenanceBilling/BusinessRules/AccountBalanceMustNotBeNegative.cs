using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;
using Loaner.BoundedContexts.MaintenanceBilling.Events;

namespace Loaner.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class AccountBalanceMustNotBeNegative : IAccountBusinessRule
    {
        private readonly AccountState accountState;
        private string detailsGenerated;
        private List<IEvent> eventsGenerated;

        public AccountBalanceMustNotBeNegative(AccountState accountState)
        {
            this.accountState = accountState;
        }

        public bool Success { get; internal set; }

        public string GetResultDetails()
        {
            return detailsGenerated;
        }

        public List<IEvent> GetGeneratedEvents()
        {
            return eventsGenerated;
        }

        public AccountState GetGeneratedState()
        {
            return accountState;
        }

        /* Rule logic goes here. */
        public void RunRule()
        {
            eventsGenerated = new List<IEvent>();
            eventsGenerated.Add(new SuperSimpleSuperCoolEventFoundByRules(
                accountState.AccountNumber,
                "AccountBalanceMustNotBeNegative"
            ));
            detailsGenerated = "THIS WORKED";
            Success = true;
        }
    }
}