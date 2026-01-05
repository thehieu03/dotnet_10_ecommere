# Cấu Trúc Dự Án Ecommerce Microservices (.NET 10)

## 📋 Tổng Quan

Đây là một dự án **E-commerce Microservices** được xây dựng bằng **.NET 10**, sử dụng kiến trúc **Clean Architecture** và **Event-Driven Architecture**.

---

## 🏗️ Kiến Trúc Tổng Thể

```
┌─────────────────────────────────────────────────────────────┐
│                    ECOMMERCE SOLUTION                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Catalog API  │  │  Basket API   │  │ Discount API │      │
│  │  (MongoDB)   │  │   (Redis)    │  │ (PostgreSQL) │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                 │              │
│         └─────────────────┼─────────────────┘              │
│                           │                                │
│                    ┌──────▼───────┐                        │
│                    │  RabbitMQ    │                        │
│                    │  (Event Bus) │                        │
│                    └──────┬───────┘                        │
│                           │                                │
│                    ┌──────▼───────┐                        │
│                    │ Ordering API  │                        │
│                    │ (SQL Server) │                        │
│                    └──────────────┘                        │
│                                                              │
│  ┌──────────────────────────────────────────────┐          │
│  │     Infrastructure (Shared Libraries)         │          │
│  │  - EventBus.Messages                          │          │
│  │  - Common.Logging                            │          │
│  └──────────────────────────────────────────────┘          │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 Cấu Trúc Thư Mục

```
Ecommerce/
├── Infrastructure/              # Shared libraries
│   ├── Common.Logging/          # Serilog logging configuration
│   └── EventBus.Messages/       # Integration events (shared)
│       ├── Common/
│       │   └── EventBusConstant.cs
│       └── Events/
│           ├── BaseIntegrationEvent.cs
│           ├── BasketCheckoutEvent.cs
│           └── BasketCheckoutEventV2.cs
│
├── Services/                    # Microservices
│   ├── Basket/                  # Shopping Cart Service
│   ├── Catalog/                 # Product Catalog Service
│   ├── Discount/                # Discount/Coupon Service
│   └── Ordering/                # Order Management Service
│
├── docker-compose.yml           # Docker orchestration
├── docker-compose.override.yml  # Local development overrides
└── README.md                    # Project documentation
```

---

## 🔧 Các Microservices

### 1. **Basket Service** (Shopping Cart)

**Mục đích:** Quản lý giỏ hàng của khách hàng

**Cấu trúc:**

```
Basket/
├── Basket.API/                  # REST API Layer
│   ├── Controller/
│   │   ├── BasketController.cs  # v1 API
│   │   └── V2/
│   │       └── BasketController.cs  # v2 API
│   ├── Program.cs
│   └── Dockerfile
│
├── Basket.Application/          # Application Layer (CQRS)
│   ├── Commands/
│   │   ├── CreateShoppingCartCommand.cs
│   │   └── DeleteBasketByUserNameCommand.cs
│   ├── Queries/
│   │   └── GetBasketByUserNameQuery.cs
│   ├── Handlers/
│   │   ├── CreateShoppingCartHandler.cs
│   │   ├── DeleteBasketByUserNameHandler.cs
│   │   └── GetBasketByUserNameHandler.cs
│   ├── Responses/
│   │   ├── ShoppingCartResponse.cs
│   │   └── ShoppingCartItemResponse.cs
│   ├── Mappers/
│   │   └── BasketMappingProfile.cs
│   └── GrpcService/
│       └── DiscountGrpcService.cs  # gRPC client cho Discount
│
├── Basket.Core/                 # Domain Layer
│   ├── Entities/
│   │   ├── ShoppingCart.cs
│   │   ├── ShoppingCartItem.cs
│   │   ├── BasketCheckout.cs
│   │   └── BasketCheckoutV2.cs
│   └── Repositories/
│       └── IBasketRepository.cs
│
└── Basket.Infrastructure/        # Infrastructure Layer
    └── Repositories/
        └── BaskRepository.cs    # Redis implementation
