using System;
using Loaner.BoundedContexts.MaintenanceBilling.BusinessRules;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class FinancialConceptNotSettled : IEvent
    {
        private readonly IAccountBusinessRule BusinessRuleWouldBeViolated;
        private readonly IDomainCommand RejectedCommand;

        public FinancialConceptNotSettled(IDomainCommand rejectedCommand, IAccountBusinessRule rule) : this()
        {
            RejectedCommand = rejectedCommand;
            BusinessRuleWouldBeViolated = rule;
        }

        public FinancialConceptNotSettled()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
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
    }
}