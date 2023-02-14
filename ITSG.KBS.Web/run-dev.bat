@echo off
set DOTNET_ENVIRONMENT=Development
set ASPNETCORE_URLS=https://localhost:7012;http://localhost:5276
dotnet watch run