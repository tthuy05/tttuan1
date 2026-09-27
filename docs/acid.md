# ACID trong cơ sở dữ liệu

ACID là bốn tính chất giúp một transaction (giao dịch) xử lý dữ liệu an toàn và đáng tin cậy.

## 1. Atomicity - Tính nguyên tử

Một transaction phải được thực hiện **toàn bộ hoặc không thực hiện gì cả**. Nếu một bước thất bại, các bước đã chạy trước đó sẽ được hoàn tác (`ROLLBACK`).

Ví dụ: khi chuyển tiền, thao tác trừ tiền ở tài khoản A và cộng tiền vào tài khoản B phải cùng thành công. Không thể chỉ trừ tiền ở A mà chưa cộng vào B.

## 2. Consistency - Tính nhất quán

Transaction phải đưa cơ sở dữ liệu từ một trạng thái hợp lệ sang một trạng thái hợp lệ khác. Các ràng buộc như khóa chính, khóa ngoại, `NOT NULL`, `UNIQUE` và quy tắc nghiệp vụ vẫn phải được bảo đảm.

Ví dụ: sau khi chuyển tiền, tổng tiền của A và B không đổi; số dư không được vi phạm quy tắc đã đặt ra.

## 3. Isolation - Tính cô lập

Các transaction chạy đồng thời không được làm sai kết quả của nhau. Một transaction chưa hoàn tất không nên để transaction khác nhìn thấy trạng thái dữ liệu dở dang.

Ví dụ: hai yêu cầu cùng chuyển tiền từ một tài khoản phải được kiểm soát để không cùng đọc một số dư cũ rồi chi vượt số tiền hiện có.

## 4. Durability - Tính bền vững

Sau khi transaction đã `COMMIT`, dữ liệu phải được lưu bền vững và không bị mất chỉ vì ứng dụng dừng hoặc máy chủ khởi động lại.

Ví dụ: khi hệ thống báo chuyển tiền thành công, kết quả vẫn còn trong cơ sở dữ liệu sau khi ứng dụng chạy lại.

## Ví dụ transaction đơn giản

Ví dụ dưới đây chỉ dùng để minh họa. Hai câu lệnh cập nhật được xem là một đơn vị công việc:

```sql
BEGIN TRANSACTION;

UPDATE accounts
SET balance = balance - 100000
WHERE id = 1;

UPDATE accounts
SET balance = balance + 100000
WHERE id = 2;

COMMIT;
```

Nếu câu lệnh thứ hai gặp lỗi, ứng dụng phải gọi `ROLLBACK` thay vì `COMMIT`. Khi đó, việc trừ tiền ở tài khoản 1 cũng bị hủy. Đây là biểu hiện rõ nhất của **Atomicity**; các ràng buộc dữ liệu hỗ trợ **Consistency**, cơ chế transaction của hệ quản trị hỗ trợ **Isolation**, và dữ liệu sau `COMMIT` thể hiện **Durability**.

Trong mini-project Library API, Entity Framework Core thực hiện các thao tác ghi bằng `SaveChangesAsync()`. Một lần gọi `SaveChangesAsync()` được EF Core bao bọc trong transaction khi nhà cung cấp cơ sở dữ liệu hỗ trợ, nhờ đó các thay đổi của lần lưu đó được áp dụng đồng bộ.
