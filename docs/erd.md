# ERD E-commerce

Sơ đồ dưới đây mô tả mô hình dữ liệu tối giản cho một hệ thống thương mại điện tử.

```mermaid
erDiagram
    users ||--o{ orders : "places"
    categories ||--o{ products : "contains"
    orders ||--|{ order_items : "includes"
    products ||--o{ order_items : "appears in"

    users {
        int id PK
        string full_name
        string email UK
        string password_hash
        datetime created_at
    }

    categories {
        int id PK
        string name UK
        string description
        datetime created_at
    }

    products {
        int id PK
        int category_id FK
        string name
        string description
        decimal price
        int stock_quantity
        datetime created_at
    }

    orders {
        int id PK
        int user_id FK
        string status
        decimal total_amount
        datetime ordered_at
    }

    order_items {
        int id PK
        int order_id FK
        int product_id FK
        int quantity
        decimal unit_price
        decimal line_total
    }
```

## Quan hệ chính

- Một `user` có thể tạo nhiều `order`; mỗi `order` thuộc về đúng một `user`.
- Một `category` có thể chứa nhiều `product`; mỗi `product` thuộc về đúng một `category`.
- Một `order` phải có ít nhất một `order_item`; mỗi `order_item` thuộc về đúng một `order`.
- Một `product` có thể xuất hiện trong nhiều `order_item`; mỗi `order_item` tham chiếu đúng một `product`.
- `order_items` là bảng trung gian giải quyết quan hệ nhiều-nhiều giữa `orders` và `products`. `unit_price` lưu giá tại thời điểm mua để đơn hàng không bị thay đổi khi giá sản phẩm được cập nhật.

## Khóa và ràng buộc đề xuất

- `users.email` và `categories.name` là duy nhất (`UK`).
- Các trường `products.category_id`, `orders.user_id`, `order_items.order_id` và `order_items.product_id` là khóa ngoại (`FK`).
- Nên đặt ràng buộc duy nhất cho cặp `(order_id, product_id)` trong `order_items` để một sản phẩm không bị lặp thành nhiều dòng trong cùng đơn hàng.
- `price`, `unit_price`, `line_total`, `quantity` và `stock_quantity` phải là giá trị không âm; `order_items.quantity` phải lớn hơn `0`.
