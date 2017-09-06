using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events
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

        public SuperSimpleSuperCoolEventFoundByRules(string message)
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
            Message = message;
        }

        public SuperSimpleSuperCoolEventFoundByRules(string accountNumber, string message) : this(message)
        {
            Message = message;
            AccountNumber = accountNumber;
        }
    }
}
