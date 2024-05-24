using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petabridge.App.Actors
{
    internal class DeviceB : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine($"DeviceB path: {Self.Path}, message: {message}");
        }
    }
}
