using System.Collections.Immutable;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State
{
    public class AccountState
    {
        /**
         * Only two ways to initiate an Account State
         * All modifications to it must be immutable and done by the Event() handler
         */
        public AccountState()
        {
            this.Obligations = ImmutableDictionary.Create<string, Obligation>();
            this.SimulatedFields = ImmutableDictionary.Create<string, string>();
            this.AuditLog = ImmutableList.Create<StateLog>();
        }

        public AccountState(string accountNumber) : this()
        {
            this.AccountNumber = accountNumber;
        }


        /**
         * 
         * Private constructors, only to be used by the Event method 
         * 
        */
        private AccountState(string accountNumber,
                             ImmutableDictionary<string, string> simulation,
                            ImmutableList<StateLog> log)
        {
            this.SimulatedFields = simulation;
            this.AccountNumber = accountNumber;
            this.AuditLog = log;
            this.Obligations = ImmutableDictionary.Create<string, Obligation>();
        }

        private AccountState(string accountNumber, double currentBalance,
                             AccountStatus accountStatus,
                             ImmutableDictionary<string, Obligation> obligations,
                             ImmutableDictionary<string, string> simulation)
        {
            AccountNumber = accountNumber;
            CurrentBalance = currentBalance;
            accountStatus = AccountStatus;
            Obligations = obligations;
            SimulatedFields = simulation;
        }
        private AccountState(string accountNumber, double currentBalance,
                             AccountStatus accountStatus,
                             ImmutableDictionary<string, Obligation> obligations,
                             ImmutableDictionary<string, string> simulation,
                             ImmutableList<StateLog> log)
        {
            AccountNumber = accountNumber;
            CurrentBalance = currentBalance;
            accountStatus = AccountStatus;
            Obligations = obligations;
            AuditLog = log;
            SimulatedFields = simulation;
        }
        /**
         * 
         * The Event() handler is responsible for always returning a new state
         * 
         */

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
                    throw new UnknownAccountEvent($"{@event.GetType()}");
            }
        }

        public AccountState Event(SomeOneSaidHiToMe occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    this.AccountStatus, this.Obligations,
                                    LoadSimulation().ToImmutableDictionary(),
                                    AuditLog.Add(new StateLog("SomeOneSaidHiToMe", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(SuperSimpleSuperCoolEventFoundByRules occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    this.AccountStatus, this.Obligations,
                                    LoadSumulation(SimulatedFields, "1", "My state has been updated, see..."),
                                    AuditLog.Add(new StateLog("SuperSimpleSuperCoolEventFoundByRules", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(ObligationAssessedConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance + occurred.Amount,
                                    this.AccountStatus, this.Obligations,
                                    SimulatedFields,
                                    AuditLog.Add(new StateLog("ObligationAssessedConcept", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(AccountCurrentBalanceUpdated occurred)
        {
            return new AccountState(this.AccountNumber, occurred.CurrentBalance,
                                    this.AccountStatus, this.Obligations,
                                    SimulatedFields,
                                   AuditLog.Add(new StateLog("AccountCurrentBalanceUpdated", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(AccountStatusChanged occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    occurred.AccountStatus, this.Obligations,
                                    SimulatedFields,
                                    AuditLog.Add(new StateLog("AccountStatusChanged", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(AccountCancelled occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    occurred.AccountStatus, this.Obligations,
                                    SimulatedFields,
                                    AuditLog.Add(new StateLog("AccountCancelled", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(ObligationAddedToAccount occurred)
        {
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    this.AccountStatus,
                                    this.Obligations.Add(occurred.Obligation.ObligationNumber, occurred.Obligation),
                                    SimulatedFields,
                                    AuditLog.Add(new StateLog("ObligationAddedToAccount", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(ObligationSettledConcept occurred)
        {
            var trans = new FinancialTransaction(occurred.FinancialConcept, occurred.Amount);
            this.Obligations[occurred.ObligationNumber]?.PostTransaction(trans);
            return new AccountState(this.AccountNumber, this.CurrentBalance,
                                    this.AccountStatus, this.Obligations,
                                    SimulatedFields,
                                    AuditLog.Add(new StateLog("ObligationSettledConcept", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        public AccountState Event(AccountCreated occurred)
        {

            return new AccountState(occurred.AccountNumber,
                                    LoadSimulation(),
                                    AuditLog.Add(new StateLog("AccountCreated", occurred.UniqueGuid(), occurred.OccurredOn())));
        }

        /* Helpers */
        private static ImmutableDictionary<string, string> LoadSimulation()
        {
            var range = ImmutableDictionary.Create<string, string>();
            for (int i = 1; i <= 100; i++)
            {

                range = range.Add(i.ToString(), $"This simulates field {i.ToString()}");
            }

            return range;
        }

        private static ImmutableDictionary<string, string> LoadSumulation(ImmutableDictionary<string, string> state,
                                                                          string keyToUpdate,
                                                                          string valueToUpdate)
        {
            state = state.SetItem(keyToUpdate, valueToUpdate);
            return state;
        }


        public string AccountNumber { get; private set; }

        public double CurrentBalance { get; private set; }

        public AccountStatus AccountStatus { get; private set; }

        public ImmutableList<StateLog> AuditLog { get; private set; }

        public ImmutableDictionary<string, string> SimulatedFields { get; private set; }

        public ImmutableDictionary<string, Obligation> Obligations { get; private set; }

        public override string ToString()
        {
            return string.Format("[AccountState: Obligations={0}, AccountNumber={1}, CurrentBalance={2}, DebugInfo={3}, SimulatedFields={4}]", Obligations, AccountNumber, CurrentBalance, AuditLog, SimulatedFields);
        }
    }




}
