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

            _remoteActor = this.GetActorRef(Address.Parse("akka.tcp://AKKACluster@192.168.1.36:9001"));
            if (_remoteActor == null)
                _remoteActor = this.GetActorRef(Address.Parse("akka.tcp://AKKACluster@192.168.1.36:9002"));

            if (_remoteActor == null)
                throw new Exception("There is no nodes available in the cluster");
        }

        private IActorRef GetActorRef(Akka.Actor.Address path)
        {
            try
            {
                return _actorSystem.ActorSelection(path + "/user/devices").ResolveOne(TimeSpan.FromSeconds(3)).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet]
        public async Task<string> Get(string message)
        {
            return await _remoteActor.Ask<string>(message);
        }
    }
}
