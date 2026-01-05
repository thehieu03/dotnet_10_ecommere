# 🔧 Fix: ERR_EMPTY_RESPONSE từ Identity Server

## ❌ Lỗi ban đầu

```
GET http://localhost:9009/.well-known/openid-configuration
net::ERR_EMPTY_RESPONSE
```

## 🔍 Nguyên nhân

1. **Thiếu `UseRouting()`** - Middleware routing chưa được cấu hình đúng thứ tự
2. **Thiếu `MapIdentityServer()`** - Identity Server endpoints chưa được map
3. **Kestrel binding conflict** - appsettings.json có cấu hình Kestrel conflict với Docker

## ✅ Cách sửa

### 1. Thêm `UseRouting()` và `MapIdentityServer()`

**File**: `Services/Identity/Identity.API/Program.cs`

```csharp
var app = builder.Build();

// Middleware order is important!
app.UseRouting();  // ✅ Thêm dòng này
app.UseCors();
app.UseIdentityServer();

// Map Identity Server endpoints
app.MapIdentityServer();  // ✅ Thêm dòng này

app.MapGet("/", () => "Identity Server is running!");

app.Run();
```

### 2. Xóa Kestrel configuration trong appsettings.json

**File**: `Services/Identity/Identity.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
  // ✅ Xóa phần Kestrel configuration
}
```

Docker sẽ tự động handle port binding qua environment variable `ASPNETCORE_HTTP_PORTS=8080`.

### 3. Rebuild và restart container

```bash
cd Ecommerce
docker-compose up -d --build identity_api
```

## ✅ Kết quả

Sau khi sửa, Identity Server sẽ:

- ✅ Expose endpoint `.well-known/openid-configuration`
- ✅ Trả về JSON configuration đúng format
- ✅ Frontend có thể kết nối và lấy discovery document

## 🧪 Test

```bash
# Test endpoint
curl http://localhost:9009/.well-known/openid-configuration

# Hoặc trong PowerShell
Invoke-WebRequest -Uri "http://localhost:9009/.well-known/openid-configuration" -UseBasicParsing
```

## 📝 Lưu ý

- Middleware order rất quan trọng: `UseRouting()` → `UseCors()` → `UseIdentityServer()`
- `MapIdentityServer()` cần thiết để expose các Identity Server endpoints
- Trong Docker, không cần cấu hình Kestrel trong appsettings.json, dùng environment variables thay thế
