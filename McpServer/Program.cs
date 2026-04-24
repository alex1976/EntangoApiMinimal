using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

// Transport mode: pass --stdio as a CLI argument for local stdio mode,
// or set MCP_TRANSPORT=stdio as an environment variable.
// Default is HTTP/HTTPS mode (for deployed or remote access).
var transportMode = args.Contains("--stdio")
    ? "stdio"
    : (Environment.GetEnvironmentVariable("MCP_TRANSPORT") ?? "http").ToLowerInvariant();

if (transportMode == "stdio")
{
    // ── Stdio (local) mode ────────────────────────────────────────────────
    // stdout is reserved for the MCP protocol; all logging must use stderr.
    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole(options =>
        options.LogToStandardErrorThreshold = LogLevel.Trace);

    builder.Services.AddHttpClient();
    builder.Services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();

    await builder.Build().RunAsync();
}
else
{
    // ── HTTP / HTTPS mode ─────────────────────────────────────────────────
    var builder = WebApplication.CreateBuilder(args);

    var endpointPath = builder.Configuration["McpServer:EndpointPath"] ?? "/mcp";
    var httpUrl = builder.Configuration["McpServer:HttpUrl"];
    var httpsUrl = builder.Configuration["McpServer:HttpsUrl"];

    var listenUrls = new List<string>();
    if (!string.IsNullOrWhiteSpace(httpUrl))
        listenUrls.Add(httpUrl);
    if (!string.IsNullOrWhiteSpace(httpsUrl))
        listenUrls.Add(httpsUrl);
    if (listenUrls.Count > 0)
        builder.WebHost.UseUrls(listenUrls.ToArray());

    builder.Services.AddHttpClient();
    builder.Services.AddMcpServer()
        .WithHttpTransport()
        .WithToolsFromAssembly();

    var app = builder.Build();

    // Expose MCP over HTTP/S at the configured endpoint path.
    app.MapMcp(endpointPath);

    await app.RunAsync();
}
