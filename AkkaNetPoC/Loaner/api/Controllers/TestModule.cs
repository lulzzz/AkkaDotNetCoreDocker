using Akka.Actor;
using Loaner.api.ActorManagement;
using Loaner.BoundedContexts.Test;
using Nancy;
using System;

namespace Loaner.api.Controllers
{
    public class TestModule : NancyModule
    {
        readonly IActorRef _littleActor = LoanerActors.LittleActor;

        public TestModule()
        {

            Get("/", args => "Hello from Nancy running on CoreCLR");

            Get("api/test", args => _littleActor.Ask<Hello>(new Hello("I am in the controller"), TimeSpan.FromDays(2)).Result);
        }
    }
}
