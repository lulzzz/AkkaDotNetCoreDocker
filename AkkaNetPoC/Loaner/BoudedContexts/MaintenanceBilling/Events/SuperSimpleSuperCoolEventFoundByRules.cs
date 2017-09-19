using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class SuperSimpleSuperCoolEventFoundByRules : IEvent
    {

        public string AccountNumber { get; private set; }
        DateTime _OccurredOn { get; }
        Guid _UniqueGuid { get; }
        public string Message { get; private set; }

        public DateTime OccurredOn()
        {
            return _OccurredOn;
        }

        public Guid UniqueGuid()
        {
            return _UniqueGuid;
        }
        public SuperSimpleSuperCoolEventFoundByRules()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }
        public SuperSimpleSuperCoolEventFoundByRules(string message) : this()
        {

            Message = message;
        }

        public SuperSimpleSuperCoolEventFoundByRules(string accountNumber, string message) : this()
        {
            Message = message;
            AccountNumber = accountNumber;
        }
    }
}
