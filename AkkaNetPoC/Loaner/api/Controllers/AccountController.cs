using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Akka.Actor;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;
using Loaner.api.ActorManagement;
using Loaner.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Loaner.api.Controllers
{
    public class AccountController : Controller
    {
        [Route("api/account")]
        [HttpGet]
        public string Account()
        {
            return "Which account are you looking for?";
        }


        [Route("api/account/{actorName}/info")]
        [HttpGet]
        public AccountStateViewModel AccountDetails(string actorName)
        {
            var system = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor/{actorName}").ResolveOne(TimeSpan.FromSeconds(1));
            if (system.Exception != null)
            {
                return new AccountStateViewModel($"{actorName} is not running at the moment");
            }
            var response = system.Result.Ask<ThisIsMyInfo>(new TellMeYourInfo(), TimeSpan.FromSeconds(3)).Result;
            response.Info.AuditLog.Sort((x, y) => x.EventDate >= y.EventDate ? 1 : -1);

            return new AccountStateViewModel(response.Info);
        }

        [Route("api/account/{actorName}")]
        [HttpGet]
        public async Task<string> Account(string actorName)
        {
            var system = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor/{actorName}").ResolveOne(TimeSpan.FromSeconds(1));
            if (system.Exception != null)
            {
                return $"{actorName} is not running at the moment";
            }
            var response = await system.Result.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(1));
            return response.Message;
        }

        [Route("api/account/{actorName}/assessment")]
        [HttpGet]
        public InvoiceLineItem GetInvoiceLineItem(string actorName)
        {
            return new InvoiceLineItem(FinancialConcept.Tax, 0, 0, 0);
        }

        [Route("api/account/{actorName}/assessment")]
        [HttpPost]
        public async Task<string> AccountPayment(string actorName, [FromBody]SimulateAssessmentModel Assessment)
        {
            var system = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor/{actorName}").ResolveOne(TimeSpan.FromSeconds(10));
            if (system.Exception != null)
            {
                return $"{actorName} is not running at the moment";
            }
            ImmutableList<InvoiceLineItem> items = ImmutableList.Create<InvoiceLineItem>(Assessment.LineItems.ToArray());

            var domanCommand = new BillingAssessment(actorName, items);
            var response = await system.Result.Ask<ThisIsMyStatus>(domanCommand, TimeSpan.FromSeconds(10));
            return response.Message;
        }
    }
}
