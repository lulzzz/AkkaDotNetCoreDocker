using Akka.Actor;
using Loaner.api.ActorManagement;
using Loaner.api.Models;
using Loaner.BoundedContexts.MaintenanceBilling.Commands;
using Loaner.BoundedContexts.MaintenanceBilling.Events;
using Loaner.BoundedContexts.MaintenanceBilling.Models;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Immutable;

namespace Loaner.api.Controllers
{
    public class AccountModule : NancyModule
    {
        public AccountModule()
        {
            Get("api/account/{actorName}/info", async args =>
            {
                var system = ActorSystemRefs
                    .ActorSystem
                    .ActorSelection($"akka://demo-system/user/demo-supervisor/{args.actorName}").ResolveOne(TimeSpan.FromSeconds(1));
                if (system.Exception != null)
                {
                    return new AccountStateViewModel($"{args.actorName} is not running at the moment");
                }
                var response = await system.Result.Ask<ThisIsMyInfo>(new TellMeYourInfo(), TimeSpan.FromSeconds(3));
                response.Info.AuditLog.Sort((x, y) => x.EventDate >= y.EventDate ? 1 : -1);

                return Response.AsJson(new AccountStateViewModel(response.Info));
                //return View["staticview", this.Request.Url];
            });

            Get("api/account/{actorName}",async args =>
            {
                var system = ActorSystemRefs
                    .ActorSystem
                    .ActorSelection($"akka://demo-system/user/demo-supervisor/{args.actorName}").ResolveOne(TimeSpan.FromSeconds(1));
                if (system.Exception != null)
                {
                    return $"{args.actorName} is not running at the moment";
                }
                var response = await system.Result.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(1));
                return Response.AsJson(response.Message);
            });

            Get("api/account/{actorName}/assessment", args => new InvoiceLineItem(FinancialConcept.Tax, 0, 0, 0));

            Post("api/account/{actorName}/assessment", async args =>
            {
                SimulateAssessmentModel assessment = this.Bind<SimulateAssessmentModel>();
                var system = ActorSystemRefs
                    .ActorSystem
                    .ActorSelection($"akka://demo-system/user/demo-supervisor/{args.actorName}").ResolveOne(TimeSpan.FromSeconds(10));
                if (system.Exception != null)
                {
                    return $"{args.actorName} is not running at the moment";
                }
                var domanCommand = new BillingAssessment( args.actorName, assessment.LineItems);
                var response = await system.Result.Ask<ThisIsMyStatus>(domanCommand, TimeSpan.FromSeconds(10));
                return Response.AsJson(response.Message);

            });
        }
    }
}
