using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public interface IAccountBusinessRule
    {
        /**
         * Rule logic gets handled in this method.
         */
        void RunRule();

        string GetResultDetails();

        /** 
         * The purpose of a rule is to return a list of orderd events which must be 
         * applied to the account 
        */
        List<IEvent> GetGeneratedEvents();

        /**
         * This is what the resulting AccountState would be from all the applied
         * events -- it can be used to allow comparisons, or for downstream rules
         * perhaps. Note that this its the calles choice to accept the events or not.
         */
        AccountState GetGeneratedState();
    }
}
