FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

RUN apt-get -qq update && apt-get -qqy --no-install-recommends install wget gnupg \
    git \
    unzip

RUN curl -sL https://deb.nodesource.com/setup_6.x |  bash -
RUN apt-get install -y nodejs


FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY BlockchainWebApp/BlockchainWebApp.csproj BlockchainWebApp/
COPY BlockchainAPI/BlockchainAPI.csproj BlockchainAPI/
RUN dotnet restore
COPY . .
WORKDIR /src/BlockchainWebApp
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlockchainWebApp.dll"]
