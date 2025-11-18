using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoMcpServer;

var builder = Host.CreateApplicationBuilder(args);

// Logging: todo va por STDERR, stdout queda limpio para el protocolo MCP
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// HttpClient para la API de TODO
builder.Services.AddHttpClient<TodoApiClient>((sp, client) =>
{
    var baseUrl = Environment.GetEnvironmentVariable("TODO_API_BASE_URL")
                 ?? "http://localhost:5083";

    if (!baseUrl.EndsWith("/"))
        baseUrl += "/";

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Registrar MCP server con transporte STDIO y tools del assembly
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(); // escanea [McpServerToolType] en el assembly actual

// Ejecutar el host (el server MCP se encarga solo por dentro)
await builder.Build().RunAsync();
