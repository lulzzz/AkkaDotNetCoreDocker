using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class BusinessRuleApplicationResult
    {
        public BusinessRuleApplicationResult()
        {

        }
        public BusinessRuleApplicationResult(bool success, AccountState state, List<IEvent> events)
        {
            Success = false;
            GeneratedState = state;
            GeneratedEvents = events;
        }
        public bool Success { get; set; }
        public AccountState GeneratedState { get; set; }
        public List<IEvent> GeneratedEvents { get; set; }
        public Dictionary<IBusinessRule, string> RuleProcessedResults { get; set; }
    }
}