# EntangoApi

the purpose of this project is to serve as a backend API to expose capability to Claude Desktop, but it can also be used independently as a general-purpose utility API for construction-related applications.
The comunication with Claude Desktop is done through the MCP protocol, which allows exposing specific tools (in this case, Price List and Document Store) that can be consumed directly within the Claude interface.

## Overview

ASP.NET Core Web API project that exposes utility and domain APIs for:
- Test/info endpoint
- SMS sending
- Email sending
- AI chat (OpenAI and Gemini)
- Document extraction with Gemini
- Weather forecast (Open-Meteo)
- Price list CRUD and search
- Document store CRUD and download

The project uses PostgreSQL with Entity Framework Core and includes Swagger/OpenAPI.

## Prerequisites

- .NET 9 SDK
- PostgreSQL Server (or Dockerized PostgreSQL)
- Docker and Docker Compose (optional)

## Getting Started

### 1. Clone the repository

```bash
git clone [your-repository-url]
cd EntangoApi
```

### 2. Configure connection string

Set `ConnectionStrings:DefaultConnection` in `EntangoApi/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EntangoApi;Username=postgres;Password=your_password"
  }
}
```

### 3. Configure external providers (as needed)

- `OpenAI:ApiKey`
- `Gemini:ApiKey`
- `Smtp:*` for email sending
- `Sms:*` for SMS provider settings

### 4. Build and run

```bash
dotnet build
dotnet run --project EntangoApi/EntangoApi.csproj
```

Default local URLs:
- HTTP: `http://localhost:5162`
- HTTPS: `https://localhost:7264`

Swagger UI (development):
- `http://localhost:5162/swagger`

## MCP Server

The repository includes a dedicated MCP server project in `McpServer/` that exposes PriceList and DocumentStore tools. It supports two transport modes:

| Mode | Use case |
|------|----------|
| **stdio** | Local process — Claude Desktop spawns the server directly |
| **HTTP/HTTPS** | Remote / deployed — clients connect over a URL |

### Run in HTTP/HTTPS mode (default)

```bash
dotnet run --project McpServer/PriceListMcpServer.csproj
```

Default MCP endpoints:
- HTTP: `http://localhost:5099/mcp`
- HTTPS: `https://localhost:7099/mcp`

These values are configured in `McpServer/appsettings.json` under:
- `McpServer:HttpUrl`
- `McpServer:HttpsUrl`
- `McpServer:EndpointPath`

### Run in stdio mode (local)

Pass `--stdio` as a command-line argument:

```bash
dotnet run --project McpServer/PriceListMcpServer.csproj -- --stdio
```

Or set the environment variable `MCP_TRANSPORT=stdio` before running.

In stdio mode all logging is written to stderr; stdout is reserved for the MCP protocol.

### Configure Claude Desktop

A ready-made `McpServer/claude_desktop_config.json` is provided. Copy the relevant entry into your Claude Desktop `claude_desktop_config.json`:

**Option A — stdio (local, no server process needed):**

```json
{
  "mcpServers": {
    "pricelist-local": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\path\\to\\EntangoApi\\McpServer\\PriceListMcpServer.csproj",
        "--",
        "--stdio"
      ],
      "env": {
        "ENTANGO_API_BASE_URL": "http://localhost:5162"
      }
    }
  }
}
```

**Option B — HTTP (server must be running):**

```json
{
  "mcpServers": {
    "pricelist-http": {
      "url": "http://localhost:5099/mcp"
    }
  }
}
```

**Option C — HTTPS (server must be running, dev cert must be trusted):**

```json
{
  "mcpServers": {
    "pricelist-https": {
      "url": "https://localhost:7099/mcp"
    }
  }
}
```

To trust the local ASP.NET Core development certificate run:

```bash
dotnet dev-certs https --trust
```

## API Reference

All routes below are relative to the API root.

### Test

- `GET /Test/info`
  - Returns a plain text info message listing available API domains.
  - Responses: `200 OK`

### Weather Forecast

- `GET /WeatherForecast/{location}?latitude={latitude}&longitude={longitude}`
  - Calls Open-Meteo and returns the full Open-Meteo response shape.
  - Path params:
    - `location` (string, required)
  - Query params:
    - `latitude` (double, required, valid range `-90..90`)
    - `longitude` (double, required, valid range `-180..180`)
  - Responses:
    - `200 OK` with Open-Meteo payload
    - `400 Bad Request` for invalid coordinates
    - `5xx` on upstream/service errors

### OpenAI

- `POST /OpenAI/chat`
  - Sends a chat completion request to OpenAI.
  - Body (`application/json`):

```json
{
  "message": "Explain reinforced concrete in simple words",
  "systemPrompt": "You are a concise engineering assistant.",
  "modelName": "gpt-3.5-turbo",
  "temperature": 0.7
}
```

  - Responses:
    - `200 OK`

