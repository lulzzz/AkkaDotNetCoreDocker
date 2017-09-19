using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Akka.Actor;
using Loaner.BoudedContexts.Test;
using Loaner.api.ActorManagement;

namespace Loaner.api.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        ActorSystem _system = ActorSystemRefs.ActorSystem;
        IActorRef _littleActor = LoanerActors.LittleActor;


        [HttpGet]
        public IEnumerable<Hello> Get()
        {
            yield return _littleActor.Ask<Hello>(new Hello("I am in the controller"), TimeSpan.FromDays(2)).Result;
        }

    }
}
