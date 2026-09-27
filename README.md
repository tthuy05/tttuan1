# Library API - Mini-project Backend Tuần 1

Library API là RESTful Web API nhỏ dùng để quản lý sách trong thư viện. Project được xây dựng cho báo cáo thực tập Tuần 1, tập trung vào code dễ đọc, chạy được trên máy cá nhân và thể hiện rõ luồng **Controller → Service → Repository → Database**.

## Mục tiêu

- Thực hành thiết kế RESTful API với resource dạng danh từ số nhiều.
- Thực hiện đầy đủ CRUD cho sách bằng các HTTP method phù hợp.
- Hỗ trợ phân trang, lọc theo thể loại và sắp xếp danh sách sách.
- Áp dụng kiến trúc ba lớp và Dependency Injection.
- Truy cập database bất đồng bộ bằng `async`/`await`.
- Sử dụng Entity Framework Core, migration và SQLite.
- Trả về HTTP status code phù hợp cho từng trường hợp.
- Có integration test, Postman collection và tài liệu phục vụ báo cáo.

## Công nghệ và phiên bản

| Thành phần | Phiên bản / lựa chọn |
|---|---|
| .NET SDK | `10.0.400` |
| Target framework | `net10.0` |
| ASP.NET Core Web API | Controller-based API |
| Entity Framework Core | `10.0.11` |
| Database | SQLite |
| API documentation | OpenAPI tích hợp của ASP.NET Core `10.0.11` |
| Test | xUnit `2.9.3`, ASP.NET Core MVC Testing `10.0.11` |
| EF CLI local tool | `dotnet-ef` `10.0.11` |

Phiên bản SDK được khai báo trong `global.json`. Project đã được phát triển với .NET SDK `10.0.400`.

## Kiến trúc

```text
HTTP Request
    ↓
Controller
    ↓ IBookService
BookService
    ↓ IBookRepository
BookRepository
    ↓
LibraryDbContext
    ↓
SQLite (library.db)
```

Vai trò của từng lớp:

- **Controller** nhận request, gọi service và chuyển kết quả thành HTTP response.
- **Service** xử lý nghiệp vụ, chuẩn hóa dữ liệu và ánh xạ entity sang DTO.
- **Repository** thực hiện truy vấn, phân trang, lọc, sắp xếp và ghi dữ liệu bằng EF Core.
- **LibraryDbContext** mô tả mô hình dữ liệu và kết nối tới SQLite.
- **Dependency Injection** trong `Program.cs` liên kết `IBookService` với `BookService` và `IBookRepository` với `BookRepository`.

`BooksController` không truy cập `LibraryDbContext` trực tiếp. Cách tách này giúp từng lớp có trách nhiệm rõ ràng và dễ thay thế khi kiểm thử.

## Cấu trúc thư mục

```text
tttuan1/
├── Controllers/
│   ├── BooksController.cs
│   └── HealthCheckController.cs
├── Data/
│   ├── LibraryDbContext.cs
│   └── Migrations/
│       ├── 20260927112245_InitialCreate.cs
│       ├── 20260927112245_InitialCreate.Designer.cs
│       └── LibraryDbContextModelSnapshot.cs
├── DTOs/
│   ├── BookCreateDto.cs
│   ├── BookQueryParameters.cs
│   ├── BookResponseDto.cs
│   ├── BookUpdateDto.cs
│   └── PagedResult.cs
├── Models/
│   └── Book.cs
├── Repositories/
│   ├── IBookRepository.cs
│   └── BookRepository.cs
├── Services/
│   ├── IBookService.cs
│   └── BookService.cs
├── Validation/
│   ├── NotWhiteSpaceAttribute.cs
│   └── PublishedYearAttribute.cs
├── docs/
│   ├── acid.md
│   ├── api-spec.md
│   ├── erd.md
│   ├── security.md
│   ├── solid.md
│   └── sql-practice.sql
├── postman/
│   └── LibraryApi.postman_collection.json
├── tests/
│   └── LibraryApi.Tests/
│       ├── BooksApiTests.cs
│       ├── CustomWebApplicationFactory.cs
│       ├── HealthCheckTests.cs
│       └── LibraryApi.Tests.csproj
├── appsettings.json
├── dotnet-tools.json
├── global.json
├── LibraryApi.csproj
├── LibraryApi.http
├── LibraryApi.sln
└── Program.cs
```

