using System;
using Akka.Persistence;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events
{
    public class AccountCurrentBalanceUpdated : IEvent
    {

        public AccountCurrentBalanceUpdated(string accountNumber, double newCurrentBalance) : this()
        {
            AccountNumber = accountNumber;
            CurrentBalance = newCurrentBalance;
        }

        public string AccountNumber { get; private set; }

		public double CurrentBalance { get; private set; }

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

        public AccountCurrentBalanceUpdated()
        {
            _UniqueGuid = Guid.NewGuid();
            _OccurredOn = DateTime.Now;
        }
    }
}