using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petabridge.App.Actors
{
    internal class DevicesRouter : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine($"DevicesRouter path: {Self.Path}, message: {message}");
            Sender.Tell("recibido: "+message);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new DevicesRouter());
        }
    }
}
