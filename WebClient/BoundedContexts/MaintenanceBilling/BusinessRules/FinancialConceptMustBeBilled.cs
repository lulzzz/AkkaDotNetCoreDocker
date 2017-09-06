using System;
using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FinancialConceptMustBeBilled : IBusinessRule
    {
        public FinancialConceptMustBeBilled(){
            
            
        }

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