## Yêu cầu môi trường

- .NET SDK `10.0.400`.
- Git nếu clone project từ repository.
- Postman là tùy chọn, chỉ cần khi muốn chạy collection có sẵn.

Kiểm tra SDK:

```powershell
dotnet --version
```

Kết quả mong đợi:

```text
10.0.400
```

## Cài đặt và khởi tạo database

Chạy các lệnh sau tại thư mục gốc, nơi có `LibraryApi.sln`.

### 1. Restore NuGet package

```powershell
dotnet restore LibraryApi.sln
```

### 2. Restore EF Core local tool

Project dùng local tool manifest `dotnet-tools.json`, vì vậy không cần cài `dotnet-ef` global:

```powershell
dotnet tool restore --tool-manifest dotnet-tools.json
```

Có thể kiểm tra phiên bản tool bằng lệnh:

```powershell
dotnet tool run dotnet-ef -- --version
```

### 3. Áp dụng migration

Migration `InitialCreate` đã có sẵn trong `Data/Migrations`. Tạo hoặc cập nhật database bằng lệnh:

```powershell
dotnet tool run dotnet-ef -- database update --project LibraryApi.csproj --startup-project LibraryApi.csproj
```

Lệnh này tạo file `library.db` tại thư mục project và thêm dữ liệu mẫu thông qua migration.

Khi thay đổi model trong tương lai, có thể tạo migration mới bằng lệnh:

```powershell
dotnet tool run dotnet-ef -- migrations add TenMigration --output-dir Data/Migrations --project LibraryApi.csproj --startup-project LibraryApi.csproj
```

Thay `TenMigration` bằng tên ngắn gọn mô tả thay đổi.

## Chạy ứng dụng

```powershell
dotnet run --launch-profile http
```

API mặc định chạy tại:

```text
http://localhost:5200
```

Khi chạy ở môi trường Development, OpenAPI JSON có tại:

```text
http://localhost:5200/openapi/v1.json
```

Nhấn `Ctrl+C` trong terminal để dừng ứng dụng.

## Cấu hình database

Connection string mặc định nằm trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=library.db"
  }
}
```

Có thể ghi đè cấu hình bằng biến môi trường `ConnectionStrings__DefaultConnection`. Hai dấu gạch dưới `__` đại diện cho cấp cấu hình lồng nhau trong .NET.

Ví dụ PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Data Source=C:\data\my-library.db"
dotnet tool run dotnet-ef -- database update --project LibraryApi.csproj --startup-project LibraryApi.csproj
dotnet run --launch-profile http
```

Xóa biến môi trường khỏi terminal hiện tại khi không còn dùng:

```powershell
Remove-Item Env:ConnectionStrings__DefaultConnection
```

Không cần đặt mật khẩu cho database SQLite cục bộ. Nếu project dùng secret trong tương lai, hãy đặt secret trong biến môi trường hoặc secret manager, không ghi trực tiếp vào source code.

File `library.db` và các file tạm của SQLite như `library.db-shm`, `library.db-wal` đã được `.gitignore` bỏ qua, vì vậy chúng không được đưa vào commit.

## Dữ liệu mẫu

Migration tạo sẵn 5 cuốn sách để có thể kiểm tra API ngay:

| Id | Title | Author | Category | Published year |
|---:|---|---|---|---:|
| 1 | Clean Code | Robert C. Martin | IT | 2008 |
| 2 | The Pragmatic Programmer | David Thomas & Andrew Hunt | IT | 1999 |
| 3 | Design Patterns | Erich Gamma et al. | IT | 1994 |
| 4 | Norwegian Wood | Haruki Murakami | Novel | 1987 |
| 5 | A Brief History of Time | Stephen Hawking | Science | 1988 |

