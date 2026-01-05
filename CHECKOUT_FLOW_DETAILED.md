# Luồng Xử Lý Checkout - Chi Tiết Các Class Được Gọi

## 📋 Tổng Quan

Tài liệu này mô tả chi tiết luồng xử lý khi user checkout đơn hàng, từ khi gửi request đến khi order được tạo trong database, bao gồm tất cả các class được gọi.

---

## 🔄 Luồng Tổng Quan

```
User Request
    ↓
Basket Service (REST API)
    ↓
RabbitMQ (Event Bus)
    ↓
Ordering Service (Event Consumer)
    ↓
Database (SQL Server)
```

---

## 📝 Chi Tiết Từng Bước

### **BƯỚC 1: User Gửi Request Checkout**

**Endpoint:** `POST /Basket/Checkout`

**Request Body:**
```json
{
  "userName": "john_doe",
  "firstName": "John",
  "lastName": "Doe",
  "emailAddress": "john@example.com",
  "addressLine": "123 Main St",
  "country": "USA",
  "state": "CA",
  "zipCode": "12345",
  "cardName": "John Doe",
  "cardNumber": "1234567890",
  "expiration": "12/25",
  "cvv": "123",
  "paymentMethod": 1
}
```

---

### **BƯỚC 2: Basket Service - Controller Layer**

#### **Class: `BasketController`**
📍 **File:** `Services/Basket/Basket.API/Controller/BasketController.cs`

**Method:** `Checkout([FromBody] BasketCheckout basketCheckout)`

**Dependencies:**
- `IMediator _mediator` - MediatR để gửi commands/queries
- `IPublishEndpoint _publishEndpoint` - MassTransit để publish events
- `ILogger<BasketController> _logger` - Logging

**Luồng xử lý trong method:**

1. **Tạo Query để lấy basket:**
   ```csharp
   var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
   ```

2. **Gửi query qua MediatR:**
   ```csharp
   var basket = await _mediator.Send(query);
   ```
   → Gọi đến: `GetBasketByUserNameHandler`

3. **Validate basket tồn tại:**
   ```csharp
   if (basket == null) return BadRequest();
   ```

4. **Map BasketCheckout → BasketCheckoutEvent:**
   ```csharp
   var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
   eventMsg.TotalPrice = basket.TotalPrice;
   ```
   → Sử dụng: `BasketMappingProfile` (AutoMapper)

5. **Publish event lên RabbitMQ:**
   ```csharp
   await _publishEndpoint.Publish(eventMsg);
   ```
   → MassTransit gửi event vào queue: `"basketchecount-queue"`

6. **Xóa basket sau khi publish:**
   ```csharp
   var deleteCmd = new DeleteBasketByUserNameCommand(basket.UserName);
   await _mediator.Send(deleteCmd);
   ```
   → Gọi đến: `DeleteBasketByUserNameHandler`

7. **Return 202 Accepted:**
   ```csharp
   return Accepted();
   ```

---

### **BƯỚC 3: Basket Service - Application Layer (Get Basket)**

#### **Class: `GetBasketByUserNameHandler`**
📍 **File:** `Services/Basket/Basket.Application/Handlers/GetBasketByUserNameHandler.cs`

**Implements:** `IRequestHandler<GetBasketByUserNameQuery, ShoppingCartResponse>`

**Dependencies:**
- `IBasketRepository _basketRepository` - Repository để truy cập Redis

**Method:** `Handle(GetBasketByUserNameQuery request, CancellationToken cancellationToken)`

**Luồng xử lý:**
1. Gọi repository để lấy basket từ Redis:
   ```csharp
   var shoppingCart = await _basketRepository.GetBasketAsync(request.Username);
   ```
   → Gọi đến: `BaskRepository.GetBasketAsync()`

2. Map ShoppingCart → ShoppingCartResponse:
   ```csharp
   var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCartResponse>(shoppingCart);
   ```
   → Sử dụng: `BasketMappingProfile` (AutoMapper)

