# Giải thích Docker Compose Files

## 📋 Tổng quan

Docker Compose dùng để định nghĩa và chạy nhiều containers cùng lúc. Có 2 file:
- `docker-compose.yml`: File cơ bản, định nghĩa cấu trúc services
- `docker-compose.override.yml`: File override, thêm cấu hình cho development

---

## 📄 docker-compose.yml (File cơ bản)

### Mục đích:
- Định nghĩa **cấu trúc cơ bản** của các services
- Có thể dùng cho **production** hoặc **development**
- Chứa thông tin **tối thiểu** cần thiết

### Phân tích từng phần:

#### 1. Services Section
```yaml
services:
  catalog.api:
    image: ${DOCKER_REGISTRY-}catalogapi
    build:
      context: .
      dockerfile: Services/Catalog/Catalog.API/Dockerfile
```

**Giải thích:**
- `services:`: Khai báo danh sách các services (containers) sẽ chạy
- `catalog.api`: Tên service (có thể dùng để reference)
- `image: ${DOCKER_REGISTRY-}catalogapi`: 
  - Tên image sau khi build
  - `${DOCKER_REGISTRY-}`: Biến môi trường (nếu có thì dùng, không có thì bỏ qua)
- `build:`: Cách build image
  - `context: .`: Build từ thư mục hiện tại (root project)
  - `dockerfile: Services/Catalog/Catalog.API/Dockerfile`: Đường dẫn đến Dockerfile

#### 2. Database Services
```yaml
catalog.db:
  image: mongo

basket.db:
  image: redis:alpine
```

**Giải thích:**
- `catalog.db`: Service MongoDB
  - `image: mongo`: Dùng image MongoDB chính thức từ Docker Hub
- `basket.db`: Service Redis
  - `image: redis:alpine`: Dùng image Redis phiên bản Alpine (nhẹ hơn)

#### 3. Volumes Section
```yaml
volumes:
  mongo_data:
  portainer_data:
```

**Giải thích:**
- Định nghĩa **named volumes** để lưu trữ dữ liệu
- `mongo_data`: Volume cho MongoDB (lưu database)
- `portainer_data`: Volume cho Portainer (lưu cấu hình)

---

## 📄 docker-compose.override.yml (File override)

### Mục đích:
- **Bổ sung/thay đổi** cấu hình từ file cơ bản
- Thường dùng cho **development environment**
- Tự động được merge với `docker-compose.yml`

### Cách hoạt động:
- Docker Compose **tự động đọc** cả 2 file
- Các cấu hình trong `override` sẽ **merge** hoặc **override** file cơ bản
- Chỉ áp dụng khi chạy `docker-compose up` (không áp dụng khi deploy production)

### Phân tích từng phần:

#### 1. Catalog API - Environment Variables
```yaml
catalog.api:
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - ASPNETCORE_HTTP_PORTS=8080
    - "DatabaseSettings__ConnectionString=mongodb://catalog.db:27017?..."
```

**Giải thích:**
- `environment:`: Định nghĩa biến môi trường cho container
- `ASPNETCORE_ENVIRONMENT=Development`: Chế độ development
- `DatabaseSettings__ConnectionString`: Connection string đến MongoDB
  - `catalog.db:27017`: Tên service (Docker tự động resolve DNS)
  - `__` (double underscore): Cách .NET đọc nested config

#### 2. Ports Mapping
```yaml
ports:
  - "8000:8080"
```

**Giải thích:**
- Format: `"HOST_PORT:CONTAINER_PORT"`
- `8000`: Port trên máy host (truy cập từ localhost:8000)
- `8080`: Port trong container (app chạy ở port 8080)

#### 3. Depends On
```yaml
depends_on:
  catalog.db:
    condition: service_healthy
```

**Giải thích:**
- `depends_on`: Đảm bảo `catalog.db` chạy trước `catalog.api`
- `condition: service_healthy`: Đợi đến khi database healthy (pass healthcheck)

