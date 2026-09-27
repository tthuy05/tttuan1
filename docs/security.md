# Authentication, Authorization và bảo mật mật khẩu

## Authentication là gì?

**Authentication (xác thực)** là quá trình kiểm tra người dùng là ai. Người dùng thường cung cấp thông tin đăng nhập như email và mật khẩu; nếu hợp lệ, hệ thống cấp một phiên đăng nhập hoặc token.

Ví dụ: hệ thống kiểm tra email và mật khẩu để xác nhận người đang đăng nhập là tài khoản `student@example.com`.

## Authorization là gì?

**Authorization (phân quyền)** là quá trình kiểm tra người dùng đã được xác thực có quyền thực hiện một hành động hay truy cập một tài nguyên hay không.

Ví dụ: mọi người dùng đã đăng nhập đều có thể xem sách, nhưng chỉ người có vai trò `Admin` mới được xóa sách.

## Điểm khác nhau

| Tiêu chí | Authentication | Authorization |
|---|---|---|
| Câu hỏi chính | Bạn là ai? | Bạn được phép làm gì? |
| Dữ liệu thường dùng | Email, mật khẩu, mã OTP | Role, policy, permission |
| Thời điểm | Thực hiện trước | Thực hiện sau khi xác thực |
| Kết quả lỗi thường gặp | `401 Unauthorized` | `403 Forbidden` |

Lưu ý: tên status code `401 Unauthorized` có thể gây nhầm lẫn, nhưng trong thực tế nó thường biểu thị yêu cầu **chưa được xác thực hợp lệ**. `403 Forbidden` biểu thị danh tính đã được biết nhưng **không đủ quyền**.

## Cấu trúc JWT

JWT (JSON Web Token) thường có ba phần, được nối bằng dấu chấm:

```text
xxxxx.yyyyy.zzzzz
Header.Payload.Signature
```

### Header

Header mô tả loại token và thuật toán ký, ví dụ `typ: JWT` và `alg: HS256`.

### Payload

Payload chứa các claim về người dùng và token, ví dụ:

- `sub`: định danh người dùng.
- `role`: vai trò của người dùng.
- `iat`: thời điểm phát hành.
- `exp`: thời điểm hết hạn.

Payload chỉ được mã hóa Base64URL, **không phải mã hóa bảo mật**. Vì vậy không được đặt mật khẩu, khóa bí mật hoặc dữ liệu nhạy cảm vào payload.

### Signature

Signature được tạo từ Header, Payload và khóa ký. Server kiểm tra chữ ký để phát hiện token đã bị sửa. Khóa ký phải được lưu trong cấu hình an toàn hoặc biến môi trường, không viết trực tiếp vào source code.

JWT nên có thời hạn ngắn, được truyền qua HTTPS và được kiểm tra chữ ký, thời hạn, issuer và audience trước khi chấp nhận.

## Password hashing với BCrypt hoặc Argon2

Mật khẩu không được lưu dưới dạng văn bản thuần và cũng không nên lưu bằng cách mã hóa có thể giải ngược. Server cần lưu **password hash** bằng thuật toán chuyên dụng như **BCrypt** hoặc **Argon2**.

Quy trình cơ bản:

1. Khi đăng ký, server tạo salt ngẫu nhiên và băm mật khẩu bằng BCrypt hoặc Argon2.
2. Database chỉ lưu chuỗi hash, trong đó có thông tin cần thiết để kiểm tra lại.
3. Khi đăng nhập, hàm `Verify` so sánh mật khẩu người dùng nhập với hash đã lưu.
4. Hệ thống không giải mã hash để lấy lại mật khẩu gốc.

BCrypt và Argon2 là các thuật toán có chi phí tính toán điều chỉnh được, giúp làm chậm việc thử hàng loạt mật khẩu khi dữ liệu bị lộ. Argon2id còn được thiết kế để tiêu tốn bộ nhớ, tăng khả năng chống tấn công bằng phần cứng chuyên dụng. Dù dùng thuật toán nào, ứng dụng vẫn cần đặt tham số chi phí phù hợp, dùng HTTPS, giới hạn số lần đăng nhập sai và không ghi mật khẩu vào log.

Mini-project Tuần 1 chưa triển khai đăng nhập JWT để giữ phạm vi đơn giản. Khi bổ sung tính năng này, Authentication nên được xử lý bởi middleware/dịch vụ riêng; quyền truy cập endpoint được khai báo bằng role hoặc policy thay vì viết lẫn vào logic CRUD sách.
