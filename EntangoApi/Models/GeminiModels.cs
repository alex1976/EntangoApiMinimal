namespace EntangoApi.Models;

public class GeminiRequest
{
    public string Message { get; set; } = string.Empty;
    public float Temperature { get; set; } = 0.7f;
    public int MaxOutputTokens { get; set; } = 1000;
    public string Model { get; set; } = "gemini-pro";
}

public class GeminiResponse
{
    public string Message { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}