3. Return response

---

### **BƯỚC 4: Basket Service - Infrastructure Layer (Get Basket)**

#### **Class: `BaskRepository`**
📍 **File:** `Services/Basket/Basket.Infrastructure/Repositories/BaskRepository.cs`

**Implements:** `IBasketRepository`

**Dependencies:**
- `IDistributedCache distributedCache` - Redis cache

**Method:** `GetBasketAsync(string userName)`

**Luồng xử lý:**
1. Lấy JSON string từ Redis:
   ```csharp
   var basket = await distributedCache.GetStringAsync(userName);
   ```

2. Kiểm tra null:
   ```csharp
   if(string.IsNullOrEmpty(basket)) return null;
   ```

3. Deserialize JSON → ShoppingCart:
   ```csharp
   return JsonConvert.DeserializeObject<ShoppingCart>(basket);
   ```

**Database:** Redis
- Key: `userName` (ví dụ: "john_doe")
- Value: JSON string của `ShoppingCart`

---

### **BƯỚC 5: Basket Service - Mapping (BasketCheckout → Event)**

#### **Class: `BasketMappingProfile`**
📍 **File:** `Services/Basket/Basket.Application/Mappers/BasketMappingProfile.cs`

**Extends:** `Profile` (AutoMapper)

**Mapping Configuration:**
```csharp
CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
```

**Mapping được thực hiện:**
- `BasketCheckout` → `BasketCheckoutEvent`
- Các properties được map tự động:
  - `UserName`, `TotalPrice`, `FirstName`, `LastName`, `EmailAddress`
  - `AddressLine`, `Country`, `State`, `ZipCode`
  - `CardName`, `CardNumber`, `Expiration`, `Cvv`, `PaymentMethod`
- `CorelationId` và `CreationDate` được tạo tự động trong `BaseIntegrationEvent`

---

### **BƯỚC 6: Basket Service - Publish Event**

#### **Class: `IPublishEndpoint` (MassTransit)**
📍 **Interface:** MassTransit library

**Method:** `Publish<T>(T message)`

**Luồng xử lý:**
1. MassTransit serialize `BasketCheckoutEvent` thành JSON
2. Gửi message vào RabbitMQ queue: `"basketchecount-queue"`
3. Event được lưu trong queue chờ consumer xử lý

**Queue:** `EventBusConstant.BasketCheckoutQueue` = `"basketchecount-queue"`

---

### **BƯỚC 7: Basket Service - Delete Basket**

#### **Class: `DeleteBasketByUserNameHandler`**
📍 **File:** `Services/Basket/Basket.Application/Handlers/DeleteBasketByUserNameHandler.cs`

**Implements:** `IRequestHandler<DeleteBasketByUserNameCommand, Unit>`

**Dependencies:**
- `IBasketRepository _basketRepository`

**Method:** `Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)`

**Luồng xử lý:**
1. Gọi repository để xóa basket:
   ```csharp
   await _basketRepository.DeleteBasketAsync(request.UserName);
   ```
   → Gọi đến: `BaskRepository.DeleteBasketAsync()`

2. Return `Unit.Value`

#### **Class: `BaskRepository` (Delete Method)**
📍 **File:** `Services/Basket/Basket.Infrastructure/Repositories/BaskRepository.cs`

**Method:** `DeleteBasketAsync(string userName)`

**Luồng xử lý:**
```csharp
await distributedCache.RemoveAsync(userName);
```
- Xóa key `userName` khỏi Redis

---

### **BƯỚC 8: RabbitMQ - Event Bus**

**Queue:** `"basketchecount-queue"`

**Event:** `BasketCheckoutEvent`

**Properties:**
- `CorelationId` (Guid) - Để trace request
- `CreationDate` (DateTime) - Thời gian tạo event
- `UserName`, `TotalPrice`, `FirstName`, `LastName`, `EmailAddress`
- `AddressLine`, `Country`, `State`, `ZipCode`
- `CardName`, `CardNumber`, `Expiration`, `Cvv`, `PaymentMethod`