```

**Database:** Redis (In-memory cache)

- Key: `UserName`
- Value: JSON của `ShoppingCart`

**Port:** `8002`

**Tính năng:**

- ✅ CRUD giỏ hàng
- ✅ API Versioning (v1, v2)
- ✅ gRPC client để gọi Discount Service
- ✅ Publish `BasketCheckoutEvent` lên RabbitMQ khi checkout

---

### 2. **Catalog Service** (Product Catalog)

**Mục đích:** Quản lý sản phẩm, thương hiệu, loại sản phẩm

**Cấu trúc:**

```
Catalog/
├── Catalog.API/                 # REST API Layer
│   ├── Controllers/
│   ├── Program.cs
│   └── Dockerfile
│
├── Catalog.Application/         # Application Layer (CQRS)
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Responses/
│   └── Mapper/
│
├── Catalog.Core/                # Domain Layer
│   ├── Entities/
│   │   ├── Product.cs
│   │   ├── ProductBrand.cs
│   │   ├── ProductType.cs
│   │   └── BaseEntity.cs
│   ├── Repositories/
│   └── Specs/                    # Specification pattern
│
└── Catalog.Infastructure/       # Infrastructure Layer
    └── Data/
        └── CatalogContext.cs    # MongoDB context
```

**Database:** MongoDB

- Database: `CatalogDb`
- Collections: `Products`, `Brands`, `Types`

**Port:** `8000`

**Tính năng:**

- ✅ CRUD sản phẩm
- ✅ Quản lý thương hiệu và loại sản phẩm
- ✅ Specification pattern cho queries phức tạp

---

### 3. **Discount Service** (Coupon/Discount)

**Mục đích:** Quản lý mã giảm giá và khuyến mãi

**Cấu trúc:**

```
Discount/
├── Discount.Api/                # REST API + gRPC Server
│   ├── Services/
│   ├── Program.cs
│   └── Dockerfile
│
├── Discount.Application/         # Application Layer
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Mapper/
│   └── Protos/
│       └── discount.proto        # gRPC service definition
│
├── Discount.Core/                # Domain Layer
│   ├── Entities/
│   │   └── Coupon.cs
│   └── Repositories/
│
└── Discount.Infrastructure/      # Infrastructure Layer
    └── Repositories/
```

**Database:** PostgreSQL

- Database: `DiscountDb`
- Table: `Coupons`

**Port:** `8001` (REST), `8080` (gRPC)

**Tính năng:**

- ✅ CRUD mã giảm giá
- ✅ gRPC server để Basket Service gọi
- ✅ REST API

---

### 4. **Ordering Service** (Order Management)

**Mục đích:** Quản lý đơn hàng

**Cấu trúc:**

```
Ordering/
├── Ordering.API/                # REST API Layer
│   ├── Controllers/
│   │   └── OrderController.cs
│   ├── EventBusConsumer/
│   │   ├── BasketOrderingConsumer.cs      # v1 consumer
│   │   └── BasketOrderingConsumerV2.cs    # v2 consumer
│   ├── Extensions/
│   │   └── DbExtension.cs      # Database migration
│   ├── Program.cs
│   └── Dockerfile
│
├── Ordering.Application/         # Application Layer (CQRS)
│   ├── Commands/
│   │   ├── CheckoutOrderCommand.cs
│   │   ├── CheckoutOrderCommandV2.cs
│   │   ├── UpdateOrderCommand.cs
│   │   └── DeleteOrderCommand.cs
│   ├── Queries/
│   │   └── GetOrderListQuery.cs
│   ├── Handlers/
│   │   ├── CheckoutOrderCommandHandler.cs
│   │   ├── UpdateOrderCommandHandler.cs
│   │   ├── DeleteOrderCommandHandler.cs
│   │   └── GetOrderListQueryHandler.cs
│   ├── Validators/
│   │   ├── CheckoutOrderCommandValidator.cs
│   │   └── CheckoutOrderCommandValidatorV2.cs
│   ├── Responses/
│   │   └── OrderResponse.cs
│   ├── Mappers/
│   │   └── OrderMappingProfile.cs
│   ├── Behaviour/
│   │   ├── ValidationBehaviour.cs        # MediatR pipeline
│   │   └── UnhandledExceptionBehaviour.cs
│   └── Extensions/
│       └── ServiceRegistration.cs
│
├── Ordering.Core/               # Domain Layer
│   ├── Entities/
│   │   └── Order.cs
│   ├── Repositories/
│   │   ├── IOrderRepository.cs
│   │   └── IRepositoryBase.cs
│   └── Common/
│
└── Ordering.Infrastructure/      # Infrastructure Layer
    ├── Data/
    │   ├── OrderContext.cs      # EF Core DbContext
    │   ├── OrderContextSeed.cs
    │   └── OrderContextFactory.cs
    ├── Repositories/
    │   ├── OrderRepository.cs
    │   └── RepositoryBase.cs
    └── Migrations/               # EF Core migrations
