using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class ObligationAddedToAccount : IEvent
	{
		public Obligation Obligation { get; private set; }
		public string AccountNumber { get; private set; }
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

		public ObligationAddedToAccount()
		{
			_UniqueGuid = Guid.NewGuid();
			_OccurredOn = DateTime.Now;
		}

		public ObligationAddedToAccount(string accountNumber, Obligation obligation) : this()
        {
			Obligation = obligation;
			AccountNumber = accountNumber;
		}
    }
}