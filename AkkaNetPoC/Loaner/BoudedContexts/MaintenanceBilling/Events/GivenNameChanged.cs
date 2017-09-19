using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class GivenNameChanged :IBiographicInformationChanged
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
        public GivenNameChanged()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }
    }
}