---

### **BƯỚC 9: Ordering Service - MassTransit Consumer**

#### **Class: `BasketOrderingConsumer`**
📍 **File:** `Services/Ordering/Ordering.API/EventBusConsumer/BasketOrderingConsumer.cs`

**Implements:** `IConsumer<BasketCheckoutEvent>` (MassTransit)

**Dependencies:**
- `IMediator mediator` - MediatR để gửi commands
- `IMapper mapper` - AutoMapper để map objects
- `ILogger<BasketOrderingConsumer> logger` - Logging

**Method:** `Consume(ConsumeContext<BasketCheckoutEvent> context)`

**Luồng xử lý:**

1. **Tạo logging scope với CorrelationId:**
   ```csharp
   using var scope = logger.BeginScope(
       "Consuming Basket Checkout Event for {correlationId}",
       context.Message.CorelationId
   );
   ```

2. **Map Event → Command:**
   ```csharp
   var cmd = mapper.Map<CheckoutOrderCommand>(context.Message);
   ```
   → Sử dụng: `OrderMappingProfile` (AutoMapper)

3. **Gửi command qua MediatR:**
   ```csharp
   var result = await mediator.Send(cmd);
   ```
   → Gọi đến: MediatR Pipeline → `CheckoutOrderCommandHandler`

4. **Log completion:**
   ```csharp
   logger.LogInformation("Basket Checkout Event completed!!!");
   ```

---

### **BƯỚC 10: Ordering Service - MediatR Pipeline**

#### **Class: `UnhandledExceptionBehaviour`** (Outer Pipeline)
📍 **File:** `Services/Ordering/Ordering.Application/Behaviour/UnhandledExceptionBehaviour.cs`

**Implements:** `IPipelineBehavior<TRequest, TResponse>`

**Method:** `Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)`

**Luồng xử lý:**
```csharp
try
{
    return await next(cancellationToken); // Gọi behavior/handler tiếp theo
}
catch (Exception e)
{
    var requestName = typeof(TRequest).Name;
    logger.LogError(e, $"Unhandled exception occurred with Request Name: {requestName}, {request}");
    throw; // Re-throw exception
}
```

**Mục đích:** Bắt và log mọi exception chưa được xử lý

---

#### **Class: `ValidationBehaviour`** (Inner Pipeline)
📍 **File:** `Services/Ordering/Ordering.Application/Behaviour/ValidationBehaviour.cs`

**Implements:** `IPipelineBehavior<TRequest, TResponse>`

**Dependencies:**
- `IEnumerable<IValidator<TRequest>> validators` - FluentValidation validators

**Method:** `Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)`

**Luồng xử lý:**

1. **Kiểm tra có validators không:**
   ```csharp
   if (validators.Any())
   ```

2. **Tạo validation context:**
   ```csharp
   var context = new ValidationContext<TRequest>(request);
   ```

3. **Chạy tất cả validators:**
   ```csharp
   var validationResults = await Task.WhenAll(
       validators.Select(v => v.ValidateAsync(context, cancellationToken))
   );
   ```
   → Gọi đến: `CheckoutOrderCommandValidator`

4. **Thu thập lỗi:**
   ```csharp
   var failures = validationResults
       .SelectMany(e => e.Errors)
       .Where(f => f != null)
       .ToList();
   ```

5. **Nếu có lỗi → throw exception:**
   ```csharp
   if (failures.Count != 0)
   {
       throw new ValidationException(failures);
   }
   ```

6. **Nếu pass → tiếp tục:**
   ```csharp
   return await next(cancellationToken); // Gọi handler
   ```

---

#### **Class: `CheckoutOrderCommandValidator`**
📍 **File:** `Services/Ordering/Ordering.Application/Validators/CheckoutOrderCommandValidator.cs`

