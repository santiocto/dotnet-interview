# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

API de Todo Lists en .NET 8. Se usa para candidatos full-stack .NET.

## Arquitectura (capas)

Se eligió una arquitectura por capas para mantener la mantenibilidad sin agregar proyectos innecesarios:

- `TodoApi`: Web API (controllers, DI, Swagger).
- `TodoLogic`: servicios de dominio que orquestan repositorios.
- `TodoDataAccess`: EF Core DbContext, repositorios y migraciones.
- `TodoDomain`: entidades y DTOs compartidos.
- `TodoApi.Tests`: tests MSTest/Moq + EF InMemory (no requiere SQL Server real).
- `TodoMcpServer`: servidor MCP que expone la API como tools vía STDIO.

## Base de datos

El devcontainer levanta un SQL Server. Si no usas el devcontainer, provisiona tu propio SQL Server y actualiza la connection string.

### Levantar un contenedor de SQL Server

1. Levanta el contenedor (ejemplo en `cmd.exe`):
   ```cmd
   docker run -d --name crunchInterview ^
     -e "ACCEPT_EULA=Y" ^
     -e "MSSQL_SA_PASSWORD=Password123!" ^
     -p 1433:1433 ^
     mcr.microsoft.com/mssql/server:2022-latest
   ```
   En Linux/macOS usa `\` en vez de `^` o ponlo en una sola línea.
2. Actualiza `TodoApi/appsettings.Development.json` con la nueva cadena, por ejemplo:
   ```json
   "TodoContext": "Server=localhost,1433;Database=CrunchInterview;User Id=sa;Password=Password123!;TrustServerCertificate=True;"
   ```
3. Crea el esquema con EF Core:
   ```bash
   dotnet ef database update --project TodoDataAccess --startup-project TodoApi
   ```
   Las migraciones están en `TodoDataAccess/Migrations`.

Con la base corriendo puedes ejecutar `dotnet run --project TodoApi` y `dotnet test TodoApi.Tests` en tu host.

#### Comandos útiles de Docker

- `docker ps --filter "name=crunchInterview"` → confirma que está corriendo.
- `docker start crunchInterview` / `docker stop crunchInterview` → lo manejas manualmente.
- `docker logs crunchInterview --tail 50` → logs de arranque.
- `sqlcmd -S localhost,1433 -U sa -P Password123! -C` → consola SQL (usa `-Q` para one-liners).

#### Consultas rápidas

- `sqlcmd -S localhost,1433 -U sa -P Password123! -C -Q "SELECT name FROM sys.databases;"` → confirma `CrunchInterview`.
- `sqlcmd -S localhost,1433 -U sa -P Password123! -C -Q "SELECT TOP (20) Id, Name FROM CrunchInterview.dbo.TodoList ORDER BY Id DESC;"` → muestra datos.

## Build

```bash
dotnet build
```

## Ejecutar la API

```bash
dotnet run --project TodoApi
```

## Tests

```bash
dotnet test
```

Tests de integración adicionales: https://github.com/crunchloop/interview-tests

## Prueba rápida de API (curl)

`dotnet run --project TodoApi` imprime URLs HTTP/HTTPS (HTTP suele ser `http://localhost:5083`). Ajusta el puerto según salida de consola y prueba:

- `curl -X GET http://localhost:5083/api/todolists`
- `curl -X POST http://localhost:5083/api/todolists -H "Content-Type: application/json" -d "{\"name\":\"Groceries\"}"`
- `curl -X GET http://localhost:5083/api/todolists/1`
- `curl -X PUT http://localhost:5083/api/todolists/1 -H "Content-Type: application/json" -d "{\"name\":\"Updated Groceries\"}"`
- `curl -X DELETE http://localhost:5083/api/todolists/1`

## MCP

El servidor MCP en `TodoMcpServer` expone los endpoints de la API como tools (STDIO, `ModelContextProtocol.Server`).

- Corre la API (`dotnet run --project TodoApi`) en `https://localhost:7027` o setea `TODO_API_BASE_URL` para otro host.
- Arranca el MCP: `dotnet run --project TodoMcpServer`. Se anuncia como `todo-api-mcp` y carga tools del ensamblado.
- Tools: `get_todo_lists`, `get_todo_list_by_id`, `create_todo_list`, `update_todo_list`, `delete_todo_list`, `get_todo_item`, `create_todo_item`, `update_todo_item`, `complete_todo_item`, `delete_todo_item`. Devuelven JSON crudo (o `bool` en deletes).

