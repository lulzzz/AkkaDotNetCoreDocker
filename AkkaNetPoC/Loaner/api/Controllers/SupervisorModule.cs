
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;

namespace Loaner.api.Controllers
{
    using ActorManagement;
    using Akka.Actor;
    using BoundedContexts.MaintenanceBilling.Aggregates;
    using BoundedContexts.MaintenanceBilling.Commands;
    using BoundedContexts.MaintenanceBilling.Events;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;
    using System;
    using System.Threading.Tasks;

    public class SupervisorModule : NancyModule
    {
        public SupervisorModule()
        {
            Get("api/supervisor", async args =>
            {
                var answer = new ThisIsMyStatus("This didn't work");
                await Task.Run(() =>
                {
                    answer = LoanerActors.AccountSupervisor.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(5)).Result;
                    var response = new SupervisedAccounts(answer.Message, answer.Accounts);
                    return response;
                });
                return Response.AsJson( new SupervisedAccounts(answer.Message, answer.Accounts));
            });
            Get("api/supervisor/run", async args =>
            {
                var answer = new ThisIsMyStatus("This didn't work");
                await Task.Run(() =>
                {
                    answer = LoanerActors.AccountSupervisor.Ask<ThisIsMyStatus>(new StartAccounts(), TimeSpan.FromSeconds(5)).Result;
                    var response = new SupervisedAccounts(answer.Message, answer.Accounts);
                });
                return Response.AsJson(new SupervisedAccounts(answer.Message, answer.Accounts));

            });

            Post("api/supervisor/simulation", args=>
            {
                SimulateBoardingOfAccountModel client = this.Bind<SimulateBoardingOfAccountModel>();
                LoanerActors.AccountSupervisor.Tell(new SimulateBoardingOfAccounts(
                    client.ClientName,
                    client.ClientAccountsFilePath,
                    client.ObligationsFilePath
                ));

                return Response.AsJson(new ThisIsMyStatus("Done"));
            });
        }
    }
}
