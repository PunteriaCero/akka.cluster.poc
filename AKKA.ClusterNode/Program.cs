using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Akka.Routing;
using AKKA.ClusterNode.Actors;
using AKKA.ClusterNode.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var gConfig = new EnvironmentConfig();

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();
        services.AddAkka(gConfig.ActorSystemName, (builder, provider) =>
        {
            builder.AddHoconFile("akka.hocon", HoconAddMode.Prepend)
                //.WithRemoting(gConfig.Hostname, gConfig.Port)
                .WithRemoting("0.0.0.0", 4055, gConfig.Hostname, gConfig.Port)
                .WithClustering(new ClusterOptions()
                {
                    Roles = gConfig.Roles,
                    SeedNodes = gConfig.SeedNodes
                })
                .WithActors((system, registry) =>
                {
                    var router = system.ActorOf(Props.Create<DeviceRouter>().WithRouter(FromConfig.Instance), "devices");
                    var deviceA = system.ActorOf<DeviceA>("DeviceA");
                    var deviceB = system.ActorOf<DeviceB>("DeviceB");
                });
        });
    })
    .ConfigureLogging((hostContext, configLogging) => { configLogging.AddConsole(); })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();