Dữ liệu seed được thêm khi áp dụng migration vào database mới; ứng dụng không chèn lại dữ liệu sau mỗi lần khởi động.

## Danh sách API

Base URL mặc định: `http://localhost:5200`

| Method | Endpoint | Chức năng | Status thành công | Status lỗi chính |
|---|---|---|---|---|
| `GET` | `/api/v1/health-check` | Kiểm tra API đang hoạt động | `200 OK` | `500 Internal Server Error` |
| `GET` | `/api/v1/books` | Lấy danh sách sách | `200 OK` | `400 Bad Request` |
| `GET` | `/api/v1/books/{id}` | Lấy sách theo id | `200 OK` | `404 Not Found` |
| `POST` | `/api/v1/books` | Tạo sách | `201 Created` | `400 Bad Request` |
| `PATCH` | `/api/v1/books/{id}` | Cập nhật một phần sách | `200 OK` | `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/v1/books/{id}` | Xóa sách | `204 No Content` | `404 Not Found` |

Đặc tả request, response và status code đầy đủ nằm trong `docs/api-spec.md`.

### Phân trang, lọc và sắp xếp

`GET /api/v1/books` hỗ trợ:

| Query parameter | Mặc định | Quy tắc |
|---|---:|---|
| `page` | `1` | Từ `1` đến `1,000,000` |
| `pageSize` | `10` | Từ `1` đến `100` |
| `category` | Không có | Lọc chính xác theo thể loại, không phân biệt chữ hoa/thường |
| `sort` | `id` | `id`, `title`, `author`, `category`, `publishedYear`; thêm `-` để sắp xếp giảm dần |

Ví dụ sắp xếp tăng dần theo tiêu đề:

```http
GET /api/v1/books?page=1&pageSize=10&category=IT&sort=title
```

Ví dụ sắp xếp giảm dần theo năm xuất bản:

```http
GET /api/v1/books?page=1&pageSize=5&sort=-publishedYear
```

Response danh sách có dạng:

```json
{
  "items": [
    {
      "id": 1,
      "title": "Clean Code",
      "author": "Robert C. Martin",
      "category": "IT",
      "publishedYear": 2008
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalItems": 3,
  "totalPages": 1
}
```

## Ví dụ request

Các ví dụ dưới đây dùng base URL `http://localhost:5200`. Có thể gửi trực tiếp bằng file `LibraryApi.http`, Postman hoặc công cụ HTTP bất kỳ.

### Health check

```http
GET http://localhost:5200/api/v1/health-check
Accept: application/json
```

Response `200 OK`:

```json
{
  "status": "ok"
}
```

### Lấy chi tiết sách

```http
GET http://localhost:5200/api/v1/books/1
Accept: application/json
```

### Tạo sách

```http
POST http://localhost:5200/api/v1/books
Content-Type: application/json

{
  "title": "ASP.NET Core in Action",
  "author": "Andrew Lock",
  "category": "IT",
  "publishedYear": 2023
}
```

API trả `201 Created`, đối tượng sách có `id` mới và header `Location` trỏ tới endpoint chi tiết.

### Cập nhật một phần sách

```http
PATCH http://localhost:5200/api/v1/books/1
Content-Type: application/json

{
  "category": "Software Engineering"
}
```

Body phải chứa ít nhất một trong các trường `title`, `author`, `category`, `publishedYear`.

### Xóa sách

```http
DELETE http://localhost:5200/api/v1/books/1
```

API trả `204 No Content` khi xóa thành công.

### Kiểm tra trường hợp không tìm thấy

```http
GET http://localhost:5200/api/v1/books/999999
```

API trả `404 Not Found` theo định dạng `ProblemDetails`.

## Kiểm tra nhanh bằng PowerShell

Mở terminal thứ nhất và chạy API:

```powershell
dotnet run --launch-profile http
```

Mở terminal thứ hai để kiểm tra health check:

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:5200/api/v1/health-check"
```

Kết quả phải có `status` bằng `ok`.

Kiểm tra danh sách sách có phân trang, lọc và sắp xếp:

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:5200/api/v1/books?page=1&pageSize=10&category=IT&sort=title"
```

