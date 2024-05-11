## Development
docker compose -f docker-compose.Development.Infrastructure.yaml up --detach

## Staging
docker compose -f docker-compose.Staging.Infrastructure.yaml up --detach
docker compose -f docker-compose.Staging.Services.yaml up --detach