#### 4. Healthcheck (MongoDB)
```yaml
catalog.db:
  healthcheck:
    test: echo 'db.runCommand("ping").ok' | mongosh localhost:27017/test --quiet
    interval: 10s
    timeout: 5s
    retries: 5
    start_period: 40s
```

**Giải thích:**
- `test`: Lệnh kiểm tra health (ping MongoDB)
- `interval: 10s`: Kiểm tra mỗi 10 giây
- `timeout: 5s`: Timeout cho mỗi lần check
- `retries: 5`: Số lần retry nếu fail
- `start_period: 40s`: Đợi 40s trước khi bắt đầu check (cho DB khởi động)

#### 5. Volumes Mapping
```yaml
volumes:
  - mongo_data:/data/db
```

**Giải thích:**
- Mount volume `mongo_data` vào `/data/db` trong container
- Dữ liệu MongoDB được lưu persistent (không mất khi restart)

---

## 🔄 Cách 2 file hoạt động cùng nhau

### Khi chạy `docker-compose up`:

1. **Đọc file cơ bản** (`docker-compose.yml`)
   - Định nghĩa: services, images, build config, volumes

2. **Đọc file override** (`docker-compose.override.yml`)
   - Thêm: environment variables, ports, depends_on, healthcheck

3. **Merge lại** thành 1 config hoàn chỉnh:
   ```yaml
   # Kết quả merge:
   catalog.api:
     image: ${DOCKER_REGISTRY-}catalogapi
     build:
       context: .
       dockerfile: Services/Catalog/Catalog.API/Dockerfile
     environment:          # ← Từ override
       - ASPNETCORE_ENVIRONMENT=Development
     ports:                # ← Từ override
       - "8000:8080"
     depends_on:           # ← Từ override
       catalog.db:
         condition: service_healthy
   ```

---

## 📊 So sánh 2 file

| Aspect | docker-compose.yml | docker-compose.override.yml |
|--------|-------------------|----------------------------|
| **Mục đích** | Cấu trúc cơ bản | Cấu hình development |
| **Dùng cho** | Production + Development | Chỉ Development |
| **Nội dung** | Services, images, volumes | Environment, ports, healthcheck |
| **Commit vào Git?** | ✅ Có | ✅ Có (nhưng có thể ignore) |

---

## 🎯 Lợi ích của cách này

1. **Tách biệt concerns:**
   - File cơ bản: Cấu trúc chung
   - File override: Cấu hình môi trường cụ thể

2. **Linh hoạt:**
   - Production: Chỉ dùng `docker-compose.yml`
   - Development: Tự động merge cả 2 file

3. **Dễ maintain:**
   - Thay đổi cấu hình dev không ảnh hưởng file cơ bản

---

## 💡 Ví dụ thực tế

### Development (dùng cả 2 file):
```bash
docker-compose up
# Tự động đọc cả 2 file và merge
```

### Production (chỉ dùng file cơ bản):
```bash
docker-compose -f docker-compose.yml up
# Chỉ đọc file cơ bản, bỏ qua override
```

---

## 🔍 Các cú pháp quan trọng

### 1. Environment Variables với `__`:
```yaml
"DatabaseSettings__ConnectionString=..."
```
- Trong .NET: `Configuration["DatabaseSettings:ConnectionString"]`
- `__` được convert thành `:` trong .NET

### 2. Service Name = DNS Name:
```yaml
"DatabaseSettings__ConnectionString=mongodb://catalog.db:27017"
```
- `catalog.db` là tên service
- Docker tự động tạo DNS entry
- Các containers có thể giao tiếp qua tên service

### 3. Port Mapping:
```yaml
"8000:8080"  # Host:Container
```
- Truy cập từ host: `http://localhost:8000`
- App trong container chạy ở port 8080

---

## ✅ Tóm tắt

- **docker-compose.yml**: Định nghĩa cấu trúc cơ bản (services, images, volumes)
- **docker-compose.override.yml**: Bổ sung cấu hình development (env, ports, healthcheck)
- **Cách hoạt động**: Tự động merge khi chạy `docker-compose up`
- **Lợi ích**: Tách biệt cấu hình, linh hoạt cho nhiều môi trường

