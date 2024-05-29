namespace AKKA.ClusterNode.Configs;

internal interface IConfig
{
    public string ActorSystemName { get; }
    public string Hostname { get; }
    public int Port { get; }
    public string[]? Roles { get; }
    public string[]? SeedNodes { get; }
}
internal class EnvironmentConfig : IConfig
{
    public EnvironmentConfig() 
    {
        this.ActorSystemName = Environment.GetEnvironmentVariable("AKKACN_ACTORSYSTEMNAME") ?? string.Empty;
        this.Hostname = Environment.GetEnvironmentVariable("AKKACN_HOSTNAME") ?? string.Empty;
        this.Port = int.Parse(Environment.GetEnvironmentVariable("AKKACN_PORT") ?? "0");
        this.Roles = (Environment.GetEnvironmentVariable("AKKACN_ROLES") ?? string.Empty).Split(",");
        this.SeedNodes = (Environment.GetEnvironmentVariable("AKKACN_SEEDNODES") ?? string.Empty).Split(",");
    }

    public string ActorSystemName { get; private set; }
    public string Hostname { get; private set; }
    public int Port { get; private set; }
    public string[]? Roles { get; private set; }
    public string[]? SeedNodes { get; private set; }
}
