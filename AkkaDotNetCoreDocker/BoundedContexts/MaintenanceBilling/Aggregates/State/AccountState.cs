using System;
using System.Collections.Immutable;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class AccountState
    {

        public AccountState(string accountNumber)
        {
            this.AccountNumber = accountNumber;
        }

        public void Event(AccountCurrentBalanceUpdated occurred)
        {
            if (occurred.AccountNumber == this.AccountNumber)
            {
                this.CurrentBalance = occurred.CurrentBalance;
            }
            else
            {
                ErrorOut($"You privided {occurred.AccountNumber}");
            }
        }

        public void Event(AccountStatusChanged occurred)
        {
            if (occurred.AccountNumber == this.AccountNumber)
            {
                this.AccountStatus = occurred.AccountStatus;
            }
            else
            {
                ErrorOut($"You privided {occurred.AccountNumber}");
            }
        }

        public void Event(AccountCancelled occurred)
        {
            if (occurred.AccountNumber == this.AccountNumber)
            {
                this.AccountStatus = occurred.AccountStatus;
            }
            else
            {
                ErrorOut($"You privided {occurred.AccountNumber}");
            }
        }
       
        public void Event(ObligationAddedToAccount occurred)
        {
            if (occurred.AccountNumber == this.AccountNumber)
            {
                this.Obligations.Add(occurred.Obligation.ObligationNumber, occurred.Obligation);
            }
            else
            {
                ErrorOut($"You privided {occurred.AccountNumber}");
            }
        }

		public void Event(ObligationAssessedConcept occurred)
		{
            if (Obligations.ContainsKey(occurred.ObligationNumber))
			{
                var obligation = Obligations[occurred.ObligationNumber];
				
            }
			else
			{
                ErrorOut($"Obligation {occurred.ObligationNumber} does not exists on this account.");
			}
		}
        //Properties
        public ImmutableDictionary<string, Obligation> Obligations { get; private set; }
        public string AccountNumber { get; private set; }
        public double CurrentBalance { get; private set; }
        public AccountStatus AccountStatus { get; private set; }

        private void ErrorOut(string message)
        {
            throw new InvalidAccountProvided($"This account is {this.AccountNumber}. {message}");

        }
    }




}