### Cómo probar MCP

1. API en ejecución.
2. (Opcional) Define URL base:
   ```cmd
   set TODO_API_BASE_URL=http://localhost:7027
   ```
3. Ejecuta MCP (desarrollo):
   ```cmd
   dotnet run --project TodoMcpServer
   ```
4. Publica para usar con un cliente MCP:
   ```cmd
   dotnet publish TodoMcpServer -c Release -o ./TodoMcpServer/publish
   ```
   En Claude Desktop configura una tool local apuntando a `...\\TodoMcpServer\\publish\\TodoMcpServer.exe` (transporte STDIO).
5. Ejemplo de prompt que el cliente resuelve encadenando tools:
   - "Crear un ítem en la lista 'Trabajo' con la descripción 'Terminar informe'."
   - Flujo: `get_todo_lists` → busca `name == "Trabajo"` → `create_todo_item` con el `id` encontrado.

Prueba manual (sin MCP), ajusta puerto si cambia:

```cmd
curl -X GET http://localhost:7027/api/todolists
curl -X POST http://localhost:7027/api/todolists -H "Content-Type: application/json" -d "{\"name\":\"Trabajo\"}"
curl -X GET http://localhost:7027/api/todolists/1
curl -X POST http://localhost:7027/api/todolists/1/items -H "Content-Type: application/json" -d "{\"description\":\"Terminar informe\"}"
```

### Claude Desktop — configuración y script

Archivo de ejemplo `%APPDATA%\Claude\claude_desktop_config.json`:

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

Notas:
- `command`: ruta al script o ejecutable. Debe correr en primer plano para que STDIO quede conectado.
- `env`: útil para `TODO_API_BASE_URL`.

Ejemplos de `run-todo-mcp.cmd` (ubicar en `TodoMcpServer\run-todo-mcp.cmd`):

Publicada:
```cmd
@echo off
set "TODO_API_BASE_URL=http://localhost:7027"
"%~dp0\publish\TodoMcpServer.exe"
```

Desarrollo:
```cmd
@echo off
set "TODO_API_BASE_URL=http://localhost:7027"
cd /d "%~dp0"
dotnet run --project .
```

Consejos:
- Define `TODO_API_BASE_URL` en el JSON o dentro del `.cmd`.
- No uses `start`; el proceso debe quedar en primer plano para mantener STDIO.
- Para publicar Windows x64 específico:
  ```cmd
  dotnet publish TodoMcpServer -c Release -r win-x64 --self-contained false -o ./TodoMcpServer/publish
  ```

Diagnóstico:
- Verifica el proceso con `tasklist /FI "IMAGENAME eq TodoMcpServer.exe"`.
- Revisa logs de la API con `docker logs -f crunchInterview` si usas la DB en contenedor.

## Notas clave

- Migraciones: `dotnet ef migrations add <Nombre> --project TodoDataAccess --startup-project TodoApi` para generar; `dotnet ef database update --project TodoDataAccess --startup-project TodoApi` para aplicar. Ubicación: `TodoDataAccess/Migrations`.
- Seeding: no hay datos iniciales; crea listas e ítems manualmente (curl, Postman o MCP).
- Plataforma: el script `TodoMcpServer/run-todo-mcp.cmd` es para Windows (CMD + rutas absolutas). En otros SO ejecuta la DLL/EXE publicada o `dotnet run --project TodoMcpServer` y apunta la configuración de Claude al ejecutable siguiendo el JSON de ejemplo.
- Claude: la configuración de Claude Desktop debe correr la DLL/EXE (o el `.cmd`) en primer plano para descubrir y ejecutar las tools vía STDIO.
- Excepciones: se añadió un exception filter para centralizar errores, pero no se incluyeron excepciones personalizadas para no afectar los tests actuales; queda pendiente a futuro.
- Validaciones: la lógica no incorpora validaciones de negocio (longitudes, campos vacíos, etc.) porque no aportaban valor en este ejemplo y se mantuvo simple.
- Assets: colección de Postman en `Documentation/PostmanCollection/TodoApi.postman_collection.json` y capturas de uso con Claude en `Documentation/Photos/Claude*.png` mostrando el flujo MCP.

## Contacto

- Martín Fernández (mfernandez@crunchloop.io)

## Sobre Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

Creemos en retribuir y colaborar :rocket:. Hablemos [`Get in touch`](https://crunchloop.io/contact).
