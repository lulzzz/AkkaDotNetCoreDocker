using System;
using System.Collections.Generic;

namespace Loaner.api
{
    public class SupervisedAccounts 
    {
        public SupervisedAccounts()
        {
            Accounts = new Dictionary<string, string>();
        }
        public SupervisedAccounts(string message, Dictionary<string,string> accounts)
        {
            Message = message;
            Accounts = accounts;
        }

        public string Message
        {
            get;
             set;
        }

        public Dictionary<string, string> Accounts
        {
            get;
             set;
        }
    }

} 