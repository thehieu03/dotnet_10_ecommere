# Ecommerce Microservices (.NET 10)

Microservices sample including Catalog, Basket, Discount, and Ordering services with RabbitMQ messaging, Redis cache, MongoDB/Postgres/SQL Server storage, gRPC, MassTransit, and Docker Compose orchestration.

## Architecture
- **Catalog**: REST API, MongoDB for products.
- **Basket**: REST API, Redis for carts; publishes `BasketCheckoutEvent` to RabbitMQ.
- **Discount**: REST API + gRPC server, PostgreSQL for coupons.
- **Ordering**: REST API, SQL Server + EF Core; consumes checkout events.
- **Shared**: `EventBus.Messages` (integration events), RabbitMQ as event bus, Portainer/pgAdmin utilities.

## Tech Stack
- .NET 10, ASP.NET Core, MediatR, AutoMapper, FluentValidation.
- MassTransit + RabbitMQ (messaging).
- Databases: MongoDB (Catalog), Redis (Basket), PostgreSQL (Discount), SQL Server (Ordering).
- EF Core for Ordering; gRPC between Basket and Discount.
- Docker & Docker Compose for local orchestration.

## Prerequisites
- Docker & Docker Compose
- (Optional) .NET 10 SDK for local builds

## Quick Start (Docker)
```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```
Services (dev defaults from docker-compose):
- Catalog API: http://localhost:8000
- Basket API: http://localhost:8002
- Discount API: http://localhost:8001
- Ordering API: http://localhost:8003
- RabbitMQ UI: http://localhost:15672
- pgAdmin: http://localhost:5050

Stop & remove (with volumes):
```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml down -v
```

## Configuration Highlights (placeholders)
- **Basket.API**
  - Redis: `CacheSettings:ConnectionString` (e.g., `basket.db:6379` in Docker)
  - gRPC Discount URL: `GrpcSettings:DiscountUrl` (e.g., `http://discount_api:8080`)
  - RabbitMQ: `EventBusSettings:HostAddress` (e.g., `amqp://<user>:<pass>@rabbit_mq:5672`)
- **Ordering.API**
  - SQL Server: `ConnectionStrings:OrderingConnectionString` (e.g., `Server=order_db,1433;...`)
  - RabbitMQ: `EventBusSettings:HostAddress` (e.g., `amqp://<user>:<pass>@rabbit_mq:5672`)
  - Startup (Docker) runs `dotnet ef database update`.
- **Discount.API**
  - PostgreSQL: `DatabaseSettings:ConnectionString` (e.g., `Server=discount.db;Port=5432;...`)
- **Catalog.API**
  - MongoDB: `DatabaseSettings` (e.g., `mongodb://catalog.db:27017`)

## Messaging Flow
- Basket publishes `BasketCheckoutEvent` to RabbitMQ on checkout.
- Ordering consumes the event, creates orders via EF Core, and persists to SQL Server.

## Development Notes
- appsettings.json use `localhost` for local runs; docker-compose override injects container hostnames.
- Healthchecks defined for Mongo/Postgres; SQL Server healthcheck omitted to avoid tooling issues.
- Ordering migrations are applied at container start (override command).

## Useful Commands (local SDK)
```bash
dotnet restore
dotnet build
dotnet test
```

## Repository Layout
- `Services/Catalog/*`
- `Services/Basket/*`
- `Services/Discount/*`
- `Services/Ordering/*`
- `Infrastructure/EventBus.Messages`
- `docker-compose.yml` / `docker-compose.override.yml`


