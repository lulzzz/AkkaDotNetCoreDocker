using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Persistence;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActorSupervisor : ReceivePersistentActor
    {
        public override string PersistenceId => Self.Path.Name;
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        /**
         * Actor's state = just a list of account under supervision
         */
        ImmutableDictionary<string, IActorRef> _accounts = ImmutableDictionary<string, IActorRef>.Empty;

        public AccountActorSupervisor()
        {
            /*** recovery section **/
            Recover<SnapshotOffer>(offer =>
            {
                _accounts = (ImmutableDictionary<string, IActorRef>)offer.Snapshot;
                _log.Debug($"Snapshot recovered.");
            });

            Recover<AccountAddedToSupervision>(command => AddThisAccountToState(command.AccountNumber));
            /*** end recovery section **/

            /** commands **/
            Command<SimulateBoardingOfAccounts>(client =>
            {
                _log.Debug($"Client: {client.ClientAccountFilePath}");
                var boardingActor = Context.ActorOf<BoardAccountActor>(name: "Boarding");
                boardingActor.Tell(client);
            });

            Command<SuperviseThisAccount>(command => ProcessSupervision(command));

            Command<StartAccounts>(command =>
            {
                StartAccounts();
                Sender.Tell(new ThisIsMyStatus($"{_accounts.Count} accounts started.", DictionaryToStringList())); ;
            });

            Command<TellMeYourStatus>(asking => Sender.Tell(new ThisIsMyStatus($"I have {_accounts.Count} accounts.", DictionaryToStringList())));

            Command<AboutMe>(me => Console.WriteLine($"About me: {me.Me}"));

            /* Example of custom error handling, also using messages */
            Command<FailedToLoadAccounts>(m => Self.Tell(typeof(Stop)));
            Command<FailedToLoadObligations>(m => Self.Tell(typeof(Stop)));

            /** end commands **/

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

        }

        private void ProcessSupervision(SuperviseThisAccount command)
        {
            if (!_accounts.ContainsKey(command.Account.AccountNumber))
            {
                var @event = AddThisAccountToState(command.Account.AccountNumber);
                Persist(@event, s =>
                {
                    var address = InstantiateThisAccount(command.Account.AccountNumber);
                    Sender.Tell(new TheReferenceToThisActor(address));
                });
                ApplySnapShotStrategy();
            }
            else
            {
                _log.Info($"You tried to load account {command.Account.AccountNumber} which has already been loaded");
            }
        }

        private AccountAddedToSupervision AddThisAccountToState(string accountNumber)
        {
            if (!_accounts.ContainsKey(accountNumber))
            {
                _accounts = _accounts.Add(accountNumber, null);
                var @event = new AccountAddedToSupervision(accountNumber);
                return @event;
            }
            return null;
        }
        private IActorRef InstantiateThisAccount(string accountNumber)
        {
            if (_accounts.ContainsKey(accountNumber))
            {
                var accountActor = Context.ActorOf<AccountActor>(name: accountNumber);
                _accounts = _accounts.SetItem(accountNumber,accountActor);
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
            if (this.LastSequenceNr % 1000 == 0)
            {
                SaveSnapshot(_accounts);
                _log.Debug($"Snapshot taken. LastSequenceNr is {this.LastSequenceNr}.");
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