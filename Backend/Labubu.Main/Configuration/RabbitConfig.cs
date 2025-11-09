namespace Labubu.Main.Configuration;

public class RabbitConfig
{
    public string Host { get; set; } = null!;
    public ushort Port { get; set; }
    public string VHost { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Pass { get; set; } = null!;
}

