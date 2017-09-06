using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events
{
    public class AutoLetterSent : IEvent
    {
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
        public AutoLetterSent()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

    }
}
