using System;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.BusinessRules;
using System.Collections.Generic;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActor : ReceivePersistentActor
    {
        public override string PersistenceId => Self.Path.Name;

        private readonly IActorRef logger;

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private AccountState _accountState;
        private List<string> testState = new List<string>();

        public AccountActor()
        {
            logger = Context.ActorOf(Props.Create<LoggingActor>(), Self.Path.Name + "logger");

            //Recovery
            Recover<SnapshotOffer>(offer =>
            {
                if (offer.Snapshot is AccountState messages) // null check
                {
                    _accountState = messages;
                }
                if (offer.Snapshot is List<string> lst) // null check
                {
                    testState = lst;
                }
            });
            Recover<AccountCreated>(@event =>
            {
                _log.Info($"Recovery event: AccountCreated");
                _accountState.Event(@event);
                 
            });
            Recover<ObligationAddedToAccount>(@event =>
            {
                _log.Info($"Recovery event: ObligationAddedToAccount");
                _accountState.Event(@event);
            });
            Recover<AccountCurrentBalanceUpdated>(@event =>
            {
                _log.Info($"Recovery event: AccountCurrentBalanceUpdated");
                _accountState.Event(@event);
            });
            Recover<AccountStatusChanged>(@event =>
            {
                _log.Info($"Recovery event: AccountStatusChanged");
                _accountState.Event(@event);
            });
            Recover<AccountCancelled>(@event =>
            {
                _log.Info($"Recovery event: AccountCancelled");
                _accountState.Event(@event);
            });


            //Commands we can handle
            Command<CreateAccount>(cmd => Persist(cmd, s =>
            {
                _accountState = new AccountState(s.AccountNumber);
                ApplySnapShotStrategy();
                testState.Add($"CreateAccount {s.AccountNumber}");

            }));

            Command<AddObligationToAccount>(cmd => Persist(cmd, s =>
            {
                Self.Tell(new ObligationAddedToAccount(s.AccountNumber, s.Obligation));
                ApplySnapShotStrategy();
                Console.WriteLine($"******************AddObligationToAccount {s.AccountNumber} - {s.Obligation.ObligationNumber}*******");
                testState.Add($"AddObligationToAccount {s.AccountNumber} - {s.Obligation.ObligationNumber}");
            }));

            Command<SettleFinancialConcept>(cmd => Persist(cmd, s =>
            {
                /**
                 * Here we can call Business Rules to validate and do whatever.
                 * Then, based on the outcome generated events.
                 * In this example, we are simply going to accept it and updated our state.
                 */
                bool conceptHasBeenBilled = false;

                foreach (var trans in _accountState.Obligations[s.ObligationNumber].GetTransactions())
                {
                    if (trans.Value.FinancialConcept == s.FinancialConcept)
                    {
                        conceptHasBeenBilled = true;
                        var settledEvent = new ObligationSettledConcept(s.ObligationNumber, s.FinancialConcept, s.Amount);
                        var newBalance = CalculateNewCurrentBalanceIfEventIsApplied(settledEvent); //Business rule
                        if (newBalance >= 0.00)
                        {
                            var balanceEvent = new AccountCurrentBalanceUpdated(_accountState.AccountNumber, newBalance);
                            Self.Tell(settledEvent);
                            Self.Tell(balanceEvent);
                            ApplySnapShotStrategy();
                        }
                        else
                        {
                            Sender.Tell(new FinancialConceptNotSettled(s, new AccountBalanceWouldBeNegative()));
                        }
                    }
                }

                if (!conceptHasBeenBilled)
                {
                    Sender.Tell(new FinancialConceptNotSettled(s, new FinancialConceptMustBeBilled()));
                }
            }));

            Command<AssessFinancialConcept>(cmd => Persist(cmd, s =>
            {
                var settledEvent = new ObligationSettledConcept(s.ObligationNumber, s.FinancialConcept, s.Amount);
                var newBalance = CalculateNewCurrentBalanceIfEventIsApplied(settledEvent); //Business rule
                var balanceEvent = new AccountCurrentBalanceUpdated(_accountState.AccountNumber, newBalance);
                Self.Tell(settledEvent);
                Self.Tell(balanceEvent);
                ApplySnapShotStrategy();

            }));

            Command<CancelAccount>(cmd => Persist(cmd, s =>
            {
                ZeroOutBucketsOnObligations(); // We could kick-off a second process if we zeroed out amounts, for example
                Self.Tell(new AccountCancelled(s.Account.AccountNumber, AccountStatus.Closed));
                Self.Tell(new AccountStatusChanged(s.Account.AccountNumber, AccountStatus.Closed));
                Self.Tell(new AccountCurrentBalanceUpdated(_accountState.AccountNumber, 0.0));
                ApplySnapShotStrategy();
            }));

            //Special handlers
            Command<SaveSnapshotSuccess>(success => DeleteMessages(success.Metadata.SequenceNr));
            Command<SaveSnapshotFailure>(failure => _log.Error($"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));
            Command<SayHi>(hi =>
            {
                string message = $"{hi}: I am {_accountState?.AccountNumber}";
                Sender.Tell(new AboutMe(message));
            });
        }
        private double ZeroOutBucketsOnObligations()
        {
            double amountZeroedOut = 0.00;
            foreach (var obligation in _accountState.Obligations)
            {
                // each obligations financial buckets with non-zero balances must be zeroed out.
                foreach (var bucket in obligation.Value.GetBucketBalances())
                {
                    if (bucket.Value > 0.00)
                    {
                        var waiveamount = bucket.Value * -1;
                        amountZeroedOut += waiveamount;
                        obligation.Value.PostTransaction(new FinancialTransaction(bucket.Key, waiveamount));
                    }
                }
            }
            return amountZeroedOut;
        }
        private double CalculateNewCurrentBalanceIfEventIsApplied(ObligationSettledConcept settledEvent)
        {
            return _accountState.CurrentBalance + settledEvent.Amount;
            //Obviously, a whole lot more complex in real life.
        }

        public void ApplySnapShotStrategy()
        {
            if (this.LastSequenceNr % 3 == 0)
            {
                SaveSnapshot(_accountState);
                SaveSnapshot(testState);
            }
        }
        public void LoadAccount()
        {
            _log.Debug($"Hello! From: {Self.Path.Name}");
        }
    }

    public class AboutMe
    {
        public AboutMe(string me)
        {
            Me = me;
        }

        public string Me { get; set; }
    }

    public class SayHi
    {
    }
}
