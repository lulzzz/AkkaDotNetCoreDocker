using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events
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