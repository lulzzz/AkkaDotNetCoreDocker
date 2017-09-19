using System;
using System.Runtime.Serialization;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    internal class InvalidAccountProvided : Exception
    {
        public InvalidAccountProvided()
        {
        }

        public InvalidAccountProvided(string message) : base(message)
        {
        }

        public InvalidAccountProvided(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAccountProvided(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}