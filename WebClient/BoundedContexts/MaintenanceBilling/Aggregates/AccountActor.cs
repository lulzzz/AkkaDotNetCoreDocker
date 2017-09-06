using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates.State;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;

namespace AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates
{
    public class AccountActor : ReceivePersistentActor
    {
        public override string PersistenceId => Self.Path.Name;
        readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        /* This Actor's State */
        AccountState _accountState = new AccountState();

        public AccountActor()
        {
            /* Hanlde Recovery */
            Recover<SnapshotOffer>(offer => offer.Snapshot is AccountState, offer =>
            {
                _accountState = (AccountState)offer.Snapshot;
                _log.Debug($"Snapshot recovered.");
            });

            Recover<AccountCreated>(@event =>
            {
                _accountState = _accountState.Event(@event);
                _log.Debug($"Recovery event: AccountCreated");
            });

            Recover<ObligationAddedToAccount>(@event =>
            {
                _accountState = _accountState.Event(@event);
                _log.Debug($"Recovery event: ObligationAddedToAccount");
            });
            Recover<SuperSimpleSuperCoolEventFoundByRules>(@event => 
            {
                _accountState = _accountState.Event(@event);
                _log.Debug($"Recovery event: SuperSimpleSuperCoolEventFoundByRules");
            });


            /* Commands we can handle */
            Command<SayHi>(hi =>
            {
                string message = $"Message: {hi.Message} (Account: {_accountState?.AccountNumber})";
                _accountState.Event(new SomeOneSaidHiToMe(_accountState?.AccountNumber, hi?.Message));
                Sender.Tell(new AboutMe(message));
                ApplySnapShotStrategy();
            });
            Command<TellMeYourStatus>(asking => Sender.Tell(new TellMeYourStatus($"{_accountState.AccountNumber} I am alive!")));

            /* Creating the account's initial state is more of a one-time thing */
            Command<CreateAccount>(command =>
            {
                if (_accountState.AccountNumber == null)
                {
                    /**
					 * we want to use behaviours here to make sure we don't allow the account to be created 
					 * once it has been created -- Become AccountBoarded perhaps?
					  */
                    var @event = new AccountCreated(command.AccountNumber);
                    Persist(@event, s =>
                    {
                        _accountState = _accountState.Event(@event);
                        _log.Debug($"Created account {command.AccountNumber}");

                    });
                }
                else
                {
                    _log.Debug($"You are trying to create {command.AccountNumber}, but has already been created. No action taken.");
                }

            });

            /* Example of running comannds through business rules */
            Command<SettleFinancialConcept>(command => ApplyBusinessRules(command));
            Command<AssessFinancialConcept>(command => ApplyBusinessRules(command));
            Command<CancelAccount>(command => ApplyBusinessRules(command));

            /** Special handlers below; we can decide how to handle snapshot processin outcomes. */
            Command<SaveSnapshotSuccess>(success => _log.Debug($"Saved snapshot") /*DeleteMessages(success.Metadata.SequenceNr);*/ );
            Command<SaveSnapshotFailure>(failure => _log.Error($"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));

            /* For the demo there no business rules are assumed when adding an obligation to an account, but there most likely will be. */
            Command<AddObligationToAccount>(command =>
            {
                if (!_accountState.Obligations.ContainsKey(command.Obligation.ObligationNumber))
                {
                    var @event = new ObligationAddedToAccount(command.AccountNumber, command.Obligation);
                    Persist(@event, s =>
                    {
                        _accountState = _accountState.Event(@event);
                        ApplySnapShotStrategy();
                        _log.Debug($"Added obligation {command.Obligation.ObligationNumber} to account {command.AccountNumber}");
                        /* Optionally, put this command on the external notificaiton system (i.e. Kafka) */
                    });
                }
                else
                {
                    _log.Debug($"You are trying to add obligation {command.Obligation.ObligationNumber} an account which has exists on account {command.AccountNumber}. No action taken.");
                }
            });
        }

        private void ApplyBusinessRules(IDomainCommand command)
        {
            /**
			 * Here we can call Business Rules to validate and do whatever.
			 * Then, based on the outcome generated events.
			 * In this example, we are simply going to accept it and updated our state.
			 */
            var result = RulesApplicator.ApplyBusinessRules(_accountState, command);
            if (result.Success)
            {
                /* I may want to do old vs new state comparisons for other reasons
				 *  but ultimately we just update the state.. */
                result.GeneratedEvents.ForEach(@event =>
                {
                    Persist(@event, s =>
                    {
                        _accountState = _accountState.Event(@event);
                        ApplySnapShotStrategy();
                        _log.Debug($"Processing event {@event.ToString()} from business rules for command {command.ToString()}");
                    });
                });
            }
        }

		/*Example of how snapshotting can be custom to the actor, in this case per 'Account' events*/
		public void ApplySnapShotStrategy()
        {
            if (this.LastSequenceNr % 5 == 0)
            {
                SaveSnapshot(_accountState);
                _log.Debug($"Snapshot taken. LastSequenceNr is {this.LastSequenceNr}.");
            }
        }
    } 
}
