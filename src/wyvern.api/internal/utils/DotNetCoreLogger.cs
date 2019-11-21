using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.Logging;

namespace wyvern.utils
{
    /// <summary>
    /// Logger built to send akka logs to the ILogger implementation
    /// </summary>
    /// <remarks>
    /// NOTE: This will be a bit noisy since dotnet will add its line prefix ahead of
    /// akka's line prefix.
    /// </remarks>
    public class DotNetCoreLogger : ReceiveActor, ILogReceive
    {
        ILogger<DotNetCoreLogger> Logger { get; }

        public DotNetCoreLogger()
        {   
            var fac = new LoggerFactory();
            Logger = fac.CreateLogger<DotNetCoreLogger>();

            Receive<InitializeLogger>(_ => this.Init(Sender));

            Receive<Debug>(e => Logger.LogDebug(e.ToString()));
            Receive<Info>(e => Logger.LogInformation(e.ToString()));
            Receive<Warning>(e => Logger.LogWarning(e.ToString()));
            Receive<Error>(e => Logger.LogError(e.Cause, e.ToString()));
        }

        void Init(IActorRef sender)
        {
            sender.Tell(new LoggerInitialized());
        }
    }

}