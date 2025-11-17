# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

This is a simple Todo List API built in .NET 8. This project is currently being used for .NET full-stack candidates.

## Database

The project comes with a devcontainer that provisions a SQL Server database. If you are not going to use the devcontainer, make sure to provision a SQL Server database and
update the connection string.

### Running your own SQL Server container

If you prefer to run the API locally without the devcontainer, you can spin up a dedicated SQL Server container and point the app to it:

1. Start the database container (example using `cmd.exe`):
   ```
   docker run -d --name crunchInterview ^
     -e "ACCEPT_EULA=Y" ^
     -e "MSSQL_SA_PASSWORD=Password123!" ^
     -p 1433:1433 ^
     mcr.microsoft.com/mssql/server:2022-latest
   ```
   On Linux/macOS replace `^` with `\` or just write the command in a single line.
2. Update `TodoApi/appsettings.Development.json` to use the new connection string, e.g.
   ```
   "TodoContext": "Server=localhost,1433;Database=CrunchInterview;User Id=sa;Password=Password123!;TrustServerCertificate=True;"
   ```
3. Create the schema via EF Core (`dotnet ef database update --project TodoDataAccess --startup-project TodoApi`) or run your SQL scripts against `localhost,1433`.

With that container running you can execute `dotnet run --project TodoApi` and `dotnet test TodoApi.Tests` directly on your host machine.

#### Handy Docker commands

- `docker ps --filter "name=crunchInterview"` → confirm the container is running.
- `docker start crunchInterview` / `docker stop crunchInterview` → manage it manually.
- `docker logs crunchInterview --tail 50` → inspect SQL Server startup output.
- `sqlcmd -S localhost,1433 -U sa -P Password123! -C` → open a SQL prompt (end batches with `GO` or use `-Q` for one-liners).
#### Handy SQL queries

Example one-liners you can run from `cmd.exe`:

- `sqlcmd -S localhost,1433 -U sa -P Password123! -C -Q "SELECT name FROM sys.databases;"` → confirms that `CrunchInterview` exists.
- `sqlcmd -S localhost,1433 -U sa -P Password123! -C -Q "SELECT TOP (20) Id, Name FROM CrunchInterview.dbo.TodoList ORDER BY Id DESC;"` → inspects the sample data.

## Build

To build the application:

`dotnet build`

## Run the API

To run the TodoApi in your local environment:

`dotnet run --project TodoApi`

## Test

To run tests:

`dotnet test`

Check integration tests at: (https://github.com/crunchloop/interview-tests)

## API quick test (curl)

`dotnet run --project TodoApi` prints both HTTPS and HTTP URLs (HTTP defaults to `http://localhost:5083`). Replace the port below with whatever your console shows and run these one-liners from `cmd.exe`:

- `curl -X GET http://localhost:5083/api/todolists`
- `curl -X POST http://localhost:5083/api/todolists -H "Content-Type: application/json" -d "{\"name\":\"Groceries\"}"`
- `curl -X GET http://localhost:5083/api/todolists/1`
- `curl -X PUT http://localhost:5083/api/todolists/1 -H "Content-Type: application/json" -d "{\"name\":\"Updated Groceries\"}"`
- `curl -X DELETE http://localhost:5083/api/todolists/1`

## MCP

El repositorio incluye un servidor MCP en `TodoMcpServer` que expone los endpoints de la API como tools. Usa transporte STDIO y `ModelContextProtocol.Server`.

- Ejecuta la API (`dotnet run --project TodoApi`) y deja corriendo en `https://localhost:7027` o exporta `TODO_API_BASE_URL` para apuntar a otro host.
- Arranca el servidor MCP con `dotnet run --project TodoMcpServer`. Se anunciará con el nombre `todo-api-mcp` y carga los tools desde el ensamblado.
- Tools disponibles: `get_todo_lists`, `get_todo_list_by_id`, `create_todo_list`, `update_todo_list`, `delete_todo_list`, `get_todo_item`, `create_todo_item`, `update_todo_item`, `complete_todo_item`, `delete_todo_item`.
- Cada operación devuelve el JSON crudo de la API (o `bool` para deletes) para que el cliente MCP lo maneje como prefiera.

### MCP — Cómo probarlo

Pasos rápidos:
1. Asegúrate de que la Web API está en ejecución (por defecto):

```cmd
dotnet run --project TodoApi
```

2. (Opcional) Configura la URL base si tu API corre en otro puerto/host:

```cmd
set TODO_API_BASE_URL=http://localhost:7027
```

3. Compila y ejecuta el servidor MCP (modo desarrollo):

```cmd
dotnet run --project TodoMcpServer
```