```

**Database:** SQL Server

- Database: `OrderDb`
- Table: `Orders`

**Port:** `8003`

**Tính năng:**

- ✅ CRUD đơn hàng
- ✅ Event consumer từ RabbitMQ
- ✅ EF Core với migrations tự động
- ✅ Validation pipeline (FluentValidation)
- ✅ Exception handling pipeline

---

## 🔄 Luồng Giao Tiếp Giữa Các Services

### 1. **Basket → Discount (gRPC)**

```
Basket Service → gRPC Client → Discount Service (gRPC Server)
```

- Mục đích: Lấy thông tin discount khi tính giá giỏ hàng
- Protocol: gRPC
- Synchronous

### 2. **Basket → Ordering (Event-Driven)**

```
Basket Service → Publish Event → RabbitMQ → Ordering Service (Consumer)
```

- Mục đích: Tạo đơn hàng khi user checkout
- Protocol: RabbitMQ (MassTransit)
- Asynchronous
- Event: `BasketCheckoutEvent` / `BasketCheckoutEventV2`

---

## 🗄️ Databases & Storage

| Service      | Database   | Type      | Purpose                         |
| ------------ | ---------- | --------- | ------------------------------- |
| **Catalog**  | MongoDB    | NoSQL     | Lưu sản phẩm, thương hiệu, loại |
| **Basket**   | Redis      | In-memory | Lưu giỏ hàng tạm thời           |
| **Discount** | PostgreSQL | SQL       | Lưu mã giảm giá                 |
| **Ordering** | SQL Server | SQL       | Lưu đơn hàng                    |

---

## 🛠️ Technology Stack

### **Core Technologies:**

- **.NET 10** - Framework
- **ASP.NET Core** - Web framework
- **C#** - Programming language

### **Architecture Patterns:**

- **Clean Architecture** - Separation of concerns
- **CQRS** - Command Query Responsibility Segregation (MediatR)
- **Repository Pattern** - Data access abstraction
- **Event-Driven Architecture** - Asynchronous communication

### **Libraries & Frameworks:**

- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **MassTransit** - Message bus abstraction
- **RabbitMQ** - Message broker
- **Entity Framework Core** - ORM (Ordering service)
- **gRPC** - Inter-service communication
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **API Versioning** - Version management

### **Infrastructure:**

- **Docker** - Containerization
- **Docker Compose** - Orchestration
- **Redis** - Caching
- **MongoDB** - NoSQL database
- **PostgreSQL** - SQL database
- **SQL Server** - SQL database
- **Elasticsearch** - Search engine (optional)
- **Kibana** - Log visualization (optional)

---

## 📦 Infrastructure Projects

### **1. EventBus.Messages**

**Mục đích:** Shared library chứa integration events

**Nội dung:**

- `BaseIntegrationEvent` - Base class cho tất cả events
- `BasketCheckoutEvent` - Event khi checkout basket (v1)
- `BasketCheckoutEventV2` - Event khi checkout basket (v2)
- `EventBusConstant` - Queue names và constants

**Sử dụng bởi:**

- Basket Service (publish events)
- Ordering Service (consume events)

### **2. Common.Logging**

**Mục đích:** Centralized logging configuration (Serilog)

**Nội dung:**

- `Logging.cs` - Serilog configuration

**Sử dụng bởi:**

- Tất cả services

---

## 🐳 Docker Configuration

### **Services:**

- `catalog_api` - Catalog Service
- `basket_api` - Basket Service
- `discount_api` - Discount Service
- `ordering_api` - Ordering Service

### **Databases:**

- `catalog.db` - MongoDB
- `basket.db` - Redis
- `discount.db` - PostgreSQL
- `order_db` - SQL Server

### **Message Broker:**

- `rabbit_mq` - RabbitMQ với Management UI

### **Utilities:**

- `Portainer` - Docker management UI
- `pg_admin` - PostgreSQL admin UI
- `elastic_search` - Search engine
- `kibana` - Log visualization

---

## 🔌 API Endpoints

### **Catalog API** (Port 8000)

- `GET /api/v1/Catalog/GetAllProducts`
- `GET /api/v1/Catalog/GetProduct/{id}`
- `POST /api/v1/Catalog/CreateProduct`
- `PUT /api/v1/Catalog/UpdateProduct`
- `DELETE /api/v1/Catalog/DeleteProduct/{id}`

### **Basket API** (Port 8002)

- `GET /Basket/GetBasket/{userName}` (v1)
- `POST /Basket/CreateBasket` (v1)
- `DELETE /Basket/DeleteBasket/{userName}` (v1)
- `POST /Basket/Checkout` (v1)
- `GET /api/v2/Basket/GetBasket/{userName}` (v2)
- `POST /api/v2/Basket/Checkout` (v2)

### **Discount API** (Port 8001)

- `GET /api/Discount/GetAllCoupons`
- `GET /api/Discount/GetCoupon/{couponCode}`
- `POST /api/Discount/CreateCoupon`
- `PUT /api/Discount/UpdateCoupon`
- `DELETE /api/Discount/DeleteCoupon/{id}`
- gRPC: `GetDiscount` method

### **Ordering API** (Port 8003)

- `GET /api/v1/Order`
- `GET /api/v1/Order/{userName}`
- `POST /api/v1/Order/CheckoutOrder`
- `PUT /api/v1/Order/UpdateOrder`
- `DELETE /api/v1/Order/{id}`

---

## 🚀 Development Workflow

### **1. Local Development:**

```bash
# Start all services với Docker Compose
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f [service_name]
```

### **2. Build & Run:**

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run specific service
cd Services/Basket/Basket.API
dotnet run
```

