using System.Diagnostics;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.DependencyInjection;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Akka.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Petabridge.App.Actors;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var host = new HostBuilder()
    .ConfigureHostConfiguration(builder =>
        builder.AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true))
    .ConfigureServices((hostContext, services) =>
    {
        var akkaConfig = hostContext.Configuration.GetRequiredSection(nameof(AkkaClusterConfig))
            .Get<AkkaClusterConfig>();
        services.AddLogging();
        services.AddAkka(akkaConfig.ActorSystemName, (builder, provider) =>
        {
            Debug.Assert(akkaConfig.Port != null, "akkaConfig.Port != null");
            builder.AddHoconFile("app.conf", HoconAddMode.Prepend)
                .WithRemoting(akkaConfig.Hostname, akkaConfig.Port.Value)
                .WithClustering(new ClusterOptions()
                {
                    Roles = akkaConfig.Roles,
                    SeedNodes = akkaConfig.SeedNodes
                })
                .AddPetabridgeCmd(cmd =>
                {
                    cmd.RegisterCommandPalette(new RemoteCommands());
                    cmd.RegisterCommandPalette(ClusterCommands.Instance);

                    // sharding commands, although the app isn't configured to host any by default
                    cmd.RegisterCommandPalette(ClusterShardingCommands.Instance);
                })
                .WithActors((system, registry) =>
                {
                    var router = system.ActorOf(Props.Create<DevicesRouter>().WithRouter(FromConfig.Instance), "devices");
                    var deviceA = system.ActorOf<DeviceA>("DeviceA");
                    var deviceB = system.ActorOf<DeviceB>("DeviceB");
                });
        });
    })
    .ConfigureLogging((hostContext, configLogging) => { configLogging.AddConsole(); })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();