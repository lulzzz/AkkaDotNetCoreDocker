using System.Collections.Immutable;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class AccountState
    {
        public AccountState()
        {
            Obligations = ImmutableDictionary.Create<string, Obligation>();
        }
        public AccountState(string accountNumber) : this()
        {
            this.AccountNumber = accountNumber;
        }
        private AccountState(string accountNumber, double currentBalance, AccountStatus accountStatus, ImmutableDictionary<string, Obligation> obligations)
        {
            AccountNumber = accountNumber;
            CurrentBalance = currentBalance;
            accountStatus = AccountStatus;
            Obligations = obligations;
        }
		private AccountState(string accountNumber, double currentBalance, AccountStatus accountStatus, ImmutableDictionary<string, Obligation> obligations, string debugInfo)
		{
			AccountNumber = accountNumber;
			CurrentBalance = currentBalance;
			accountStatus = AccountStatus;
			Obligations = obligations;
            DebugInfo = debugInfo;
		}
        public AccountState Event(SomeOneSaidHiToMe occurred){
           	return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations, $"{this.DebugInfo}|{occurred.DebugInfo}" );
		}
        public AccountState Event(IEvent @event)
        {
            switch (@event)
            {
                case AccountCurrentBalanceUpdated occurred:
                    return this.Event(occurred);
                case AccountStatusChanged occurred:
                    return this.Event(occurred);
                case AccountCancelled occurred:
                    return this.Event(occurred);
                case ObligationAddedToAccount occurred:
                    return this.Event(occurred);
                case ObligationAssessedConcept occurred:
                    return this.Event(occurred);
                case ObligationSettledConcept occurred:
                    return this.Event(occurred);
                case AccountCreated occurred:
                    return this.Event(occurred);
                case SuperSimpleSuperCoolEventFoundByRules occurred:
                    return this.Event(occurred);
                default:
                    throw new UnknownBusinessRule($"{@event.GetType()}");
            }
        }
        public AccountState Event(SuperSimpleSuperCoolEventFoundByRules occurred){
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations,"Processed dummy SuperSimpleSuperCoolEventFoundByRules");

        }
        public AccountState Event(AccountCurrentBalanceUpdated occurred)
        {
            return new AccountState(this.AccountNumber, occurred.CurrentBalance, this.AccountStatus, this.Obligations);
        }

        public AccountState Event(AccountStatusChanged occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, occurred.AccountStatus, this.Obligations);
        }

        public AccountState Event(AccountCancelled occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, occurred.AccountStatus, this.Obligations);
        }

        public AccountState Event(ObligationAddedToAccount occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations.Add(occurred.Obligation.ObligationNumber, occurred.Obligation));
        }

        public AccountState Event(ObligationAssessedConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations);

        }
        public AccountState Event(ObligationSettledConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations);

        }
        public AccountState Event(AccountCreated occurred)
        {
            return new AccountState(occurred.AccountNumber);
        }

        public ImmutableDictionary<string, Obligation> Obligations { get; }
        public string AccountNumber { get; }
        public double CurrentBalance { get; }
        private AccountStatus AccountStatus { get; }

        public string DebugInfo { get; set; }


    }




}
