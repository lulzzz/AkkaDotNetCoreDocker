using System;
using Akka.Actor;
using Akka.Event;

namespace AkkaDotNetCoreDocker
{
    public class LoggingActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);

        public LoggingActor()
        {
            this.Receive<string>(s =>
            {
                Console.WriteLine(s);
                logger.Info(s);
            });
        }
    }

}

