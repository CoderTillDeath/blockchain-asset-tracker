@echo off

msbuild /t:restore BlockchainApp.sln
dotnet build BlockchainAPI/BlockchainAPI.csproj
dotnet build BlockchainAPI.Tests/BlockchainAPI.Tests.csproj

cd BlockchainAPI.Tools

dotnet restore
dotnet minicover instrument --workdir ../ --assemblies BlockchainAPI.Tests/**/bin/**/*.dll --sources BlockchainAPI/**/*.cs
dotnet minicover reset

cd ..

dotnet test --no-build BlockchainAPI.Tests/BlockchainAPI.Tests.csproj

cd BlockchainAPI.Tools

dotnet minicover uninstrument --workdir ../

dotnet minicover htmlreport --workdir ../ --threshold 90

dotnet minicover report --workdir ../ --threshold 90

cd ..