**Extends:** `AbstractValidator<CheckoutOrderCommand>` (FluentValidation)

**Validation Rules:**

1. **UserName:**
   ```csharp
   RuleFor(o => o.UserName)
       .NotEmpty()
       .NotNull()
       .MaximumLength(70)
   ```

2. **TotalPrice:**
   ```csharp
   RuleFor(o => o.TotalPrice)
       .NotEmpty()
       .GreaterThan(-1)
   ```

3. **EmailAddress:**
   ```csharp
   RuleFor(o => o.EmailAddress)
       .NotEmpty()
   ```

4. **FirstName:**
   ```csharp
   RuleFor(o => o.FirstName)
       .NotEmpty()
       .NotNull()
   ```

5. **LastName:**
   ```csharp
   RuleFor(o => o.LastName)
       .NotEmpty()
       .NotNull()
   ```

**Nếu validation fail:** Throw `ValidationException` với danh sách lỗi

---

### **BƯỚC 11: Ordering Service - Command Handler**

#### **Class: `CheckoutOrderCommandHandler`**
📍 **File:** `Services/Ordering/Ordering.Application/Handlers/CheckoutOrderCommandHandler.cs`

**Implements:** `IRequestHandler<CheckoutOrderCommand, int>`

**Dependencies:**
- `IOrderRepository orderRepository` - Repository để lưu order
- `IMapper mapper` - AutoMapper để map objects
- `ILogger<CheckoutOrderCommandHandler> logger` - Logging

**Method:** `Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)`

**Luồng xử lý:**

1. **Map Command → Order Entity:**
   ```csharp
   var orderEntity = _mapper.Map<Order>(request);
   ```
   → Sử dụng: `OrderMappingProfile` (AutoMapper)

2. **Lưu order vào database:**
   ```csharp
   var generatedOrder = await _orderRepository.AddAsync(orderEntity);
   ```
   → Gọi đến: `OrderRepository.AddAsync()`

3. **Log success:**
   ```csharp
   _logger.LogInformation("Order with Id {OrderId} successfully created", generatedOrder.Id);
   ```

4. **Return OrderId:**
   ```csharp
   return generatedOrder.Id;
   ```

---

### **BƯỚC 12: Ordering Service - Mapping (Command → Entity)**

#### **Class: `OrderMappingProfile`**
📍 **File:** `Services/Ordering/Ordering.Application/Mappers/OrderMappingProfile.cs`

**Extends:** `Profile` (AutoMapper)

**Mapping Configuration:**
```csharp
CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
```

**Mapping được thực hiện:**
- `CheckoutOrderCommand` → `Order` (Entity)
- Các properties được map tự động:
  - `UserName`, `TotalPrice`, `FirstName`, `LastName`, `EmailAddress`
  - `AddressLine`, `Country`, `State`, `ZipCode`
  - `CardName`, `CardNumber`, `Expiration`, `Cvv`, `PaymentMethod`
- `Id` được tạo tự động bởi database

---

### **BƯỚC 13: Ordering Service - Repository Layer**

#### **Class: `OrderRepository`**
📍 **File:** `Services/Ordering/Ordering.Infrastructure/Repositories/OrderRepository.cs`

**Extends:** `RepositoryBase<Order>`

**Implements:** `IOrderRepository`

**Dependencies:**
- `OrderContext dbContext` - EF Core DbContext

**Method:** `AddAsync(Order entity)` (inherited from RepositoryBase)

---

#### **Class: `RepositoryBase<T>`**
📍 **File:** `Services/Ordering/Ordering.Infrastructure/Repositories/RepositoryBase.cs`

**Method:** `AddAsync(T entity)`

**Luồng xử lý:**

1. **Add entity vào DbContext:**
   ```csharp
   dbContext.Set<T>().Add(entity);
   ```

