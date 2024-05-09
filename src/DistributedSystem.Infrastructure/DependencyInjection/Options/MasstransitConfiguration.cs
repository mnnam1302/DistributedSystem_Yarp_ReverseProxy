namespace DistributedSystem.Infrastructure.DependencyInjection.Options;

public class MasstransitConfiguration
{
    public string Host { get; set; }

    public string VHost { get; set; }

    public ushort Port { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}
