using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class AccountState
    {

        public AccountState()
        {
            Obligations = ImmutableDictionary.Create<string, Obligation>();
            this.SimulatedFields = ImmutableDictionary.Create<string, string>();
        }
        public AccountState(string accountNumber) : this()
        {
            this.AccountNumber = accountNumber;
        }

        public AccountState(string accountNumber,ImmutableDictionary<string, string> simulation)
        {
            this.AccountNumber = accountNumber;
            Obligations = ImmutableDictionary.Create<string, Obligation>();
            this.SimulatedFields = ImmutableDictionary.Create<string, string>();
        }
       
        private AccountState(string accountNumber, double currentBalance, AccountStatus accountStatus, ImmutableDictionary<string, Obligation> obligations,ImmutableDictionary<string, string> simulation)
        {
            AccountNumber = accountNumber;
            CurrentBalance = currentBalance;
            accountStatus = AccountStatus;
            Obligations = obligations;
            SimulatedFields = simulation;
        }
        private AccountState(string accountNumber, double currentBalance, AccountStatus accountStatus, ImmutableDictionary<string, Obligation> obligations, ImmutableDictionary<string, string> simulation, string debugInfo)
        {
            AccountNumber = accountNumber;
            CurrentBalance = currentBalance;
            accountStatus = AccountStatus;
            Obligations = obligations;
            DebugInfo = debugInfo;
            SimulatedFields = simulation;
        }
        public AccountState Event(SomeOneSaidHiToMe occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary(), $"{this.DebugInfo}|{occurred.DebugInfo}");
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
        public AccountState Event(SuperSimpleSuperCoolEventFoundByRules occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations, LoadSumulation().ToImmutableDictionary(),"Processed dummy SuperSimpleSuperCoolEventFoundByRules");

        }
        public AccountState Event(AccountCurrentBalanceUpdated occurred)
        {
            return new AccountState(this.AccountNumber, occurred.CurrentBalance, this.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary());
        }

        public AccountState Event(AccountStatusChanged occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, occurred.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary());
        }

        public AccountState Event(AccountCancelled occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, occurred.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary());
        }

        public AccountState Event(ObligationAddedToAccount occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations.Add(occurred.Obligation.ObligationNumber, occurred.Obligation),LoadSumulation().ToImmutableDictionary());
        }

        public AccountState Event(ObligationAssessedConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary());

        }
        public AccountState Event(ObligationSettledConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance, this.AccountStatus, this.Obligations,LoadSumulation().ToImmutableDictionary());

        }
        public AccountState Event(AccountCreated occurred)
        {
            
            return new AccountState(occurred.AccountNumber,LoadSumulation().ToImmutableDictionary());
        }


        private static Dictionary<string, string> LoadSumulation()
        {
            var range = new Dictionary<string, string>();
            for (int i = 0; i < 100; i++)
            {
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                byte[] bytes = new byte[10];
                crypto.GetBytes(bytes);
                range.Add(i.ToString(), $"This is a very large and random string to sumulate state padding {Encoding.ASCII.GetString(bytes)}");
            }

            return range;
        }


        public ImmutableDictionary<string, Obligation> Obligations { get; }
        public string AccountNumber { get; }
        public double CurrentBalance { get; }
        private AccountStatus AccountStatus { get; }

        public string DebugInfo { get; set; }

        public ImmutableDictionary<string, string> SimulatedFields { get; }

    }




}
