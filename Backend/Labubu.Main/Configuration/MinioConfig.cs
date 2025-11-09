namespace Labubu.Main.Configuration;

public class MinioConfig
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = "labubu-clothes";
    public bool UseSSL { get; set; } = false;
}

