using System;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class SomeOneSaidHiToMe : IEvent
	{
		private DateTime _OccurredOn { get; }
		public string AccountNumber { get; }
		private Guid _UniqueGuid { get; }

		public DateTime OccurredOn()
		{
			return _OccurredOn;
		}
		public Guid UniqueGuid()
		{
			return _UniqueGuid;
		}
		public SomeOneSaidHiToMe(string accountNumber, string debuInfo)
		{
			AccountNumber = accountNumber;
			_UniqueGuid = Guid.NewGuid();
			_OccurredOn = DateTime.Now;
            DebugInfo = debuInfo;
		}

        public string DebugInfo { get; }

	}
}