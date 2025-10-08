# Инструкции по созданию системы ролей в SQL Server

## Шаги для выполнения в SQL Server Management Studio

### 1. **Создание таблицы Roles**

Выполните следующий SQL скрипт:

```sql
-- Создание таблицы ролей
CREATE TABLE Roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(200),
    created_date DATETIME2 DEFAULT GETDATE()
);

-- Вставка базовых ролей
INSERT INTO Roles (role_name, description) VALUES 
('Admin', 'Администратор системы - полный доступ'),
('User', 'Обычный пользователь - ограниченный доступ');

-- Проверка создания
SELECT * FROM Roles;
```

### 2. **Обновление таблицы User**

```sql
-- Добавление поля role_id в таблицу User
ALTER TABLE [User] 
ADD role_id INT NOT NULL DEFAULT 2; -- По умолчанию роль "User"

-- Добавление внешнего ключа
ALTER TABLE [User]
ADD CONSTRAINT FK_User_Role 
FOREIGN KEY (role_id) REFERENCES Roles(role_id);

-- Обновление существующих пользователей (если есть)
UPDATE [User] SET role_id = 2 WHERE role_id IS NULL;

-- Проверка структуры таблицы
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'User'
ORDER BY ORDINAL_POSITION;
```

### 3. **Создание представления для удобства**

```sql
-- Создание представления для получения пользователей с ролями
CREATE VIEW UserWithRoles AS
SELECT 
    u.user_id,
    u.name,
    u.email,
    u.phone,
    r.role_name,
    r.description as role_description,
    u.role_id
FROM [User] u
INNER JOIN Roles r ON u.role_id = r.role_id;

-- Проверка представления
SELECT * FROM UserWithRoles;
```

### 4. **Создание первого администратора**

```sql
-- Создание первого администратора (замените данные на свои)
INSERT INTO [User] (name, email, phone, role_id) 
VALUES ('Администратор', 'admin@cinema.com', '+7 (999) 123-45-67', 1);

-- Проверка создания
SELECT * FROM UserWithRoles WHERE role_name = 'Admin';
```

### 5. **Проверка работы системы ролей**

```sql
-- Получить всех пользователей с ролями
SELECT * FROM UserWithRoles;

-- Получить только администраторов
SELECT * FROM UserWithRoles WHERE role_name = 'Admin';

-- Получить только обычных пользователей
SELECT * FROM UserWithRoles WHERE role_name = 'User';

-- Изменить роль пользователя (замените user_id на реальный ID)
-- UPDATE [User] SET role_id = 1 WHERE user_id = 1; -- Сделать администратором
-- UPDATE [User] SET role_id = 2 WHERE user_id = 1; -- Сделать обычным пользователем
```

## Структура таблиц после выполнения

### Таблица Roles:
| Поле | Тип | Описание |
|------|-----|----------|
| role_id | INT IDENTITY | Первичный ключ |
| role_name | NVARCHAR(50) | Название роли |
| description | NVARCHAR(200) | Описание роли |
| created_date | DATETIME2 | Дата создания |

### Таблица User (обновленная):
| Поле | Тип | Описание |
|------|-----|----------|
| user_id | INT IDENTITY | Первичный ключ |
| name | NVARCHAR(100) | Имя пользователя |
| email | NVARCHAR(100) | Email |
| phone | NVARCHAR(20) | Телефон |
| role_id | INT | Ссылка на роль (FK) |

## Роли в системе

### 1. **Admin (Администратор)**
- Полный доступ ко всем функциям
- Может управлять пользователями
- Может изменять роли других пользователей
- Доступ к административным функциям

### 2. **User (Обычный пользователь)**
- Ограниченный доступ
- Может создавать бронирования
- Может просматривать свои бронирования
- Не может управлять другими пользователями

## Проверка успешного создания

После выполнения всех скриптов выполните:

```sql
-- Проверка таблиц
SELECT 'Roles table' as TableName, COUNT(*) as RecordCount FROM Roles
UNION ALL
SELECT 'Users table' as TableName, COUNT(*) as RecordCount FROM [User];

-- Проверка связей
SELECT 
    u.name,
    u.email,
    r.role_name,
    r.description
FROM [User] u
INNER JOIN Roles r ON u.role_id = r.role_id
ORDER BY r.role_name, u.name;
```

Если все выполнилось успешно, вы увидите:
- Таблицу Roles с 2 записями (Admin, User)
- Обновленную таблицу User с полем role_id
- Представление UserWithRoles
- Первого администратора в системе
