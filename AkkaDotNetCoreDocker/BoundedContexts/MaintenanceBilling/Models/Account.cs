using System.Collections.Generic;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models
{
    public class Account
    {
        public Account()
        {

        }

        public Account(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
        public AccountStatus AccountStatus { get; set; }
        public string AccountNumber { get; set; }
        public double CurrentBalance { get; set; }
        public List<Obligation> Obligations { get; set; }

    }

 
}
