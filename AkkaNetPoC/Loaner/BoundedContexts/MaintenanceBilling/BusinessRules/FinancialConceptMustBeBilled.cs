using System;
using System.Collections.Generic;
using Loaner.BoundedContexts.MaintenanceBilling.BusinessRules;
using Loaner.BoundedContexts.MaintenanceBilling.Events;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FinancialConceptMustBeBilled : IAccountBusinessRule
    {
        public List<IEvent> GetGeneratedEvents()
        {
            throw new NotImplementedException();
        }

        public AccountState GetGeneratedState()
        {
            throw new NotImplementedException();
        }

        public string GetResultDetails()
        {
            throw new NotImplementedException();
        }

        public void RunRule()
        {
            throw new NotImplementedException();
        }
    }
}