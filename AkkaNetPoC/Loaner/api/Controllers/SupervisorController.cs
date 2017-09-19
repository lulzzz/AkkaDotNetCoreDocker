using System;
using Akka.Actor;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.api.ActorManagement;
using Loaner.api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Loaner.api.Controllers
{
    public class SupervisorController : Controller
    {

        [Route("api/supervisor")]
        [HttpGet]
        public async Task<SupervisedAccounts> AccountSupervisor()
        {
            var answer = new ThisIsMyStatus("This didn't work");
            await Task.Run(() =>
            {
                answer = LoanerActors.AccountSupervisor.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(5)).Result;
                var response = new SupervisedAccounts(answer.Message, answer.Accounts);
                return response;
            });
            return new SupervisedAccounts(answer.Message, answer.Accounts);
        }

        [HttpGet]
        [Route("api/supervisor/run")]
        public async Task<SupervisedAccounts> StartAccountsAsync()
        {
            var answer = new ThisIsMyStatus("This didn't work");
            await Task.Run(() =>
           {
               answer = LoanerActors.AccountSupervisor.Ask<ThisIsMyStatus>(new StartAccounts(), TimeSpan.FromSeconds(5)).Result;
               var response = new SupervisedAccounts(answer.Message, answer.Accounts);
           });
            return new SupervisedAccounts(answer.Message, answer.Accounts);
        }

        [HttpPost]
        [Route("api/supervisor/simulation")]
        public ThisIsMyStatus BoardAccountsAsync([FromBody]SimulateBoardingOfAccountModel client)
        {

            LoanerActors.AccountSupervisor.Tell(new SimulateBoardingOfAccounts(
                client.ClientName,
                client.ClientAccountsFilePath,
                client.ObligationsFilePath
           ));

            return new ThisIsMyStatus("Done");
        }

    }
}
