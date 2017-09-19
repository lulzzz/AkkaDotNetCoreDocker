
using System;
using System.Collections.Immutable;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;

namespace Loaner.api.Models
{
    public class AccountStateViewModel
    {
        public AccountStateViewModel(string message)
        {
            Message = message;
            AccountState = new AccountState();
        }

        public AccountStateViewModel(AccountState accountState)
        {
            this.AccountState = accountState;
            Message = $"State as of: {DateTime.Now}";
        }
        public AccountState AccountState { get; }

        public string Message { get; }
    }
}
