using System;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Models;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActor : ReceivePersistentActor
    {
        public override string PersistenceId => Self.Path.Name;
        private readonly IActorRef logger;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        AccountState _accountState { get; set; }

        public AccountActor()
        {
            logger = Context.ActorOf(Props.Create<LoggingActor>(), Self.Path.Name + "logger");

            //Recovery
            Recover<AccountCurrentBalanceUpdated>(cmd => _accountState.Event(cmd));
            Recover<AccountStatusChanged>(cmd => _accountState.Event(cmd));
            Recover<AccountCancelled>(cmd => _accountState.Event(cmd));


            //Commands we can handle
            Command<SettleFinancialConcept>(cmd => Persist(cmd, s =>
            {
                if (_accountState.Obligations.ContainsKey(s.ObligationNumber))
                {
                    var trx = new FinancialTransaction(s.FinancialConcept, s.Amount);
                    _accountState.Obligations[s.ObligationNumber].PostTransaction(trx);
                    _accountState.Event(new AccountCurrentBalanceUpdated(_accountState.AccountNumber, s.Amount));
                }
            }));

            Command<AssessFinancialConcept>(cmd => Persist(cmd, s =>
            {
                if (_accountState.Obligations.ContainsKey(s.ObligationNumber))
                {
                    var trx = new FinancialTransaction(s.FinancialConcept, s.Amount);
                    _accountState.Obligations[s.ObligationNumber].PostTransaction(trx);
                    _accountState.Event(new AccountCurrentBalanceUpdated(_accountState.AccountNumber, s.Amount));
                }

            }));

            Command<CancelAccount>(cmd => Persist(cmd, s =>
            {
                if (s.Account.AccountNumber == _accountState.AccountNumber)
                {
                    Self.Tell(new AccountCancelled(s.Account.AccountNumber, AccountStatus.Closed));
                    Self.Tell(new AccountStatusChanged(s.Account.AccountNumber, AccountStatus.Closed));
                    Self.Tell(new AccountCurrentBalanceUpdated(_accountState.AccountNumber, 0.0));
                }
            }));

            //Special handlers
            Command<SaveSnapshotSuccess>(success => DeleteMessages(success.Metadata.SequenceNr));
            Command<SaveSnapshotFailure>(failure => _log.Error($"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));
            Command<SayHi>(hi =>
            {
                string message = $"{Self.Path.Name} has a current balance of ${_accountState.CurrentBalance} and it's account code is: {_accountState.AccountStatus}";
                Sender.Tell(new AboutMe(message));
            });
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
