using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ThisIsMyInfo
    {
        public ThisIsMyInfo(AccountState info)
        {
            Info = info;
        }

        public AccountState Info { get; }
    }
}