using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Akka.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _remoteActor;

        public ActorsController()
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    dot-netty.tcp {
                        hostname = ""127.0.0.1""
                        port = 0
                    }
                }
            }");
            _actorSystem = ActorSystem.Create("ClusterSys", config);

            var remoteAddress = Address.Parse("akka.tcp://ClusterSys@127.0.0.1:9221");
            _remoteActor = _actorSystem.ActorSelection(remoteAddress + "/user/devices").ResolveOne(TimeSpan.FromSeconds(3)).Result;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string message = "algo";
            return await _remoteActor.Ask<string>(message);
        }
    }
}
