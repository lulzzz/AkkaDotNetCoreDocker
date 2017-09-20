using System;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class MailingAddressUpdated : IBiographicInformationChanged
    {
        public MailingAddressUpdated()
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