### **3. Database Migrations (Ordering):**

```bash
cd Services/Ordering/Ordering.Infrastructure
dotnet ef migrations add [MigrationName]
dotnet ef database update
```

---

## 📊 Data Flow Example: Checkout Process

```
1. User → POST /Basket/Checkout
   ↓
2. Basket Service:
   - Validate basket exists
   - Get discount via gRPC (Discount Service)
   - Calculate total price
   - Publish BasketCheckoutEvent → RabbitMQ
   - Delete basket from Redis
   - Return 202 Accepted
   ↓
3. RabbitMQ Queue: "basketchecount-queue"
   ↓
4. Ordering Service (BasketOrderingConsumer):
   - Receive event
   - Map event → CheckoutOrderCommand
   - Validate command (FluentValidation)
   - Create order in SQL Server
   - Log success
```

---

## 🎯 Design Patterns Sử Dụng

1. **Clean Architecture** - Layered architecture
2. **CQRS** - Separate commands and queries
3. **Repository Pattern** - Data access abstraction
4. **Specification Pattern** - Complex queries (Catalog)
5. **Event-Driven Architecture** - Loose coupling
6. **API Gateway Pattern** - (Có thể thêm sau)
7. **Circuit Breaker Pattern** - (Có thể thêm sau)

---

## 📝 Notes

- **API Versioning:** Basket và Ordering services hỗ trợ versioning
- **Logging:** Tất cả services sử dụng Serilog với centralized config
- **Validation:** Ordering service sử dụng FluentValidation với MediatR pipeline
- **Error Handling:** UnhandledExceptionBehaviour tự động log exceptions
- **Database Migrations:** Ordering service tự động apply migrations khi start

---

## 🔐 Security Considerations

- Connection strings trong `appsettings.json` (không commit secrets)
- RabbitMQ credentials trong environment variables
- Database passwords trong Docker environment variables
- API keys và secrets nên sử dụng Azure Key Vault hoặc similar

---

## 📈 Scalability

- Mỗi service có thể scale độc lập
- Stateless services (trừ Ordering với database)
- Redis cho caching và session management
- RabbitMQ cho async processing
- Docker containers cho easy deployment

---

## 🧪 Testing (Future)

- Unit tests cho handlers và validators
- Integration tests cho API endpoints
- E2E tests cho checkout flow
- Load testing với k6 hoặc JMeter

---

## 📚 Documentation

- **README.md** - Project overview
- **Swagger UI** - API documentation (available at `/swagger`)
- **Code comments** - Inline documentation

---

## 🎓 Learning Resources

- Clean Architecture principles
- CQRS pattern với MediatR
- Event-Driven Architecture
- Microservices best practices
- .NET 10 features

---

**Last Updated:** 2024
**Version:** 1.0
**Maintainer:** Development Team
