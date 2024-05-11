FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

COPY ["src/ReverseProxy/ApiGateway/ApiGateway.csproj", "src/ReverseProxy/ApiGateway/"]
COPY ["src/DistributedSystem.Contract/DistributedSystem.Contract.csproj", "src/DistributedSystem.Contract/"]

RUN dotnet restore "./src/ReverseProxy/ApiGateway/./ApiGateway.csproj"

COPY . .

WORKDIR "/src/src/ReverseProxy/ApiGateway"
RUN dotnet build "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]