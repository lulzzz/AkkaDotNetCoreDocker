using System;
using Loaner.BoundedContexts.MaintenanceBilling.Models;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class NotePostedOnAccount : IEvent
    {
        public NotePostedOnAccount()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }

        public NotePostedOnAccount(Note messageNote) : this()
        {
            MessageNote = messageNote;
        }

        private DateTime _OccurredOn { get; }
        private Guid _UniqueGuid { get; }
        public Note MessageNote { get; set; }

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