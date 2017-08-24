using System;
using System.Numerics;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events
{
    public interface IEvent
    {
        DateTime OccurredOn();
        Guid UniqueGuid();
    }
}
