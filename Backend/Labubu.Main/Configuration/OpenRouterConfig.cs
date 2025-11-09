namespace Labubu.Main.Configuration;

public class OpenRouterConfig
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "openai/chatgpt-4o-latest";
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
}

