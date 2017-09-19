using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;
using Loaner.BoundedContexts.MaintenanceBilling.Events;

namespace Loaner.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class AccountBalanceMustNotBeNegative : IAccountBusinessRule
    {
        private AccountState accountState;
        private List<IEvent> eventsGenerated;
        private string detailsGenerated;

        public bool Success { get; internal set; }

        public AccountBalanceMustNotBeNegative(AccountState accountState)
        {
            this.accountState = accountState;
        }

        public string GetResultDetails()
        {
            return this.detailsGenerated;
        }

        public List<IEvent> GetGeneratedEvents()
        {
            return this.eventsGenerated;
        }

        public AccountState GetGeneratedState()
        {
            return this.accountState;
        }

        /* Rule logic goes here. */
        public void RunRule()
        {
            this.eventsGenerated = new List<IEvent>();
            this.eventsGenerated.Add(new SuperSimpleSuperCoolEventFoundByRules(
                this.accountState.AccountNumber,
                "AccountBalanceMustNotBeNegative"
            ));
            this.detailsGenerated = "THIS WORKED";
            this.Success = true;
        }


    }
}
