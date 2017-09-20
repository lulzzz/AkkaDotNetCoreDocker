using System.Collections.Generic;

namespace Loaner.BoundedContexts.MaintenanceBilling.Events
{
    public class ThisIsMyStatus
    {
        public ThisIsMyStatus()
        {
            Accounts = new Dictionary<string, string>();
            Message = "";
        }

        public ThisIsMyStatus(string message)
        {
            Message = message;
            Accounts = new Dictionary<string, string>();
        }

        public ThisIsMyStatus(string message, Dictionary<string, string> accounts)
        {
            Message = message;
            Accounts = accounts;
        }

        public string Message { get; }
        public Dictionary<string, string> Accounts { get; }
    }
}