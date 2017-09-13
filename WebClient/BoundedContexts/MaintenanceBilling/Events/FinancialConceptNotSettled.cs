using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class FinancialConceptNotSettled : IEvent
    {
        private readonly IDomainCommand RejectedCommand;
        private readonly IAccountBusinessRule BusinessRuleWouldBeViolated;

        public FinancialConceptNotSettled(IDomainCommand rejectedCommand, IAccountBusinessRule rule) : this()
        {
            this.RejectedCommand = rejectedCommand;
            this.BusinessRuleWouldBeViolated = rule;
        }

        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }

        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }
        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }
        public FinancialConceptNotSettled()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }
    }
}
