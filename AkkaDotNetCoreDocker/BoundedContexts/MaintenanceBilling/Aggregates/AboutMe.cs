namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AboutMe
    {
        public AboutMe(string me)
        {
            Me = me;
        }
        public string Me { get; set; }
    }
}
