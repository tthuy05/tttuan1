# Áp dụng SOLID trong Library API

SOLID là nhóm nguyên lý giúp source code dễ hiểu, dễ kiểm thử và dễ thay đổi. Trong phạm vi mini-project này, hai nguyên lý được thể hiện rõ nhất là **Single Responsibility Principle** và **Dependency Inversion Principle**.

## Single Responsibility Principle (SRP)

**Mỗi lớp chỉ nên có một trách nhiệm chính và chỉ có một lý do chính để thay đổi.**

Library API tách luồng xử lý thành các lớp có nhiệm vụ riêng:

| File | Trách nhiệm chính | Khi nào cần thay đổi? |
|---|---|---|
| `Controllers/BooksController.cs` | Nhận HTTP request, gọi service và trả HTTP response/status code | Khi contract hoặc cách xử lý HTTP của Books API thay đổi |
| `Services/BookService.cs` | Xử lý quy tắc nghiệp vụ và điều phối thao tác về sách | Khi quy tắc nghiệp vụ thay đổi |
| `Repositories/BookRepository.cs` | Thực hiện truy vấn và cập nhật dữ liệu sách bằng EF Core | Khi cách truy cập dữ liệu thay đổi |
| `Data/LibraryDbContext.cs` | Khai báo `DbSet` và cấu hình mô hình dữ liệu | Khi mô hình hoặc ánh xạ database thay đổi |

Nhờ cách tách này, `BooksController` không chứa câu lệnh EF Core và không truy cập `LibraryDbContext` trực tiếp. Tương tự, `BookRepository` không quyết định HTTP status code. Đây chính là luồng ba lớp của project:

```text
HTTP Request
    ↓
Controllers/BooksController.cs
    ↓ IBookService
Services/BookService.cs
    ↓ IBookRepository
Repositories/BookRepository.cs
    ↓
Data/LibraryDbContext.cs → SQLite
```

`Controllers/HealthCheckController.cs` cũng được tách riêng vì health check có trách nhiệm khác với CRUD sách.

## Dependency Inversion Principle (DIP)

**Module cấp cao không nên phụ thuộc trực tiếp vào module cấp thấp; cả hai nên phụ thuộc vào abstraction.**

Các dependency trong project được đặt theo abstraction:

- `Controllers/BooksController.cs` nhận `IBookService` qua constructor, thay vì tự tạo `new BookService(...)`.
- `Services/BookService.cs` nhận `IBookRepository`, thay vì phụ thuộc trực tiếp vào `BookRepository` hoặc `LibraryDbContext`.
- `Services/IBookService.cs` mô tả các chức năng nghiệp vụ mà Controller cần.
- `Repositories/IBookRepository.cs` mô tả các thao tác dữ liệu mà Service cần.
- `Program.cs` đăng ký cặp implementation với Dependency Injection container: `IBookService` → `BookService` và `IBookRepository` → `BookRepository`.

Vì vậy, Controller và Service chỉ biết đến contract cần sử dụng. Implementation có thể được thay thế, chẳng hạn dùng repository giả khi unit test, mà không phải sửa logic HTTP trong Controller.

`Repositories/BookRepository.cs` vẫn nhận `Data/LibraryDbContext.cs` qua constructor vì repository là lớp hạ tầng chịu trách nhiệm làm việc với EF Core. DIP trong project được áp dụng tại các ranh giới Controller–Service và Service–Repository; không cần tạo interface cho mọi lớp nếu không đem lại giá trị thực tế.

## Lợi ích đối với mini-project

- Code ngắn và dễ trình bày theo đúng luồng Controller → Service → Repository → Database.
- Có thể kiểm thử Service bằng một implementation giả của `IBookRepository`.
- Có thể thay đổi cách lưu dữ liệu mà ít ảnh hưởng đến Controller.
- Tránh để một lớp vừa xử lý HTTP, vừa chứa nghiệp vụ, vừa truy vấn database.
