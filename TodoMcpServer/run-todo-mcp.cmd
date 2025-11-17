@echo off
setlocal

REM URL de tu API TODO (usa https porque tu API está en https://localhost:7027)
set TODO_API_BASE_URL=http://localhost:5083

REM Ir a la carpeta del proyecto MCP
cd /d "C:\Users\Acer\Desktop\Laburo\crunch\dotnet-interview\TodoMcpServer"

REM Ejecutar el DLL ya compilado
dotnet "bin\Debug\net8.0\TodoMcpServer.dll"

endlocal