```json
{
  "message": "...",
  "model": "gpt-3.5-turbo-0125",
  "tokensUsed": 123
}
```

    - `500` if API key is missing
    - Upstream status code if OpenAI returns an error

### Gemini

- `POST /Gemini/chat`
  - Sends a text prompt to Gemini.
  - Body (`application/json`):

```json
{
  "message": "Summarize this construction spec",
  "temperature": 0.7,
  "maxOutputTokens": 1000,
  "model": "gemini-pro"
}
```

  - Responses:
    - `200 OK`

```json
{
  "message": "...",
  "model": "gemini-pro"
}
```

    - `500` if API key is missing
    - Upstream status code if Gemini returns an error

- `POST /Gemini/docextract`
  - Extracts structured data from an uploaded PDF using Gemini.
  - Content type: `multipart/form-data`
  - Form fields:
    - `Pdf` (file, required)
    - `SchemaJson` (string, required)
    - `Model` (string, optional, default `text-bison-001`)
    - `Temperature` (double, optional, default `0.0`)
    - `MaxOutputTokens` (int, optional, default `2048`)
  - Responses:
    - `200 OK` with extracted JSON if model output parses as JSON
    - `400 Bad Request` for missing/invalid inputs
    - `422 Unprocessable Entity` if model returns non-JSON text
    - `500` if API key is missing or on internal errors

### Mail

- `POST /Mail/send`
  - Sends an email through configured SMTP service.
  - Body (`application/json`):

```json
{
  "recipients": ["user@example.com"],
  "subject": "Project update",
  "body": "The report is ready.",
  "priority": 1
}
```

  - `priority` enum: `0=Low`, `1=Normal`, `2=High`
  - Responses:
    - `200 OK` when email is sent
    - `400 Bad Request` for validation errors
    - `500` on mail service errors

### SMS

- `POST /Sms/send`
  - Sends or queues SMS messages via configured provider.
  - Body (`application/json`):

```json
{
  "recipients": ["+391234567890"],
  "message": "Your appointment is confirmed.",
  "from": "+390000000000"
}
```

  - `from` is optional; if omitted, configured default is used.
  - Responses:
    - `200 OK`
    - `400 Bad Request` for validation/provider argument errors
    - `500` on internal errors

### Price List

- `GET /PriceList`
  - Returns all price list items.
  - Responses: `200 OK`

- `POST /PriceList/search`
  - Filters price list records.
  - Body (`application/json`):

```json
{
  "authorContains": "mario",
  "descriptionContains": "cement",
  "classificationEquals": "A1",
  "page": 1,
  "pageSize": 20
}
```

  - Responses: `200 OK`, `400 Bad Request` when body is missing

- `GET /PriceList/{id}`
  - Gets one record by id.
  - Responses: `200 OK`, `404 Not Found`

- `POST /PriceList`
  - Creates a record.
  - Body: `CreatePriceListRequest` JSON (same fields as entity except `PriceListId`).
  - Required fields include: `productId`, `productDescription`.
  - Responses: `201 Created`

- `PUT /PriceList/{id}`
  - Updates a record.
  - Body: `UpdatePriceListRequest` JSON including `priceListId` matching route id.
  - Responses: `204 No Content`, `400 Bad Request`, `404 Not Found`

- `DELETE /PriceList/{id}`
  - Deletes a record.
  - Responses: `204 No Content`, `404 Not Found`

### Document Store

- `GET /DocumentStore`
  - Returns metadata list of all documents.
  - Responses: `200 OK`

- `GET /DocumentStore/{id}`
  - Returns document metadata by id.
  - Responses: `200 OK`, `404 Not Found`

- `GET /DocumentStore/{id}/download`
  - Downloads file bytes for a document.
  - Responses: `200 OK` (binary file), `404 Not Found`

- `POST /DocumentStore`
  - Uploads a new document.
  - Content type: `multipart/form-data`
  - Form fields:
    - `DocumentDescription` (string, required)
    - `DocumentType` (string, optional)
    - `DocumentCategory` (string, optional)
    - `Document` (file, required)
  - Responses: `201 Created`, `400 Bad Request`

- `PUT /DocumentStore/{id}`
  - Updates document metadata and optionally file content.
  - Content type: `multipart/form-data`
  - Form fields:
    - `DocumentDescription` (string, required)
    - `DocumentType` (string, optional)
    - `DocumentCategory` (string, optional)
    - `Document` (file, optional)
  - Responses: `204 No Content`, `400 Bad Request`, `404 Not Found`

- `DELETE /DocumentStore/{id}`
  - Deletes a document.
  - Responses: `204 No Content`, `404 Not Found`

## Technologies

- ASP.NET Core (.NET 9)
- Entity Framework Core
- PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- Swagger/OpenAPI (`Swashbuckle.AspNetCore`)
