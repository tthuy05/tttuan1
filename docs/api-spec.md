# Đặc tả RESTful API - Library API

**Base URL:** `http://localhost:5200`  
**Content-Type:** `application/json`

## Danh sách API

| Method | Endpoint | Mô tả | Request Body | Response thành công | Status Code |
|---|---|---|---|---|---|
| `GET` | `/api/v1/health-check` | Kiểm tra API đang hoạt động | Không có | `{ "status": "ok" }` | `200 OK` |
| `GET` | `/api/v1/books` | Lấy danh sách sách; hỗ trợ phân trang, lọc và sắp xếp | Không có | Đối tượng phân trang chứa mảng `items` | `200 OK`, `400 Bad Request` |
| `GET` | `/api/v1/books/{id}` | Lấy một cuốn sách theo `id` | Không có | Đối tượng `Book` | `200 OK`, `404 Not Found` |
| `POST` | `/api/v1/books` | Tạo sách mới | `{ title, author, category, publishedYear }` | `Book` vừa tạo; header `Location` trỏ tới tài nguyên mới | `201 Created`, `400 Bad Request` |
| `PATCH` | `/api/v1/books/{id}` | Cập nhật một phần thông tin sách | `{ title?, author?, category?, publishedYear? }` | `Book` sau khi cập nhật | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/v1/books/{id}` | Xóa sách theo `id` | Không có | Không có nội dung | `204 No Content`, `404 Not Found` |

> Khi xảy ra lỗi ngoài dự kiến, các endpoint trả về `500 Internal Server Error` theo cấu trúc lỗi chung và không làm lộ thông tin nội bộ.

## Mô hình dữ liệu

### Book

```json
{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "category": "IT",
  "publishedYear": 2008
}
```

| Trường | Kiểu | Bắt buộc | Quy tắc |
|---|---|---:|---|
| `id` | integer | Có trong response | Số nguyên dương, do hệ thống sinh |
| `title` | string | Có | Không được để trống hoặc chỉ chứa khoảng trắng; tối đa `200` ký tự |
| `author` | string | Có | Không được để trống hoặc chỉ chứa khoảng trắng; tối đa `150` ký tự |
| `category` | string | Có | Không được để trống hoặc chỉ chứa khoảng trắng; tối đa `100` ký tự |
| `publishedYear` | integer | Có | Từ `1000` đến năm hiện tại |

### CreateBookRequest

```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "category": "IT",
  "publishedYear": 2008
}
```

### UpdateBookRequest

Chỉ gửi các trường cần thay đổi. Ít nhất một trường phải được cung cấp; trường đã gửi vẫn phải thỏa mãn quy tắc kiểm tra dữ liệu.

```json
{
  "category": "Software Engineering",
  "publishedYear": 2009
}
```

## Phân trang, lọc và sắp xếp

`GET /api/v1/books` hỗ trợ các query parameter sau:

| Tham số | Kiểu | Mặc định | Mô tả |
|---|---|---:|---|
| `page` | integer | `1` | Trang cần lấy; từ `1` đến `1,000,000` |
| `pageSize` | integer | `10` | Số bản ghi trên một trang; từ `1` đến `100` |
| `category` | string | Không có | Lọc sách theo thể loại, không phân biệt chữ hoa/thường |
| `sort` | string | `id` | Trường sắp xếp: `id`, `title`, `author`, `category`, `publishedYear`; thêm tiền tố `-` để giảm dần |

Ví dụ:

```http
GET /api/v1/books?page=1&pageSize=10&category=IT&sort=title
```

Response `200 OK`:

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
  "totalItems": 1,
  "totalPages": 1
}
```

Nếu không có kết quả, API vẫn trả `200 OK` với `items: []`, `totalItems: 0` và `totalPages: 0`.

## Response theo từng thao tác

### Lấy chi tiết sách

```http
GET /api/v1/books/1
```

Response `200 OK` là một đối tượng `Book`. Nếu `id` không tồn tại, API trả `404 Not Found`.

### Tạo sách

```http
POST /api/v1/books
Content-Type: application/json
```

Request body dùng `CreateBookRequest`. Response `201 Created` trả về đối tượng `Book` có `id` mới và header ví dụ:

```http
Location: /api/v1/books/4
```

### Cập nhật sách

```http
PATCH /api/v1/books/1
Content-Type: application/json
```

Request body dùng `UpdateBookRequest`. Response `200 OK` trả về đối tượng `Book` sau khi cập nhật.

### Xóa sách

```http
DELETE /api/v1/books/1
```

Response thành công là `204 No Content` và không có response body.

## Cấu trúc lỗi

API dùng định dạng `ProblemDetails` thống nhất.

### `400 Bad Request` - dữ liệu không hợp lệ

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "Title is required."
    ]
  },
  "traceId": "00-..."
}
```

Lỗi `400` được trả về khi body không đúng định dạng, dữ liệu vi phạm quy tắc, `page`/`pageSize` ngoài phạm vi hoặc giá trị `sort` không được hỗ trợ.

### `404 Not Found` - không tìm thấy sách

```json
{
  "title": "Book not found",
  "status": 404,
  "detail": "No book with id 999 was found."
}
```

### `500 Internal Server Error` - lỗi hệ thống

Lỗi ngoài dự kiến được trả về theo định dạng `ProblemDetails` với `status` bằng `500`. Các trường mô tả do framework tạo có thể thay đổi theo môi trường; ví dụ minh họa:

```json
{
  "title": "<mô tả lỗi chung>",
  "status": 500
}
```

