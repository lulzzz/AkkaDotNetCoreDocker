using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using Akka.Monitoring;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActorSupervisor : ReceivePersistentActor
    {
        public override string PersistenceId => Self.Path.Name;
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        /**
         * Actor's state = just a list of account under supervision
         */
        Dictionary<string, IActorRef> _accounts = new Dictionary<string, IActorRef>();

        public AccountActorSupervisor()
        {
            /*** recovery section **/
            Recover<SnapshotOffer>(offer => this.ProcessSnapshot(offer));
            Recover<AccountAddedToSupervision>(command => this.ReplayEvent(command.AccountNumber));

            /** commands **/
            Command<SimulateBoardingOfAccounts>(client => RunSimulator(client));
            Command<SuperviseThisAccount>(command => ProcessSupervision(command));
            Command<StartAccounts>(command => this.StartAccounts());
            Command<TellMeYourStatus>(asking => Sender.Tell(new ThisIsMyStatus($"I have {_accounts.Count} accounts.", new Dictionary<string, string>())));
            Command<AboutMe>(me => Console.WriteLine($"About me: {me.Me}"));
            Command<string>(NoMessage => { });

            /** Special handlers below; we can decide how to handle snapshot processin outcomes. */
            Command<SaveSnapshotSuccess>(success => DeleteMessages(success.Metadata.SequenceNr));
            Command<SaveSnapshotFailure>(failure => _log.Error($"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));
            Command<DeleteMessagesSuccess>(msg => _log.Info($"Successfully cleared log after snapshot ({msg.ToString()})"));
            CommandAny(msg => _log.Error($"Unhandled message in {Self.Path.Name}. Message:{msg.ToString()}"));

        }
        protected override void PostStop()
        {
            Context.IncrementActorStopped();
        }
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
        }
        private void Monitor(){
            Context.IncrementMessagesReceived();
        }

        private void ProcessSnapshot(SnapshotOffer offer)
        {
            this.Monitor();
            _accounts = (Dictionary<string, IActorRef>)offer.Snapshot;
            _log.Info($"Snapshot recovered.");
        }

        private void RunSimulator(SimulateBoardingOfAccounts client)
        {
            this.Monitor();
            _log.Info($"Boarding client: {client.ClientName}");

            var boardingActor = Context.ActorOf<BoardAccountActor>(name: $"Client-{client.ClientName}");
            boardingActor.Tell(client);
            Sender.Tell($"Started boarding of {client.ClientName} accounts at {DateTime.Now} ");
        }

        private Dictionary<string, string> DictionaryToStringList()
        {
            Dictionary<string, string> viewble = new Dictionary<string, string>();
            foreach (var a in _accounts)
            {
                viewble.Add(a.Key, a.Value?.ToString() ?? "Not Instantiated");
            }
            return viewble;
        }

        private void StartAccounts()
        {
            this.Monitor();
            var accounts = new List<string>(_accounts.Keys);
            foreach (var account in accounts)
            {
                if (_accounts[account] == null)
                {
                    InstantiateThisAccount(account);
                    _log.Debug($"instantiated account {account}");
                }
                else
                {
                    _log.Debug($"skipped account {account}, already instantiated.");
                }
            }
            Sender.Tell(new ThisIsMyStatus($"{_accounts.Count} accounts started.", DictionaryToStringList())); ;
        }

        private void ProcessSupervision(SuperviseThisAccount command)
        {
            this.Monitor();
            if (!_accounts.ContainsKey(command.AccountNumber))
            {
                AccountAddedToSupervision @event = AddThisAccountToState(command.AccountNumber);
                Persist(@event, s =>
                {
                    var address = InstantiateThisAccount(command.AccountNumber);

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
      
        private AccountAddedToSupervision AddThisAccountToState(string accountNumber)
        {
            if (!_accounts.ContainsKey(accountNumber))
            {
                _accounts.Add(accountNumber, null);
                var @event = new AccountAddedToSupervision(accountNumber);
                return @event;
            }
            return null;
        }
      
        private IActorRef InstantiateThisAccount(string accountNumber)
        {
            if (_accounts.ContainsKey(accountNumber))
            {
                
                var accountActor = Context.ActorOf(Props.Create<AccountActor>(), name: accountNumber);
                _accounts[accountNumber] = accountActor;
                _log.Debug($"Instantiated account {accountActor.Path.Name}");
                return accountActor;
            }
            else
            {
                throw new Exception($"Why are you trying to instantiate an account not yet registered?");
            }
        }
      
        public void ApplySnapShotStrategy()
        {
            if (this.LastSequenceNr != 0 && this.LastSequenceNr % 1000 == 0)
            {
                var state = new Dictionary<string, IActorRef>(); // immutable, remember?
                foreach (var record in _accounts.Keys)
                {
                    state.Add(record, null);
                }
                SaveSnapshot(state);
                _log.Info($"Snapshot taken. LastSequenceNr is {this.LastSequenceNr}.");
                Context.IncrementCounter("SnapShotTaken");
            }
        }
    }

    public class TheReferenceToThisActor
    {
        public IActorRef address { get; }
        public TheReferenceToThisActor(IActorRef address)
        {
            this.address = address;
        }
    }
}