2. **Save changes vào database:**
   ```csharp
   await dbContext.SaveChangesAsync();
   ```
   → EF Core thực hiện:
   - Generate SQL INSERT statement
   - Execute SQL trên SQL Server
   - Get generated ID từ database
   - Update entity với ID mới

3. **Return entity với ID:**
   ```csharp
   return entity;
   ```

**Database:** SQL Server
- Database: `OrderDb`
- Table: `Orders`
- ID được generate tự động (Identity column)

---

## 📊 Sơ Đồ Luồng Hoàn Chỉnh

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. USER REQUEST                                                 │
│    POST /Basket/Checkout                                        │
│    Body: BasketCheckout { UserName, Address, Payment, ... }    │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 2. BASKET SERVICE - CONTROLLER                                 │
│    BasketController.Checkout()                                 │
│    ├─ Create GetBasketByUserNameQuery                          │
│    └─ Send via MediatR                                         │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 3. BASKET SERVICE - APPLICATION LAYER                          │
│    GetBasketByUserNameHandler.Handle()                         │
│    ├─ Call IBasketRepository.GetBasketAsync()                   │
│    └─ Map ShoppingCart → ShoppingCartResponse                  │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 4. BASKET SERVICE - INFRASTRUCTURE LAYER                        │
│    BaskRepository.GetBasketAsync()                              │
│    ├─ Get JSON from Redis (key: userName)                      │
│    └─ Deserialize → ShoppingCart                               │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 5. BASKET SERVICE - CONTROLLER (Continue)                      │
│    BasketController.Checkout()                                  │
│    ├─ Validate basket != null                                  │
│    ├─ Map BasketCheckout → BasketCheckoutEvent                │
│    │   (BasketMappingProfile)                                  │
│    ├─ Set TotalPrice from basket                               │
│    ├─ Publish event → RabbitMQ                                │
│    ├─ Delete basket (DeleteBasketByUserNameHandler)            │
│    └─ Return 202 Accepted                                       │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 6. RABBITMQ                                                     │
│    Queue: "basketchecount-queue"                               │
│    Event: BasketCheckoutEvent {                                 │
│      CorelationId, CreationDate,                               │
│      UserName, TotalPrice, Address, Payment, ...                │
│    }                                                             │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 7. ORDERING SERVICE - MASSTRANSIT                              │
│    MassTransit Hosted Service                                  │
│    ├─ Listen to queue "basketchecount-queue"                    │
│    ├─ Receive event                                            │
│    └─ Create BasketOrderingConsumer instance                   │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 8. ORDERING SERVICE - EVENT CONSUMER                            │
│    BasketOrderingConsumer.Consume()                              │
│    ├─ Create logging scope (CorrelationId)                      │
│    ├─ Map BasketCheckoutEvent → CheckoutOrderCommand           │
│    │   (OrderMappingProfile)                                   │
│    └─ Send command via MediatR                                 │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 9. ORDERING SERVICE - MEDIATR PIPELINE                         │
│    UnhandledExceptionBehaviour (Outer)                          │
│    ├─ try {                                                     │
│    │     ValidationBehaviour (Inner)                            │
│    │     ├─ Get CheckoutOrderCommandValidator                  │
│    │     ├─ Run validation rules                               │
│    │     │   (CheckoutOrderCommandValidator)                   │
│    │     ├─ If fail → throw ValidationException                │
│    │     └─ If pass → next()                                    │
│    │         │                                                  │
│    │         ▼                                                  │
│    │     CheckoutOrderCommandHandler                            │
│    │     ├─ Map CheckoutOrderCommand → Order Entity            │
│    │     │   (OrderMappingProfile)                              │
│    │     ├─ Call IOrderRepository.AddAsync()                   │
│    │     └─ Return OrderId                                      │
│    │                                                            │
│    └─ } catch (Exception e) {                                  │
│          Log error                                               │
│          throw;                                                 │
│        }                                                         │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 10. ORDERING SERVICE - REPOSITORY LAYER                         │
│     OrderRepository.AddAsync()                                  │
│     ├─ dbContext.Set<Order>().Add(orderEntity)                  │
│     └─ dbContext.SaveChangesAsync()                             │
│         ├─ EF Core generates SQL INSERT                         │
│         ├─ Execute on SQL Server                                │
│         ├─ Get generated ID                                     │
│         └─ Update entity with ID                                │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 11. DATABASE (SQL SERVER)                                       │
│     Database: OrderDb                                           │
│     Table: Orders                                               │
│     INSERT INTO Orders (...) VALUES (...)                       │
│     Return: OrderId (int)                                       │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📋 Danh Sách Tất Cả Các Class Được Gọi

