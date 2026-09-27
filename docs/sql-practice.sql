-- =============================================================
-- SQL PRACTICE: E-COMMERCE DEMO (SQLite)
-- =============================================================
-- Đây là schema luyện tập độc lập, KHÔNG phải schema của Library API.
-- Tất cả bảng đều là TEMP TABLE: chỉ tồn tại trong connection hiện tại
-- và không được ghi vào database Books, kể cả khi mở nhầm database đó.
--
-- Có thể chạy bằng SQLite CLI từ thư mục gốc project:
--   sqlite3 :memory: ".read docs/sql-practice.sql"
-- =============================================================

PRAGMA foreign_keys = ON;

-- Chỉ xóa bảng tạm cùng tên trong connection hiện tại.
DROP TABLE IF EXISTS temp.order_items;
DROP TABLE IF EXISTS temp.orders;
DROP TABLE IF EXISTS temp.products;
DROP TABLE IF EXISTS temp.categories;
DROP TABLE IF EXISTS temp.users;

CREATE TEMP TABLE users (
    id         INTEGER PRIMARY KEY,
    full_name  TEXT NOT NULL,
    email      TEXT NOT NULL UNIQUE
);

CREATE TEMP TABLE categories (
    id    INTEGER PRIMARY KEY,
    name  TEXT NOT NULL UNIQUE
);

CREATE TEMP TABLE products (
    id           INTEGER PRIMARY KEY,
    category_id  INTEGER NOT NULL,
    name         TEXT NOT NULL,
    price        REAL NOT NULL CHECK (price >= 0),
    FOREIGN KEY (category_id) REFERENCES categories(id)
);

CREATE TEMP TABLE orders (
    id          INTEGER PRIMARY KEY,
    user_id     INTEGER NOT NULL,
    order_date  TEXT NOT NULL,
    status      TEXT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TEMP TABLE order_items (
    id          INTEGER PRIMARY KEY,
    order_id    INTEGER NOT NULL,
    product_id  INTEGER NOT NULL,
    quantity    INTEGER NOT NULL CHECK (quantity > 0),
    unit_price  REAL NOT NULL CHECK (unit_price >= 0),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Dữ liệu mẫu được thêm trong một transaction.
BEGIN TRANSACTION;

INSERT INTO users (id, full_name, email) VALUES
    (1, 'Nguyen An',  'an@example.com'),
    (2, 'Tran Binh',  'binh@example.com'),
    (3, 'Le Chi',     'chi@example.com'); -- Chưa có đơn hàng

INSERT INTO categories (id, name) VALUES
    (1, 'IT'),
    (2, 'Van phong pham'),
    (3, 'Gia dung'); -- Chưa có sản phẩm

INSERT INTO products (id, category_id, name, price) VALUES
    (1, 1, 'Laptop',       15000000),
    (2, 1, 'Ban phim',       500000),
    (3, 1, 'Chuot',           300000),
    (4, 2, 'So tay',           50000);

INSERT INTO orders (id, user_id, order_date, status) VALUES
    (1, 1, '2026-09-20', 'completed'),
    (2, 1, '2026-09-21', 'pending'),
    (3, 2, '2026-09-22', 'completed');

INSERT INTO order_items (id, order_id, product_id, quantity, unit_price) VALUES
    (1, 1, 2, 1, 500000),
    (2, 1, 4, 2,  50000),
    (3, 2, 1, 1, 15000000),
    (4, 3, 3, 2, 300000);

COMMIT;

-- =============================================================
-- 1. INNER JOIN
-- Chỉ trả về sản phẩm có category tương ứng.
-- =============================================================
SELECT
    p.id,
    p.name AS product_name,
    c.name AS category_name,
    p.price
FROM products AS p
INNER JOIN categories AS c ON c.id = p.category_id
ORDER BY p.id;

-- =============================================================
-- 2. LEFT JOIN
-- Trả về tất cả user, kể cả Le Chi chưa có đơn hàng.
-- =============================================================
SELECT
    u.id,
    u.full_name,
    o.id AS order_id,
    o.status
FROM users AS u
LEFT JOIN orders AS o ON o.user_id = u.id
ORDER BY u.id, o.id;

-- =============================================================
-- 3. COUNT + GROUP BY
-- Đếm số đơn của từng user; COUNT(o.id) trả 0 cho user chưa có đơn.
-- =============================================================
SELECT
    u.id,
    u.full_name,
    COUNT(o.id) AS order_count
FROM users AS u
LEFT JOIN orders AS o ON o.user_id = u.id
GROUP BY u.id, u.full_name
ORDER BY u.id;

-- =============================================================
-- 4. SUM + GROUP BY
-- Tính tổng tiền của từng đơn từ số lượng và đơn giá lúc đặt hàng.
-- =============================================================
SELECT
    o.id AS order_id,
    u.full_name,
    ROUND(SUM(oi.quantity * oi.unit_price), 2) AS total_amount
FROM orders AS o
INNER JOIN users AS u ON u.id = o.user_id
INNER JOIN order_items AS oi ON oi.order_id = o.id
GROUP BY o.id, u.full_name
ORDER BY o.id;

-- =============================================================
-- 5. AVG + GROUP BY
-- Tính giá trung bình theo category; LEFT JOIN giữ cả category rỗng.
-- =============================================================
SELECT
    c.id,
    c.name AS category_name,
    ROUND(AVG(p.price), 2) AS average_price
FROM categories AS c
LEFT JOIN products AS p ON p.category_id = c.id
GROUP BY c.id, c.name
ORDER BY c.id;
