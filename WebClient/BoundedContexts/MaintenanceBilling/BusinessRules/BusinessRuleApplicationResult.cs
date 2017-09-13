﻿using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules
{
    public class BusinessRuleApplicationResult
    {
        public BusinessRuleApplicationResult()
        {
            RuleProcessedResults = new Dictionary<IAccountBusinessRule, string>();
            GeneratedEvents = new List<IEvent>();
            GeneratedState = new AccountState();
            Success = false;
        }
       
        public bool Success { get; set; }
        public AccountState GeneratedState { get; set; }
        public List<IEvent> GeneratedEvents { get; set; }
        public Dictionary<IAccountBusinessRule, string> RuleProcessedResults { get; set; }
    }
}