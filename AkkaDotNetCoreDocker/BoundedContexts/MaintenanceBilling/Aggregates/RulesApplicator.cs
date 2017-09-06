using System.Collections.Generic;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class RulesApplicator 
    {
         
        public static BusinessRuleApplicationResult ApplyBusinessRules(AccountState accountState, IDomainCommand comnd)
        {
            var rules = GetBusinessRulesToApply(accountState, comnd);
            var result = new BusinessRuleApplicationResult();

            foreach (var reglaDeNegocio in rules)
			{
				switch (reglaDeNegocio)
				{
                    case AccountBalanceMustNotBeNegative rule:
                        rule.RunRule();
                        if(rule.Success){
							result.RuleProcessedResults.Add(rule,$"Business Rule Applied. {rule.GetResultDetails()}");
                            rule.GetGeneratedEvents().ForEach( @event => result.GeneratedEvents.Add(@event));
                            result.GeneratedState = rule.GetGeneratedState();
                        }else{
                            result.Success = false;
                            result.RuleProcessedResults.Add(rule, $"Business Rule Failed Application. {rule.GetResultDetails()}");
                            return result; //we stop processing any further rules.
                        }
						break;
					default:
						throw new UnknownBusinessRule();
				}

			}
            //for each rule in rules
            // pass the info to the rule and call rule
            // create event of result of calling rule & apply event to state
            // return new state
            return result;
        }

        public static List<IBusinessRule> GetBusinessRulesToApply(AccountState accountState, IDomainCommand @command)
        {
            //When Event is a SettleFinancialConcept?orWhatever command
            // get the rules to apply to this account for this particular command
            // and the order in which they need to be applied
            var list= new List<IBusinessRule>();
            list.Add(new AccountBalanceMustNotBeNegative(accountState));
            return list;
        }

        
    }
}
