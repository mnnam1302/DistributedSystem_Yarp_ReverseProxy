#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

COPY ["src/Authorization/Authorization.API/Authorization.API.csproj", "src/Authorization/Authorization.API/"]
COPY ["src/DistributedSystem.Contract/DistributedSystem.Contract.csproj", "src/DistributedSystem.Contract/"]
COPY ["src/Authorization/Authorization.Application/Authorization.Application.csproj", "src/Authorization/Authorization.Application/"]
COPY ["src/Authorization/Authorization.Domain/Authorization.Domain.csproj", "src/Authorization/Authorization.Domain/"]
COPY ["src/Authorization/Authorization.Persistence/Authorization.Persistence.csproj", "src/Authorization/Authorization.Persistence/"]
COPY ["src/Authorization/Authorization.Infrastructure/Authorization.Infrastructure.csproj", "src/Authorization/Authorization.Infrastructure/"]
COPY ["src/Authorization/Authorization.Presentation/Authorization.Presentation.csproj", "src/Authorization/Authorization.Presentation/"]

RUN dotnet restore "./src/Authorization/Authorization.API/./Authorization.API.csproj"
COPY . .

WORKDIR "/src/src/Authorization/Authorization.API"
RUN dotnet build "./Authorization.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./Authorization.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorization.API.dll"]