4. Probar con un cliente MCP (recomendado: Claude Desktop u otro cliente compatible):

- Publica el servidor MCP para obtener un ejecutable que el cliente pueda lanzar:

```cmd
dotnet publish TodoMcpServer -c Release -o ./TodoMcpServer/publish
```

- En Claude Desktop (o cliente equivalente) crea una nueva "Local tool" o "External tool" y apunta al ejecutable que publicaste (`...\\TodoMcpServer\\publish\\TodoMcpServer.exe`). Asegúrate de que el transporte sea STDIO.

- Inicia la herramienta desde el cliente; el cliente deberá listar las tools disponibles (nombres y descripciones). A partir de ahí puedes enviar prompts en lenguaje natural y el cliente podrá decidir qué tools invocar y en qué orden.

Ejemplo de prompt que el cliente podrá resolver encadenando tools automáticamente:

"Crear un ítem en la lista 'Trabajo' con la descripción 'Terminar informe'"

Flujo que el cliente hará automáticamente:
- Invoca `get_todo_lists` para obtener el JSON de todas las listas.
- Busca la lista con `name == "Trabajo"` y extrae su `id`.
- Invoca `create_todo_item` con `todoListId` = id encontrada y `description` = "Terminar informe".

5. Prueba manual rápida (no usando MCP) — llamadas directas a la Web API con `curl` (útil para comprobar que la API responde):

Reemplaza el puerto si tu `dotnet run` imprime otro valor.

```cmd
curl -X GET http://localhost:7027/api/todolists
curl -X POST http://localhost:7027/api/todolists -H "Content-Type: application/json" -d "{\"name\":\"Trabajo\"}"
curl -X GET http://localhost:7027/api/todolists/1
curl -X POST http://localhost:7027/api/todolists/1/items -H "Content-Type: application/json" -d "{\"description\":\"Terminar informe\"}"
```

---

### MCP + Claude Desktop — configuración y script auxiliar

Qué se prueba con Claude Desktop:

- Descubrimiento de tools: Claude solicita la lista de tools y verifica nombres/descripciones.
- Encadenamiento: Claude puede planear pasos (ej. buscar la lista "Trabajo", extraer su id y crear un ítem).
- STDIO: Claude lanza el comando indicado en la configuración y se comunica por STDIN/STDOUT usando MCP.
- Cancelación: Claude puede cancelar una operación y las tools deben respetar `CancellationToken`.

Archivo de configuración (ejemplo): `%APPDATA%\\Claude\\claude_desktop_config.json`

```json
{
   "mcpServers": {
      "TodoMcpServer": {
         "command": "C:\\Users\\Acer\\Desktop\\Laburo\\crunch\\dotnet-interview\\TodoMcpServer\\run-todo-mcp.cmd",
         "args": [],
         "env": {
            "TODO_API_BASE_URL": "http://localhost:7027"
         }
      }
   }
}
```

Notas sobre la configuración:

- `command`: ruta absoluta al script o ejecutable que Claude lanzará. El proceso debe ejecutarse en primer plano para mantener STDIO conectado.
- `args`: argumentos opcionales.
- `env`: variables de entorno que Claude inyectará en el proceso (recomendado para `TODO_API_BASE_URL`).

Ejemplos de `run-todo-mcp.cmd` (coloca el archivo en `TodoMcpServer\\run-todo-mcp.cmd`):

Publicada (recomendado para uso con Claude):
```cmd
@echo off
set "TODO_API_BASE_URL=http://localhost:7027"
"%~dp0\\publish\\TodoMcpServer.exe"
```

Desarrollo (útil durante el desarrollo):
```cmd
@echo off
set "TODO_API_BASE_URL=http://localhost:7027"
cd /d "%~dp0"
dotnet run --project .
```

Consejos rápidos:

- Puedes definir `TODO_API_BASE_URL` directamente en el `env` del JSON o dentro del `.cmd`.
- No uses `start` sin mantener STDIO; el proceso debe quedar en primer plano para que Claude capture la entrada/salida.
- Si necesitas publicar para Windows x64 con runtime específico:

```cmd
dotnet publish TodoMcpServer -c Release -r win-x64 --self-contained false -o ./TodoMcpServer/publish
```

Diagnóstico:

- Comprueba que Claude lanzó el proceso con `tasklist /FI "IMAGENAME eq TodoMcpServer.exe"`.
- Revisa logs de la API con `docker logs -f crunchInterview` si usas la DB en contenedor.

Si quieres, puedo crear el `run-todo-mcp.cmd` en el repo y/o añadir el `claude_desktop_config.json` de ejemplo.



## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).
