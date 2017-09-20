using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loaner.BoundedContexts.Test
{
    public class LittleActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        public LittleActor()
        {
            Become(Awake);
        }
        public void Awake()
        {
            Console.WriteLine($"I am awake!");
            Receive<Hello>(msg =>
            {
                Stash.UnstashAll();
                Console.WriteLine($"{msg.Message}");
                Sender.Tell(new Hello());
            });
            Receive<Goodbye>(msg =>
            {
                Become(Asleep);
            });
        }

        public void Asleep()
        {
            Console.WriteLine($"I am awake!");
            Receive<Hello>(msg =>
            {
                Become(Awake);
            });
            Receive<Goodbye>(msg =>
            {
                Stash.Stash();
            });
            
        }
    }
}
