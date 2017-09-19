using Akka.Actor;
using Akka.Event;
using Akka.Monitoring;
using Akka.Persistence;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.State;
using Loaner.BoundedContexts.MaintenanceBilling.BusinessRules;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Events;


namespace Loaner.BoundedContexts.MaintenanceBilling.Aggregates
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
            Recover<SnapshotOffer>(offer => offer.Snapshot is AccountState, offer => ApplySnapShot(offer));
            Recover<AccountCreated>(@event => ApplyPastEvent("AccountCreated", @event));
            Recover<ObligationAddedToAccount>(@event => ApplyPastEvent("ObligationAddedToAccount", @event));
            Recover<ObligationAssessedConcept>(@event => ApplyPastEvent("ObligationAddedToAccount", @event));
            Recover<SuperSimpleSuperCoolEventFoundByRules>(@event => ApplyPastEvent("SuperSimpleSuperCoolEventFoundByRules", @event));

            /**
             * Creating the account's initial state is more of a one-time thing 
             * For the demo there no business rules are assumed when adding an 
             * obligation to an account, but there most likely will be in reality
             * */
            Command<CreateAccount>(command => InitiateAccount(command));
            Command<AddObligationToAccount>(command => AddObligation(command));

            /* Example of running comannds through business rules */
            Command<SettleFinancialConcept>(command => ApplyBusinessRules(command));
            Command<AssessFinancialConcept>(command => ApplyBusinessRules(command));
            Command<BillingAssessment>(command => ProcessBilling(command));
            Command<CancelAccount>(command => ApplyBusinessRules(command));
            Command<AskToBeSupervised>(command => SendParentMyState(command));

            /** Special handlers below; we can decide how to handle snapshot processin outcomes. */
            Command<SaveSnapshotSuccess>(success => DeleteMessages(success.Metadata.SequenceNr));
            Command<SaveSnapshotFailure>(failure => _log.Error($"Actor {Self.Path.Name} was unable to save a snapshot. {failure.Cause.Message}"));
            //Command<RecoverySuccess>(msg => this.WakeUp());
            Command<TellMeYourStatus>(asking => Sender.Tell(new ThisIsMyStatus(message: $"{Self.Path.Name} I am alive!")));
            Command<TellMeYourInfo>(asking => Sender.Tell(new ThisIsMyInfo(info: _accountState)));
            Command<DeleteMessagesSuccess>(msg => _log.Info($"Successfully cleared log after snapshot ({msg.ToString()})"));
            CommandAny(msg => _log.Error($"Unhandled message in {Self.Path.Name}. Message:{msg.ToString()}"));

        }

        private void ProcessBilling(BillingAssessment command)
        {

            ApplyBusinessRules(command);

            Sender.Tell(new ThisIsMyStatus($"Your billing request has been submited to occount {_accountState.AccountNumber}. The new account state is: {_accountState}"));
        }


        /*private void WakeUp()
        {
            var Mommy = Context.ActorSelection($"/user/demo-supervisor-{_accountState.Portfolio}").ResolveOne(TimeSpan.FromSeconds(10)).Result;
            Mommy.Tell(new SuperviseThisAccount(_accountState.AccountNumber));
        }*/

        private void SendParentMyState(AskToBeSupervised command)
        {
            this.Monitor();
            /* Assuming this is all we have to load for an account, then we can have the account
             * send the supervisor to add it to it's list -- then it can terminate. 
             */
            command.MyNewParent.Tell(new SuperviseThisAccount(Self.Path.Name));
            Context.Stop(Self);
        }

        private void ApplySnapShot(SnapshotOffer offer)
        {
            _accountState = (AccountState)offer.Snapshot;
            _log.Debug($"Snapshot recovered.");
        }

        private void ApplyPastEvent(string eventname, IEvent @event)
        {
            _accountState = _accountState.Event(@event);
            _log.Debug($"Recovery event: {eventname}");
        }

        private void AddObligation(AddObligationToAccount command)
        {
            this.Monitor();
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
        }

        private void InitiateAccount(CreateAccount command)
        {
            this.Monitor();
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
                _log.Info($"You are trying to create {command.AccountNumber}, but has already been created. No action taken.");
            }
        }

        private void ApplyBusinessRules(IDomainCommand command)
        {
            this.Monitor();
            /**
			 * Here we can call Business Rules to validate and do whatever.
			 * Then, based on the outcome generated events.
			 * In this example, we are simply going to accept it and updated our state.
			 */
            var result = AccountBusinessRulesManager.ApplyBusinessRules(_accountState, command);
            _log.Info($"There were {result.GeneratedEvents.Count} events for {command.ToString()} command. And it was {result.Success}");
            _log.Info($"And it returned {result.Success}");
            if (result.Success)
            {
                /* I may want to do old vs new state comparisons for other reasons
				 *  but ultimately we just update the state.. */
                var events = result.GeneratedEvents;
                foreach (var @event in events)
                {
                    _log.Info($"Event: {@event.ToString()}");
                    Persist(@event, s =>
                    {
                        _accountState = _accountState.Event(@event);
                        ApplySnapShotStrategy();
                        _log.Info($"Processing event {@event.ToString()} from business rules for command {command.ToString()}");
                    });
                }
            }
        }

        /*Example of how snapshotting can be custom to the actor, in this case per 'Account' events*/
        public void ApplySnapShotStrategy()
        {
            if (this.LastSequenceNr != 0 && this.LastSequenceNr % 100 == 0)
            {
                SaveSnapshot(_accountState);
                _log.Debug($"Snapshot taken. LastSequenceNr is {this.LastSequenceNr}.");
            }
        }
        private void Monitor()
        {
            Context.IncrementMessagesReceived();
        }
        protected override void PostStop()
        {
            Context.IncrementActorStopped();
        }
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
        }
    }
}
