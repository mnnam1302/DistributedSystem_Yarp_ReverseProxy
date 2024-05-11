FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

COPY ["src/Query/Query.API/Query.API.csproj", "src/Query/Query.API/"]
COPY ["src/DistributedSystem.Contract/DistributedSystem.Contract.csproj", "src/DistributedSystem.Contract/"]
COPY ["src/Query/Query.Application/Query.Application.csproj", "src/Query/Query.Application/"]
COPY ["src/Query/Query.Domain/Query.Domain.csproj", "src/Query/Query.Domain/"]
COPY ["src/Query/Query.Infrastructure/Query.Infrastructure.csproj", "src/Query/Query.Infrastructure/"]
COPY ["src/Query/Query.Persistence/Query.Persistence.csproj", "src/Query/Query.Persistence/"]
COPY ["src/Query/Query.Presentation/Query.Presentation.csproj", "src/Query/Query.Presentation/"]

RUN dotnet restore "./src/Query/Query.API/./Query.API.csproj"

COPY . .

WORKDIR "/src/src/Query/Query.API"
RUN dotnet build "./Query.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./Query.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Query.API.dll"]