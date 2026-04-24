namespace EntangoApi.Models;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SystemPrompt { get; set; }
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    public float Temperature { get; set; } = 0.7f;
}

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public double TokensUsed { get; set; }
}