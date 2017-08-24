using System;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models 
{
    public interface ITransaction{

		DateTime OccurredOn();
		Guid UniqueGuid();
    }
}