### **Basket Service:**

1. ✅ `BasketController` - Entry point
2. ✅ `GetBasketByUserNameQuery` - Query object
3. ✅ `GetBasketByUserNameHandler` - Query handler
4. ✅ `BaskRepository` - Redis repository
5. ✅ `BasketMappingProfile` - AutoMapper profile
6. ✅ `BasketCheckoutEvent` - Event object
7. ✅ `IPublishEndpoint` - MassTransit interface
8. ✅ `DeleteBasketByUserNameCommand` - Command object
9. ✅ `DeleteBasketByUserNameHandler` - Command handler

### **RabbitMQ:**

10. ✅ RabbitMQ Queue: `"basketchecount-queue"`

### **Ordering Service:**

11. ✅ `BasketOrderingConsumer` - Event consumer
12. ✅ `OrderMappingProfile` - AutoMapper profile
13. ✅ `CheckoutOrderCommand` - Command object
14. ✅ `UnhandledExceptionBehaviour` - Exception handling pipeline
15. ✅ `ValidationBehaviour` - Validation pipeline
16. ✅ `CheckoutOrderCommandValidator` - FluentValidation validator
17. ✅ `CheckoutOrderCommandHandler` - Command handler
18. ✅ `Order` - Entity object
19. ✅ `OrderRepository` - Repository
20. ✅ `RepositoryBase<Order>` - Base repository
21. ✅ `OrderContext` - EF Core DbContext

---

## ⏱️ Thứ Tự Thực Thi

1. **BasketController.Checkout()** - Nhận request
2. **GetBasketByUserNameHandler** - Lấy basket từ Redis
3. **BaskRepository.GetBasketAsync()** - Truy cập Redis
4. **BasketMappingProfile** - Map BasketCheckout → Event
5. **IPublishEndpoint.Publish()** - Gửi event lên RabbitMQ
6. **DeleteBasketByUserNameHandler** - Xóa basket
7. **BaskRepository.DeleteBasketAsync()** - Xóa từ Redis
8. **BasketOrderingConsumer.Consume()** - Nhận event từ RabbitMQ
9. **OrderMappingProfile** - Map Event → Command
10. **UnhandledExceptionBehaviour** - Bọc pipeline
11. **ValidationBehaviour** - Validate command
12. **CheckoutOrderCommandValidator** - Chạy validation rules
13. **CheckoutOrderCommandHandler** - Xử lý command
14. **OrderMappingProfile** - Map Command → Entity
15. **OrderRepository.AddAsync()** - Lưu vào database
16. **RepositoryBase.AddAsync()** - Base implementation
17. **OrderContext.SaveChangesAsync()** - EF Core save
18. **SQL Server** - Thực thi INSERT

---

## 🎯 Tóm Tắt

**Tổng cộng:** 18+ classes/interfaces được gọi trong quá trình checkout

**Thời gian:** Asynchronous - User nhận response ngay (202 Accepted), order được tạo ở background

**Database Operations:**
- Redis: 2 operations (Get, Delete)
- SQL Server: 1 operation (Insert)

**Message Queue:** 1 event được publish và consume

**Validation:** Tự động validate trước khi tạo order

**Error Handling:** Tự động log mọi exception

---

**Last Updated:** 2024

