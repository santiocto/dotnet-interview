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
3. Create the schema via EF Core (`dotnet ef database update --project TodoDataAccess`) or run your SQL scripts against `localhost,1433`.

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

## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).
