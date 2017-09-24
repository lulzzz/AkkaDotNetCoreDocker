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
using System.Threading.Tasks;

namespace Loaner.api.Controllers
{
    public class AccountModule : NancyModule
    {
        public AccountModule()
        {
            Get("api/account/{actorName}/info", async args =>
            {
                try
                {
                    string account = args.actorName;
                    string path = $@"/user/demo-supervisor/{account}";
                    var system = ActorSystemRefs
                        .ActorSystem
                        .ActorSelection(path)
                        .ResolveOne(TimeSpan.FromSeconds(3)).Result;


                    if (system.IsNobody())
                    {
                        throw new ActorNotFoundException();
                    }
                    var response = await Task.Run(
                        () => system.Ask<ThisIsMyInfo>(new TellMeYourInfo(), TimeSpan.FromSeconds(3))
                      );
                    response.Info.AuditLog.Sort((x, y) => x.EventDate >= y.EventDate ? 1 : -1);
                    return Response.AsJson(new AccountStateViewModel(response.Info));
                }
                catch (ActorNotFoundException)
                {
                    return new AccountStateViewModel($"{args.actorName} is not running at the moment");
                }
                catch (Exception e)
                {
                    return new AccountStateViewModel($"{args.actorName} {e.Message}");

                }
            });

            Get("api/account/{actorName}", async args =>
             {
                 try
                 {
                     var system = ActorSystemRefs
                         .ActorSystem
                         .ActorSelection($"akka://demo-system/user/demo-supervisor/{args.actorName}")
                         .ResolveOne(TimeSpan.FromSeconds(3));
                     if (system.Exception != null)
                     {
                         throw system.Exception;
                     }
                     var response = await Task.Run(
                         () => system.Result.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(1))
                         );
                     return Response.AsJson(response.Message);
                 }
                 catch (ActorNotFoundException)
                 {
                     return new AccountStateViewModel($"{args.actorName} is not running at the moment");
                 }
                 catch (Exception e)
                 {
                     return new AccountStateViewModel($"{args.actorName} {e.Message}");

                 }
             });

            Get("api/account/{actorName}/assessment", args => new InvoiceLineItem(FinancialConcept.Tax, 0, 0, 0));

            Post("api/account/{actorName}/assessment", async args =>
            {
                string account = args.actorName;
                SimulateAssessmentModel assessment = this.Bind<SimulateAssessmentModel>();
                try
                {
                    var domanCommand = new BillingAssessment(account, assessment.LineItems);
                    var system = ActorSystemRefs
                        .ActorSystem
                        .ActorSelection($"akka://demo-system/user/demo-supervisor/{account}")
                        .ResolveOne(TimeSpan.FromSeconds(3));
                    if (system.Exception != null)
                    {
                        throw system.Exception;
                    }
                    var response = await Task.Run(
                        () => system.Result.Ask<ThisIsMyStatus>(domanCommand, TimeSpan.FromSeconds(2))
                    );
                    return Response.AsJson(response.Message);
                }
                catch (ActorNotFoundException)
                {
                    return new AccountStateViewModel($"{args.actorName} is not running at the moment");
                }
                catch (Exception e)
                {
                    return new AccountStateViewModel($"{args.actorName} {e.Message}");

                }
            });
        }
    }
}
