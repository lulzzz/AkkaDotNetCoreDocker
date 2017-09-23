using Akka.Actor;
using Akka.Event;
using Akka.Monitoring;
using Akka.Persistence;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActorSupervisor : ReceivePersistentActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        /**
         * Actor's state = just a list of account under supervision
         */
        private Dictionary<string, IActorRef> _accounts = new Dictionary<string, IActorRef>();

        public AccountActorSupervisor()
        {
            /*** recovery section **/
            Recover<SnapshotOffer>(offer => ProcessSnapshot(offer));
            Recover<AccountAddedToSupervision>(command => ReplayEvent(command.AccountNumber));

            /** commands **/
            Command<SimulateBoardingOfAccounts>(client => RunSimulator(client));
            Command<SuperviseThisAccount>(command => ProcessSupervision(command));
            Command<StartAccounts>(command => StartAccounts());
            Command<TellMeYourStatus>(asking => Sender.Tell(new ThisIsMyStatus($"I have {_accounts.Count} accounts.",
                DictionaryToStringList())));
            Command<AboutMe>(me => Console.WriteLine($"About me: {me.Me}"));
            Command<string>(noMessage => { });

            /** Special handlers below; we can decide how to handle snapshot processin outcomes. */
            Command<SaveSnapshotSuccess>(success => DeleteMessages(success.Metadata.SequenceNr));
            Command<SaveSnapshotFailure>(
                failure => _log.Error(
                    $"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));
            Command<DeleteMessagesSuccess>(
                msg => _log.Info($"Successfully cleared log after snapshot ({msg.ToString()})"));
            CommandAny(msg => _log.Error($"Unhandled message in {Self.Path.Name}. Message:{msg.ToString()}"));
        }

        public override string PersistenceId => Self.Path.Name;

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
        }

        protected override void PreStart()
        {
            Context.IncrementActorCreated();
        }

        private void Monitor()
        {
            Context.IncrementMessagesReceived();
        }

        private void ProcessSnapshot(SnapshotOffer offer)
        {
            Monitor();
            _accounts = (Dictionary<string, IActorRef>) offer.Snapshot;
            _log.Info($"Snapshot recovered.");
        }

        private void RunSimulator(SimulateBoardingOfAccounts client)
        {
            Monitor();
            _log.Info($"Boarding client: {client.ClientName}");

            var boardingActor = Context.ActorOf<BoardAccountActor>($"Client-{client.ClientName}");
            boardingActor.Tell(client);
            Sender.Tell(new ThisIsMyStatus($"Started boarding of {client.ClientName} accounts at {DateTime.Now} "));
            //boardingActor.Tell(PoisonPill.Instance);
        }

        private Dictionary<string, string> DictionaryToStringList()
        {
            var viewble = new Dictionary<string, string>();
            foreach (var a in _accounts)
                viewble.Add(a.Key, a.Value?.ToString() ?? "Not Instantiated");
            return viewble;
        }

        private void StartAccounts()
        {
            Monitor();
            var immutAccounts = _accounts.Keys.ToList();
            foreach (var account in immutAccounts )
                if (account.Length != 0 && _accounts[account] == null)
                {
                    InstantiateThisAccount(account);
                    _log.Debug($"instantiated account {account}");
                }
                else
                {
                    _log.Debug($"skipped account {account}, already instantiated.");
                }
            Sender.Tell(new ThisIsMyStatus($"{_accounts.Count} accounts started.", DictionaryToStringList()));

        }

        private void ProcessSupervision(SuperviseThisAccount command)
        {
            Monitor();
            if (!_accounts.ContainsKey(command.AccountNumber))
            {
                var @event = new AccountAddedToSupervision(command.AccountNumber);
                Persist(@event, s =>
                {
                    _accounts.Add(command.AccountNumber, null); 
                    //InstantiateThisAccount(command.AccountNumber);
                });
                ApplySnapShotStrategy();
            }
            else
            {
                _log.Info($"You tried to load account {command.AccountNumber} which has already been loaded");
            }
        }

        private void ReplayEvent(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                 throw new Exception("Why is this blank?");
            }
            else
            {
                if (_accounts.ContainsKey(accountNumber))
                {
                    _log.Info($"Supervisor already has {accountNumber} in state. No action taken");
                }
                else
                {
                    _accounts.Add(accountNumber, null);
                    _log.Info($"Replayed event on {accountNumber}");
                }
            }
        }
 
        private IActorRef InstantiateThisAccount(string accountNumber)
        {
            if (_accounts.ContainsKey(accountNumber))
            {
                var accountActor = Context.ActorOf(Props.Create<AccountActor>(), accountNumber);
                _accounts[accountNumber] = accountActor;
                _log.Debug($"Instantiated account {accountActor.Path.Name}");
                return accountActor;
            }
            throw new Exception($"Why are you trying to instantiate an account not yet registered?");
        }

        public void ApplySnapShotStrategy()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 50 == 0)
            {
                var state = new Dictionary<string, IActorRef>(); // immutable, remember?
                foreach (var record in _accounts.Keys)
                    state.Add(record, null);
                SaveSnapshot(state);
                _log.Info($"Snapshot taken. LastSequenceNr is {LastSequenceNr}.");
                Context.IncrementCounter("SnapShotTaken");
            }
        }
    }

    public class TheReferenceToThisActor
    {
        public TheReferenceToThisActor(IActorRef address)
        {
            this.Address = address;
        }

        public IActorRef Address { get; }
    }
}