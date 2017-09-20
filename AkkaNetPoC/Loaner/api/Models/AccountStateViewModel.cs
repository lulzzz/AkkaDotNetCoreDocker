﻿
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;
using System;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;

namespace Loaner.api.Models
{
    public class AccountStateViewModel
    {
        public AccountStateViewModel()
        {
            AccountState = new AccountState();
        }

        public AccountStateViewModel(string message) : this()
        {
            Message = message;
        }

        public AccountStateViewModel(AccountState accountState)
        {
            this.AccountState = accountState;
            Message = $"State as of: {DateTime.Now}";
        }
        public AccountState AccountState { get; set; }

        public string Message { get; set; }
    }
}