Tạo thử một cuốn sách:

```powershell
$body = @{
    title = "Refactoring"
    author = "Martin Fowler"
    category = "IT"
    publishedYear = 1999
} | ConvertTo-Json

Invoke-RestMethod `
    -Method Post `
    -Uri "http://localhost:5200/api/v1/books" `
    -ContentType "application/json" `
    -Body $body
```

## Build và chạy test

Build toàn bộ solution:

```powershell
dotnet build LibraryApi.sln --no-restore
```

Chạy toàn bộ test:

```powershell
dotnet test LibraryApi.sln
```

Integration test dùng SQLite in-memory riêng, không đọc hoặc thay đổi `library.db`. Các test hiện có kiểm tra:

- Health check trả `200 OK` và `{ "status": "ok" }`.
- Phân trang, lọc category và sắp xếp title.
- Toàn bộ luồng tạo, xem, cập nhật và xóa sách.
- Dữ liệu không hợp lệ trả `400 Bad Request`.
- `GET`, `PATCH`, `DELETE` với id không tồn tại trả `404 Not Found`.

## Kiểm tra bằng Postman

1. Chạy API tại `http://localhost:5200`.
2. Mở Postman và chọn **Import**.
3. Import file `postman/LibraryApi.postman_collection.json`.
4. Kiểm tra collection variable `baseUrl` có giá trị `http://localhost:5200`.
5. Chạy collection theo thứ tự đã sắp xếp.

Collection gồm:

- Health Check
- Get Books
- Create Book
- Get Book By Id
- Update Book
- Delete Book
- Get Book Not Found

Request **Create Book** tự lưu id vừa tạo vào biến `bookId` để các request sau dùng lại. Collection không chứa token, mật khẩu hoặc secret.

## Tài liệu bổ sung

| File | Nội dung |
|---|---|
| `docs/api-spec.md` | Đặc tả toàn bộ REST API |
| `docs/erd.md` | ERD Mermaid cho mô hình e-commerce |
| `docs/acid.md` | Giải thích bốn tính chất ACID |
| `docs/security.md` | Authentication, Authorization, JWT và password hashing |
| `docs/solid.md` | SRP và DIP được áp dụng trong project |
| `docs/sql-practice.sql` | Bài tập SQLite độc lập về JOIN và hàm tổng hợp |

`docs/sql-practice.sql` dùng các bảng tạm (`TEMP TABLE`), vì vậy không thêm bảng vào database Books.

## Gợi ý ảnh chụp cho báo cáo

- **Hình 2 - Đặc tả RESTful API:** mở `docs/api-spec.md` ở chế độ Markdown Preview.
- **Hình 3 - ERD E-commerce:** mở `docs/erd.md` trên GitHub hoặc trình xem Markdown hỗ trợ Mermaid.
- **Hình 5 - Controller → Service → Repository:** mở cạnh nhau `Controllers/BooksController.cs`, `Services/BookService.cs` và `Repositories/BookRepository.cs`.
- **Hình 7 - Cấu trúc thư mục:** mở Explorer của VS Code tại thư mục project hoặc chụp cây thư mục trong README này.
- **Hình 8 - Health check:** chạy request **Health Check** trong Postman và chụp response `200 OK` với `{ "status": "ok" }`.
- **Hình 9 - GitHub Repository và commit history:** commit rồi push thủ công sau khi kiểm tra project; project không tự tạo remote hoặc tự push.

## Quy trình chạy lại ngắn gọn

```powershell
dotnet restore LibraryApi.sln
dotnet tool restore --tool-manifest dotnet-tools.json
dotnet tool run dotnet-ef -- database update --project LibraryApi.csproj --startup-project LibraryApi.csproj
dotnet build LibraryApi.sln --no-restore
dotnet test LibraryApi.sln --no-build
dotnet run --launch-profile http --no-build
```

Sau đó mở `http://localhost:5200/api/v1/health-check` để xác nhận API hoạt động.
