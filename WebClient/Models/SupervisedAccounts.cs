using System;
using System.Collections.Generic;

namespace WebClient
{
    public class SupervisedAccounts 
    {
        public SupervisedAccounts(string message, Dictionary<string,string> accounts)
        {
            Message = message;
            Accounts = accounts;
        }

        public string Message
        {
            get;
            private set;
        }

        public Dictionary<string, string> Accounts
        {
            get;
            private set;
        }
    }

} 