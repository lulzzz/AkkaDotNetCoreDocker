using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ThisIsMyInfo
    {
        public AccountState Info { get; }

        public ThisIsMyInfo(AccountState info)
        {
            this.Info = info;
        }
    }
}