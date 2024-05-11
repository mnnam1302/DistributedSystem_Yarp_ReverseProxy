#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

COPY ["src/DistributedSystem.API/DistributedSystem.API.csproj", "src/DistributedSystem.API/"]
COPY ["src/DistributedSystem.Application/DistributedSystem.Application.csproj", "src/DistributedSystem.Application/"]
COPY ["src/DistributedSystem.Contract/DistributedSystem.Contract.csproj", "src/DistributedSystem.Contract/"]
COPY ["src/DistributedSystem.Domain/DistributedSystem.Domain.csproj", "src/DistributedSystem.Domain/"]
COPY ["src/DistributedSystem.Persistence/DistributedSystem.Persistence.csproj", "src/DistributedSystem.Persistence/"]
COPY ["src/DistributedSystem.Infrastructure/DistributedSystem.Infrastructure.csproj", "src/DistributedSystem.Infrastructure/"]
COPY ["src/DistributedSystem.Presentation/DistributedSystem.Presentation.csproj", "src/DistributedSystem.Presentation/"]

RUN dotnet restore "./src/DistributedSystem.API/./DistributedSystem.API.csproj"

COPY . .

WORKDIR "/src/src/DistributedSystem.API"
RUN dotnet build "./DistributedSystem.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./DistributedSystem.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedSystem.API.dll"]