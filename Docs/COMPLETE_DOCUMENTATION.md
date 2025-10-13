# CinemaBookingApp - Полная документация

## Содержание
1. [Обзор проекта](#обзор-проекта)
2. [Архитектура системы](#архитектура-системы)
3. [Структура проекта](#структура-проекта)
4. [Установка и настройка](#установка-и-настройка)
5. [База данных](#база-данных)
6. [Система авторизации](#система-авторизации)
7. [Детальное описание кода](#детальное-описание-кода)
8. [API и методы](#api-и-методы)
9. [Руководство пользователя](#руководство-пользователя)
10. [Устранение неполадок](#устранение-неполадок)
11. [Разработка и расширение](#разработка-и-расширение)

---

## Обзор проекта

**CinemaBookingApp** - это система бронирования билетов в кинотеатре, разработанная на C# WinForms с использованием SQL Server и Dapper ORM.

### Основные возможности:
- 🎬 Управление фильмами и сеансами
- 🎫 Система бронирования билетов с выбором мест
- 🎟️ **Отмена отдельных билетов** в бронировании
- 🔒 **Личные бронирования** (пользователи видят только свои)
- 🎫 **Ограниченный доступ** в форме создания бронирования
- 👥 Управление пользователями
- 🔐 Система ролей (Администратор/Пользователь)
- 🔍 **Поиск и фильтрация** по всем таблицам
- 📊 Просмотр статистики и отчетов

### Технологический стек:
- **Frontend:** Windows Forms (.NET 9.0)
- **Backend:** C# с ADO.NET и Dapper
- **Database:** Microsoft SQL Server
- **ORM:** Dapper (микро-ORM)
- **Architecture:** Layered Architecture

---

## Архитектура системы

### Диаграмма архитектуры:
```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────┐ │
│  │ LoginForm   │ │ MainForm    │ │ MovieForm   │ │ ...     │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────┘ │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                      Business Layer                         │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐             │
│  │ User        │ │ Movie       │ │ Booking     │ │ ...       │ │
│  │ Management  │ │ Management  │ │ Management  │             │ │
│  └─────────────┘ └─────────────┘ └─────────────┘             │ │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                       Data Access Layer                     │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐             │
│  │ DapperRepo  │ │ DataBaseMgr │ │ Models      │ │ ...       │ │
│  │ sitory      │ │             │ │             │             │ │
│  └─────────────┘ └─────────────┘ └─────────────┘             │ │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                         │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐             │
│  │ SQL Server  │ │ Tables      │ │ Views       │ │ ...       │ │
│  │ Database    │ │             │ │             │             │ │
│  └─────────────┘ └─────────────┘ └─────────────┘             │ │
└─────────────────────────────────────────────────────────────┘
```

### Принципы архитектуры:
- **Separation of Concerns** - разделение ответственности
- **Dependency Injection** - внедрение зависимостей
- **Repository Pattern** - паттерн репозитория
- **Model-View-Controller** - MVC паттерн

---

## Структура проекта

```
CinemaBookingApp/
├── 📁 Data/                          # Слой доступа к данным
│   ├── DatabaseManager.cs           # ADO.NET менеджер БД
│   └── DapperRepository.cs          # Dapper репозиторий
├── 📁 Forms/                         # Пользовательский интерфейс
│   ├── LoginForm.cs                 # Форма авторизации
│   ├── MainForm.cs                  # Главная форма
│   ├── MovieForm.cs                 # Управление фильмами
│   ├── ScreeningForm.cs             # Управление сеансами
│   ├── BookingForm.cs               # Создание бронирований
│   ├── UserForm.cs                  # Управление пользователями
│   └── TicketsForm.cs               # Просмотр билетов
├── 📁 Models/                        # Модели данных
│   ├── User.cs                      # Модель пользователя
│   ├── Movie.cs                     # Модель фильма
│   ├── Screening.cs                 # Модель сеанса
│   ├── Booking.cs                    # Модель бронирования
│   ├── Ticket.cs                     # Модель билета
│   ├── HallType.cs                  # Модель типа зала
│   ├── CinemaHall.cs                # Модель кинозала
│   └── Role.cs                      # Модель роли
├── 📄 Program.cs                     # Точка входа приложения
├── 📄 CinemaBookingApp.csproj       # Файл проекта
├── 📄 README.md                     # Основная документация
├── 📄 DATABASE_CONNECTION.md        # Документация БД
├── 📄 SQL_ROLES_SETUP.md           # Настройка ролей
└── 📄 AUTHORIZATION_GUIDE_UPDATED.md # Руководство по авторизации
```

---

## Установка и настройка

### Требования к системе:
- Windows 10/11
- .NET 9.0 Runtime
- SQL Server 2019 или выше
- Visual Studio 2022 (для разработки)

### Установка зависимостей:

#### 1. Установка .NET 9.0:
```bash
# Скачайте и установите .NET 9.0 SDK с официального сайта Microsoft
# https://dotnet.microsoft.com/download/dotnet/9.0
```

#### 2. Установка SQL Server:
```bash
# Скачайте и установите SQL Server Express или Developer Edition
# https://www.microsoft.com/en-us/sql-server/sql-server-downloads
```

#### 3. Восстановление NuGet пакетов:
```bash
cd CinemaBookingApp
dotnet restore
```

### Настройка базы данных:

#### 1. Создание базы данных:
```sql
CREATE DATABASE [Проект Вакула, Белов, Сухинин];
```

#### 2. Создание таблиц:
```sql
-- Таблица пользователей
CREATE TABLE [User] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    phone NVARCHAR(20),
    role_id INT NOT NULL DEFAULT 2
);

-- Таблица ролей
CREATE TABLE Roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(200),
    created_date DATETIME2 DEFAULT GETDATE()
);

-- Таблица фильмов
CREATE TABLE Movie (
    movie_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(200) NOT NULL,
    description TEXT,
    duration INT NOT NULL,
    genre NVARCHAR(100),
    release_date DATE,
    rating DECIMAL(3,1)
);

-- Таблица типов залов
CREATE TABLE HallType (
    hall_type_id INT IDENTITY(1,1) PRIMARY KEY,
    type_name NVARCHAR(50) NOT NULL,
    capacity INT NOT NULL,
    price_per_seat DECIMAL(10,2) NOT NULL
);

-- Таблица кинозалов
CREATE TABLE CinemaHall (
    hall_id INT IDENTITY(1,1) PRIMARY KEY,
    hall_name NVARCHAR(50) NOT NULL,
    hall_type_id INT NOT NULL,
    FOREIGN KEY (hall_type_id) REFERENCES HallType(hall_type_id)
);

-- Таблица сеансов
CREATE TABLE Screening (
    screening_id INT IDENTITY(1,1) PRIMARY KEY,
    movie_id INT NOT NULL,
    hall_id INT NOT NULL,
    start_time DATETIME2 NOT NULL,
    end_time DATETIME2 NOT NULL,
    FOREIGN KEY (movie_id) REFERENCES Movie(movie_id),
    FOREIGN KEY (hall_id) REFERENCES CinemaHall(hall_id)
);

-- Таблица бронирований
CREATE TABLE Booking (
    booking_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    screening_id INT NOT NULL,
    booking_date DATETIME2 DEFAULT GETDATE(),
    total_price DECIMAL(10,2) NOT NULL,
    status NVARCHAR(20) DEFAULT 'Active',
    FOREIGN KEY (user_id) REFERENCES [User](user_id),
    FOREIGN KEY (screening_id) REFERENCES Screening(screening_id)
);

-- Таблица билетов
CREATE TABLE Ticket (
    ticket_id INT IDENTITY(1,1) PRIMARY KEY,
    booking_id INT NOT NULL,
    seat_number NVARCHAR(10) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (booking_id) REFERENCES Booking(booking_id)
);
```

#### 3. Добавление внешних ключей:
```sql
ALTER TABLE [User]
ADD CONSTRAINT FK_User_Role 
FOREIGN KEY (role_id) REFERENCES Roles(role_id);
```

#### 4. Вставка базовых данных:
```sql
-- Базовые роли
INSERT INTO Roles (role_name, description) VALUES 
('Admin', 'Администратор системы - полный доступ'),
('User', 'Обычный пользователь - ограниченный доступ');

-- Первый администратор
INSERT INTO [User] (name, email, phone, role_id) 
VALUES ('Администратор', 'admin@cinema.com', '+7 (999) 123-45-67', 1);
```

### Настройка подключения:

#### Обновление строки подключения:
В файлах `MainForm.cs`, `LoginForm.cs` и `DatabaseManager.cs` обновите строку подключения:

```csharp
private string connectionString = "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;";
```

---

## База данных

### Схема базы данных:

#### Диаграмма ER (Entity-Relationship):
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    User     │    │    Role     │    │    Movie    │
│─────────────│    │─────────────│    │─────────────│
│ user_id PK  │◄───┤ role_id PK  │    │ movie_id PK │
│ name        │    │ role_name   │    │ title       │
│ email       │    │ description │    │ genre       │
│ phone       │    │ created_date│    │ duration_minutes│
│ role_id FK  │    └─────────────┘    │ description │
└─────────────┘                       └─────────────┘
         │                                      │
         │                                      │
         │                                      │
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Booking   │    │  Screening  │    │ CinemaHall  │
│─────────────│    │─────────────│    │─────────────│
│ booking_id  │◄───┤ screening_id│    │ hall_id PK  │
│ user_id FK  │    │ movie_id FK │◄───┤ hall_name   │
│ order_date  │    │ hall_id FK  │    │ capacity    │
│ total_amount│    │ start_time  │    │ hall_type_id│
│ status      │    │ end_time    │    └─────────────┘
└─────────────┘    └─────────────┘             │
         │                                      │
         │                                      │
         │                                      │
┌─────────────┐                    ┌─────────────┐
│   Ticket    │                    │  HallType   │
│─────────────│                    │─────────────│
│ ticket_id PK│                    │ hall_type_id│
│ booking_id  │                    │ type_name   │
│ seat_number │                    │ capacity    │
│ price       │                    │ price_per_seat│
└─────────────┘                    └─────────────┘
```

#### Связи между сущностями:
- **Пользователь (User)** —< **Бронирование (Booking)** —< **Билет (Ticket)** —> **Сеанс (Screening)** —> **Зал (CinemaHall)**
- **Сеанс (Screening)** относится к **Фильму (Movie)**
- **Зал (CinemaHall)** связан с **Типом зала (HallType)**
- **Пользователь (User)** связан с **Ролью (Role)**

### Описание таблиц:

#### 1. **User** - Пользователи системы
| Поле | Тип | Описание |
|------|-----|----------|
| user_id | INT IDENTITY | Первичный ключ |
| name | NVARCHAR(100) | Имя пользователя |
| email | NVARCHAR(100) | Email (уникальный) |
| phone | NVARCHAR(20) | Телефон |
| role_id | INT | Ссылка на роль |

#### 2. **Roles** - Роли пользователей
| Поле | Тип | Описание |
|------|-----|----------|
| role_id | INT IDENTITY | Первичный ключ |
| role_name | NVARCHAR(50) | Название роли |
| description | NVARCHAR(200) | Описание роли |
| created_date | DATETIME2 | Дата создания |

#### 3. **Movie** - Фильмы
| Поле | Тип | Описание |
|------|-----|----------|
| movie_id | INT IDENTITY | Первичный ключ |
| title | NVARCHAR(200) | Название фильма |
| description | TEXT | Описание |
| duration | INT | Длительность в минутах |
| genre | NVARCHAR(100) | Жанр |
| release_date | DATE | Дата выхода |
| rating | DECIMAL(3,1) | Рейтинг |

#### 4. **Screening** - Сеансы
| Поле | Тип | Описание |
|------|-----|----------|
| screening_id | INT IDENTITY | Первичный ключ |
| movie_id | INT | Ссылка на фильм |
| hall_id | INT | Ссылка на зал |
| start_time | DATETIME2 | Время начала |
| end_time | DATETIME2 | Время окончания |

#### 5. **Booking** - Бронирования
| Поле | Тип | Описание |
|------|-----|----------|
| booking_id | INT IDENTITY | Первичный ключ |
| user_id | INT | Ссылка на пользователя |
| screening_id | INT | Ссылка на сеанс |
| booking_date | DATETIME2 | Дата бронирования |
| total_price | DECIMAL(10,2) | Общая стоимость |
| status | NVARCHAR(20) | Статус бронирования |

#### 6. **Ticket** - Билеты
| Поле | Тип | Описание |
|------|-----|----------|
| ticket_id | INT IDENTITY | Первичный ключ |
| booking_id | INT | Ссылка на бронирование |
| seat_number | NVARCHAR(10) | Номер места |
| price | DECIMAL(10,2) | Цена билета |

**Назначение:** Принадлежит одному бронированию (N к 1). Относится к одному сеансу (N к 1). Отвечает за место и цену билета. Формирует состав заказа.

#### 7. **HallType** - Типы залов
| Поле | Тип | Описание |
|------|-----|----------|
| hall_type_id | INT IDENTITY | Первичный ключ |
| type_name | NVARCHAR(50) | Название типа |
| capacity | INT | Вместимость |
| price_per_seat | DECIMAL(10,2) | Цена за место |

**Назначение:** Характеристика конкретного зала в кинотеатре, определяющая его особенности и формат показываемого кино. Влияет на стоимость билетов.

#### 8. **CinemaHall** - Кинозалы
| Поле | Тип | Описание |
|------|-----|----------|
| hall_id | INT IDENTITY | Первичный ключ |
| hall_name | NVARCHAR(50) | Название зала |
| capacity | INT | Вместимость |
| hall_type_id | INT | Ссылка на тип зала |

**Назначение:** Связан с несколькими сеансами (1 ко многим). Ограничивает количество и нумерацию мест. Организует размещение зрителей.

### Статусы бронирования

В системе бронирования используются следующие статусы для поля `status` в таблице `Booking`:

#### **PENDING** - Ожидает подтверждения
- **Когда устанавливается:** При создании нового бронирования
- **Описание:** Бронирование создано, но еще не подтверждено
- **Действия пользователя:** Может быть отменено или подтверждено
- **Автоматические переходы:** Через определенное время может перейти в `CANCELLED`

#### **COMPLETED** - Завершено
- **Когда устанавливается:** После успешной оплаты и подтверждения
- **Описание:** Бронирование полностью оплачено и подтверждено
- **Действия пользователя:** Билеты действительны для посещения
- **Особенности:** Не может быть отменено, только возвращено

#### **CANCELLED** - Отменено
- **Когда устанавливается:** 
  - Пользователем вручную до подтверждения
  - Автоматически при истечении времени ожидания
  - Администратором по запросу
- **Описание:** Бронирование отменено, места освобождены
- **Действия пользователя:** Может создать новое бронирование
- **Особенности:** Места становятся доступными для других пользователей

#### Логика переходов статусов:

```
PENDING → COMPLETED (при оплате)
PENDING → CANCELLED (при отмене или таймауте)
COMPLETED → CANCELLED (только администратором, с возвратом средств)
```

#### Примеры использования статусов:

1. **Пользователь создает бронирование:**
   ```sql
   INSERT INTO Booking (user_id, order_date, total_amount, status) 
   VALUES (1, GETDATE(), 500.00, 'PENDING');
   ```

2. **После оплаты:**
   ```sql
   UPDATE Booking 
   SET status = 'COMPLETED' 
   WHERE booking_id = 123;
   ```

3. **При отмене:**
   ```sql
   UPDATE Booking 
   SET status = 'CANCELLED' 
   WHERE booking_id = 123;
   ```

#### Бизнес-логика статусов:

- **PENDING:** Резервирует места на ограниченное время (например, 15 минут)
- **COMPLETED:** Гарантирует доступ к сеансу
- **CANCELLED:** Освобождает места для повторного бронирования

---

## Система авторизации

### Обзор системы ролей:

#### Роли в системе:
1. **Admin** - Администратор
2. **User** - Обычный пользователь

#### Права доступа:

| Функция | Admin | User |
|---------|-------|------|
| Просмотр фильмов | ✅ | ✅ |
| Добавление фильмов | ✅ | ❌ |
| Редактирование фильмов | ✅ | ❌ |
| Удаление фильмов | ✅ | ❌ |
| Просмотр сеансов | ✅ | ❌ |
| Управление сеансами | ✅ | ❌ |
| Создание бронирований | ✅ | ✅ (только на себя) |
| Просмотр бронирований | ✅ (все) | ✅ (только свои) |
| Отмена бронирований | ✅ (все) | ✅ (только свои) |
| Просмотр билетов | ✅ (все) | ✅ (только свои) |
| Отмена отдельных билетов | ✅ (все) | ✅ (только свои) |
| Поиск и фильтрация | ✅ (все данные) | ✅ (только свои данные) |
| Просмотр билетов | ✅ | ❌ |
| Управление пользователями | ✅ | ❌ |

### Процесс авторизации:

#### 1. Вход администратора:
```csharp
// Логика в LoginForm.cs
if (txtLogin.Text.ToLower() == "admin" && txtPassword.Text == "123")
{
    IsAuthenticated = true;
    UserRole = "Admin";
    UserName = "Администратор";
}
```

#### 2. Вход обычного пользователя:
```csharp
// Поиск пользователя по имени или email
var user = await dapperRepo.GetUserByEmailAsync(txtLogin.Text.Trim());
if (user == null)
{
    user = users.FirstOrDefault(u => u.Name.ToLower().Contains(txtLogin.Text.Trim().ToLower()));
}

// Если не найден, создаем временного пользователя
if (user == null && !string.IsNullOrWhiteSpace(txtLogin.Text.Trim()))
{
    IsAuthenticated = true;
    UserRole = "User";
    UserName = txtLogin.Text.Trim();
}
```

### Проверка прав доступа:

#### Метод проверки администратора:
```csharp
private bool IsCurrentUserAdmin()
{
    return currentUserRole == "Admin";
}

private bool CheckAdminAccess()
{
    if (!IsCurrentUserAdmin())
    {
        MessageBox.Show("Доступ запрещен. Требуются права администратора.", 
                       "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }
    return true;
}
```

---

## Детальное описание кода

### 1. Program.cs - Точка входа

```csharp
using System;
using System.Windows.Forms;
using CinemaBookingApp.Forms;

namespace CinemaBookingApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Показываем форму входа
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Если авторизация успешна, запускаем основное приложение
                    using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserName))
                    {
                        Application.Run(mainForm);
                    }
                }
                else
                {
                    // Если пользователь отменил вход, закрываем приложение
                    Application.Exit();
                }
            }
        }
    }
}
```

**Описание:**
- Инициализирует WinForms приложение
- Показывает форму авторизации
- Передает данные о пользователе в главную форму
- Обрабатывает отмену входа

### 2. LoginForm.cs - Форма авторизации

#### Основные компоненты:
```csharp
public partial class LoginForm : Form
{
    private TextBox txtLogin = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnCancel = null!;
    private Label lblStatus = null!;
    
    private DapperRepository? dapperRepo;
    private string connectionString = "...";
    
    // Свойства для передачи данных
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool IsAuthenticated { get; private set; } = false;
    
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string? UserRole { get; private set; }
    
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string? UserName { get; private set; }
}
```

#### Логика авторизации:
```csharp
private async void BtnLogin_Click(object? sender, EventArgs e)
{
    // Проверка администратора
    if (txtLogin.Text.ToLower() == "admin" && txtPassword.Text == "123")
    {
        IsAuthenticated = true;
        UserRole = "Admin";
        UserName = "Администратор";
        // Успешный вход
    }
    
    // Проверка обычного пользователя
    var user = await dapperRepo!.GetUserByEmailAsync(txtLogin.Text.Trim());
    if (user == null)
    {
        var users = await dapperRepo.GetUsersAsync();
        user = users.FirstOrDefault(u => u.Name.ToLower().Contains(txtLogin.Text.Trim().ToLower()));
    }
    
    if (user != null)
    {
        IsAuthenticated = true;
        UserRole = user.Role?.RoleName ?? "User";
        UserName = user.Name;
        // Успешный вход
    }
    
    // Создание временного пользователя
    if (!string.IsNullOrWhiteSpace(txtLogin.Text.Trim()))
    {
        IsAuthenticated = true;
        UserRole = "User";
        UserName = txtLogin.Text.Trim();
        // Успешный вход
    }
}
```

### 3. MainForm.cs - Главная форма

#### Конструктор с параметрами роли:
```csharp
public MainForm(string? userRole = null, string? userName = null)
{
    currentUserRole = userRole;
    currentUserName = userName;
    
    InitializeComponent();
    dbManager = new DataBaseManager(connectionString);
    dapperRepo = new DapperRepository(connectionString);
    
    if (!dbManager.TestConnection())
    {
        MessageBox.Show("Ошибка подключения к базе данных!");
        return;
    }
    
    LoadMovies();
    LoadBookings();
    
    // Загружаем данные только для администраторов
    if (IsCurrentUserAdmin())
    {
        LoadScreenings();
        LoadUsers();
    }
}
```

#### Адаптивная инициализация интерфейса:
```csharp
private void InitializeComponent()
{
    // Обновляем заголовок с информацией о пользователе
    string title = "Система бронирования кинотеатра";
    if (!string.IsNullOrEmpty(currentUserName))
    {
        title += $" - {currentUserName}";
        if (!string.IsNullOrEmpty(currentUserRole))
        {
            title += $" ({currentUserRole})";
        }
    }
    this.Text = title;
    
    // Добавляем вкладки в зависимости от роли пользователя
    if (IsCurrentUserAdmin())
    {
        // Администратор видит все вкладки
        tabControl.Controls.AddRange(new TabPage[] {
            tabMovies, tabScreenings, tabBookings, tabUsers
        });
    }
    else
    {
        // Обычный пользователь видит только фильмы и бронирования
        tabControl.Controls.AddRange(new TabPage[] {
            tabMovies, tabBookings
        });
    }
}
```

#### Инициализация вкладки фильмов с проверкой роли:
```csharp
private void InitializeMoviesTab()
{
    dataGridMovies.Location = new Point(10, 10);
    dataGridMovies.Size = new Size(850, 400);
    dataGridMovies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dataGridMovies.ReadOnly = true;
    dataGridMovies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    
    // Добавляем кнопки только для администраторов
    if (IsCurrentUserAdmin())
    {
        btnAddMovie.Text = "Добавить фильм";
        btnAddMovie.Location = new Point(10, 420);
        btnAddMovie.Size = new Size(120, 30);
        btnAddMovie.Click += BtnAddMovie_Click;
        
        btnEditMovie.Text = "Редактировать";
        btnEditMovie.Location = new Point(140, 420);
        btnEditMovie.Size = new Size(120, 30);
        btnEditMovie.Click += BtnEditMovie_Click;
        
        btnDeleteMovie.Text = "Удалить";
        btnDeleteMovie.Location = new Point(270, 420);
        btnDeleteMovie.Size = new Size(120, 30);
        btnDeleteMovie.Click += BtnDeleteMovie_Click;
        
        tabMovies.Controls.AddRange(new Control[] {
            dataGridMovies, btnAddMovie, btnEditMovie, btnDeleteMovie
        });
    }
    else
    {
        // Для обычных пользователей только таблица фильмов
        tabMovies.Controls.Add(dataGridMovies);
    }
}
```

### 4. DatabaseManager.cs - ADO.NET менеджер

#### Основная структура:
```csharp
using System.Data.SqlClient;
using CinemaBookingApp.Models;

namespace CinemaBookingApp.Data
{
    public class DataBaseManager
    {
        private readonly string connectionString;
        
        public DataBaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public bool TestConnection()
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

#### Методы для работы с фильмами:
```csharp
public DataTable GetMovies()
{
    using var connection = new SqlConnection(connectionString);
    var command = new SqlCommand("SELECT * FROM Movie ORDER BY title", connection);
    var adapter = new SqlDataAdapter(command);
    var dataTable = new DataTable();
    adapter.Fill(dataTable);
    return dataTable;
}

public void AddMovie(string title, string description, int duration, string genre, DateTime releaseDate, decimal rating)
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    var command = new SqlCommand(@"
        INSERT INTO Movie (title, description, duration, genre, release_date, rating) 
        VALUES (@title, @description, @duration, @genre, @releaseDate, @rating)", connection);
    
    command.Parameters.AddWithValue("@title", title);
    command.Parameters.AddWithValue("@description", description);
    command.Parameters.AddWithValue("@duration", duration);
    command.Parameters.AddWithValue("@genre", genre);
    command.Parameters.AddWithValue("@releaseDate", releaseDate);
    command.Parameters.AddWithValue("@rating", rating);
    
    command.ExecuteNonQuery();
}

public void UpdateMovie(int movieId, string title, string description, int duration, string genre, DateTime releaseDate, decimal rating)
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    var command = new SqlCommand(@"
        UPDATE Movie 
        SET title = @title, description = @description, duration = @duration, 
            genre = @genre, release_date = @releaseDate, rating = @rating 
        WHERE movie_id = @movieId", connection);
    
    command.Parameters.AddWithValue("@movieId", movieId);
    command.Parameters.AddWithValue("@title", title);
    command.Parameters.AddWithValue("@description", description);
    command.Parameters.AddWithValue("@duration", duration);
    command.Parameters.AddWithValue("@genre", genre);
    command.Parameters.AddWithValue("@releaseDate", releaseDate);
    command.Parameters.AddWithValue("@rating", rating);
    
    command.ExecuteNonQuery();
}

public void DeleteMovie(int movieId)
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    var command = new SqlCommand("DELETE FROM Movie WHERE movie_id = @movieId", connection);
    command.Parameters.AddWithValue("@movieId", movieId);
    command.ExecuteNonQuery();
}
```

### 5. DapperRepository.cs - Dapper репозиторий

#### Основная структура:
```csharp
using System.Data;
using System.Data.SqlClient;
using Dapper;
using CinemaBookingApp.Models;

namespace CinemaBookingApp.Data
{
    public class DapperRepository
    {
        private readonly string connectionString;
        
        public DapperRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        private IDbConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
        
        public bool TestConnection()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

#### Методы для работы с пользователями:
```csharp
public async Task<IEnumerable<User>> GetUsersAsync()
{
    const string sql = @"
        SELECT u.user_id as UserId, u.name as Name, u.email as Email, u.phone as Phone, u.role_id as RoleId,
               r.role_name as RoleName, r.description as RoleDescription
        FROM [User] u
        INNER JOIN Roles r ON u.role_id = r.role_id
        ORDER BY u.name";

    using var connection = GetConnection();
    return await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
    {
        user.Role = role;
        return user;
    }, splitOn: "RoleName");
}

public async Task<User?> GetUserByIdAsync(int userId)
{
    const string sql = @"
        SELECT u.user_id as UserId, u.name as Name, u.email as Email, u.phone as Phone, u.role_id as RoleId,
               r.role_name as RoleName, r.description as RoleDescription
        FROM [User] u
        INNER JOIN Roles r ON u.role_id = r.role_id
        WHERE u.user_id = @UserId";

    using var connection = GetConnection();
    return await connection.QueryFirstOrDefaultAsync<User, Role, User>(sql, (user, role) =>
    {
        user.Role = role;
        return user;
    }, new { UserId = userId }, splitOn: "RoleName");
}

public async Task<User?> GetUserByEmailAsync(string email)
{
    const string sql = @"
        SELECT u.user_id as UserId, u.name as Name, u.email as Email, u.phone as Phone, u.role_id as RoleId,
               r.role_name as RoleName, r.description as RoleDescription
        FROM [User] u
        INNER JOIN Roles r ON u.role_id = r.role_id
        WHERE u.email = @Email";

    using var connection = GetConnection();
    return await connection.QueryFirstOrDefaultAsync<User, Role, User>(sql, (user, role) =>
    {
        user.Role = role;
        return user;
    }, new { Email = email }, splitOn: "RoleName");
}

public async Task AddUserAsync(string name, string email, string? phone, int roleId)
{
    const string sql = @"
        INSERT INTO [User] (name, email, phone, role_id) 
        VALUES (@Name, @Email, @Phone, @RoleId)";

    using var connection = GetConnection();
    await connection.ExecuteAsync(sql, new { Name = name, Email = email, Phone = phone, RoleId = roleId });
}

public async Task UpdateUserAsync(int userId, string name, string email, string? phone, int roleId)
{
    const string sql = @"
        UPDATE [User] 
        SET name = @Name, email = @Email, phone = @Phone, role_id = @RoleId 
        WHERE user_id = @UserId";

    using var connection = GetConnection();
    await connection.ExecuteAsync(sql, new { UserId = userId, Name = name, Email = email, Phone = phone, RoleId = roleId });
}

public async Task DeleteUserAsync(int userId)
{
    const string sql = "DELETE FROM [User] WHERE user_id = @UserId";

    using var connection = GetConnection();
    await connection.ExecuteAsync(sql, new { UserId = userId });
}
```

#### Методы для работы с ролями:
```csharp
public async Task<IEnumerable<Role>> GetRolesAsync()
{
    const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles ORDER BY role_name";

    using var connection = GetConnection();
    return await connection.QueryAsync<Role>(sql);
}

public async Task<Role?> GetRoleByIdAsync(int roleId)
{
    const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles WHERE role_id = @RoleId";

    using var connection = GetConnection();
    return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { RoleId = roleId });
}

public async Task<Role?> GetRoleByNameAsync(string roleName)
{
    const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles WHERE role_name = @RoleName";

    using var connection = GetConnection();
    return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { RoleName = roleName });
}

public async Task ChangeUserRoleAsync(int userId, int newRoleId)
{
    const string sql = "UPDATE [User] SET role_id = @NewRoleId WHERE user_id = @UserId";

    using var connection = GetConnection();
    await connection.ExecuteAsync(sql, new { UserId = userId, NewRoleId = newRoleId });
}
```

### 6. Models - Модели данных

#### User.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int RoleId { get; set; } = 2; // По умолчанию роль "User"
        public Role? Role { get; set; }
        
        /// <summary>
        /// Проверяет, является ли пользователь администратором
        /// </summary>
        public bool IsAdmin => Role?.RoleName == "Admin";
        
        /// <summary>
        /// Проверяет, является ли пользователь обычным пользователем
        /// </summary>
        public bool IsUser => Role?.RoleName == "User";
    }
}
```

#### Role.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    /// <summary>
    /// Модель роли пользователя
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
```

#### Movie.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public string? Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? Rating { get; set; }
    }
}
```

#### Screening.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class Screening
    {
        public int ScreeningId { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Movie? Movie { get; set; }
        public CinemaHall? CinemaHall { get; set; }
    }
}
```

#### Booking.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ScreeningId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Active";
        public User? User { get; set; }
        public Screening? Screening { get; set; }
    }
}
```

#### Ticket.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Booking? Booking { get; set; }
    }
}
```

#### HallType.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class HallType
    {
        public int HallTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerSeat { get; set; }
    }
}
```

#### CinemaHall.cs:
```csharp
using System;

namespace CinemaBookingApp.Models
{
    public class CinemaHall
    {
        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public int HallTypeId { get; set; }
        public HallType? HallType { get; set; }
    }
}
```

---

## API и методы

### DatabaseManager API

#### Методы для работы с фильмами:
```csharp
// Получение всех фильмов
DataTable GetMovies()

// Добавление фильма
void AddMovie(string title, string description, int duration, string genre, DateTime releaseDate, decimal rating)

// Обновление фильма
void UpdateMovie(int movieId, string title, string description, int duration, string genre, DateTime releaseDate, decimal rating)

// Удаление фильма
void DeleteMovie(int movieId)
```

#### Методы для работы с сеансами:
```csharp
// Получение всех сеансов
DataTable GetScreenings()

// Добавление сеанса
void AddScreening(int movieId, int hallId, DateTime startTime, DateTime endTime)

// Обновление сеанса
void UpdateScreening(int screeningId, int movieId, int hallId, DateTime startTime, DateTime endTime)

// Удаление сеанса
void DeleteScreening(int screeningId)
```

#### Методы для работы с пользователями:
```csharp
// Получение всех пользователей
DataTable GetUsers()

// Добавление пользователя
void AddUser(string name, string email, string phone, int roleId)

// Обновление пользователя
void UpdateUser(int userId, string name, string email, string phone, int roleId)

// Удаление пользователя
void DeleteUser(int userId)
```

#### Методы для работы с бронированиями:
```csharp
// Получение всех бронирований
DataTable GetBookings()

// Добавление бронирования
void AddBooking(int userId, int screeningId, decimal totalPrice)

// Обновление статуса бронирования
void UpdateBookingStatus(int bookingId, string status)

// Удаление бронирования
void DeleteBooking(int bookingId)
```

### DapperRepository API

#### Асинхронные методы для пользователей:
```csharp
// Получение всех пользователей с ролями
Task<IEnumerable<User>> GetUsersAsync()

// Получение пользователя по ID
Task<User?> GetUserByIdAsync(int userId)

// Получение пользователя по email
Task<User?> GetUserByEmailAsync(string email)

// Добавление пользователя
Task AddUserAsync(string name, string email, string? phone, int roleId)

// Обновление пользователя
Task UpdateUserAsync(int userId, string name, string email, string? phone, int roleId)

// Удаление пользователя
Task DeleteUserAsync(int userId)
```

#### Асинхронные методы для ролей:
```csharp
// Получение всех ролей
Task<IEnumerable<Role>> GetRolesAsync()

// Получение роли по ID
Task<Role?> GetRoleByIdAsync(int roleId)

// Получение роли по названию
Task<Role?> GetRoleByNameAsync(string roleName)

// Изменение роли пользователя
Task ChangeUserRoleAsync(int userId, int newRoleId)

// Получение пользователей по роли
Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
```

#### Асинхронные методы для фильмов:
```csharp
// Получение всех фильмов
Task<IEnumerable<Movie>> GetMoviesAsync()

// Получение фильма по ID
Task<Movie?> GetMovieByIdAsync(int movieId)

// Добавление фильма
Task AddMovieAsync(string title, string? description, int duration, string? genre, DateTime? releaseDate, decimal? rating)

// Обновление фильма
Task UpdateMovieAsync(int movieId, string title, string? description, int duration, string? genre, DateTime? releaseDate, decimal? rating)

// Удаление фильма
Task DeleteMovieAsync(int movieId)
```

#### Асинхронные методы для сеансов:
```csharp
// Получение всех сеансов с фильмами и залами
Task<IEnumerable<Screening>> GetScreeningsAsync()

// Получение сеанса по ID
Task<Screening?> GetScreeningByIdAsync(int screeningId)

// Добавление сеанса
Task AddScreeningAsync(int movieId, int hallId, DateTime startTime, DateTime endTime)

// Обновление сеанса
Task UpdateScreeningAsync(int screeningId, int movieId, int hallId, DateTime startTime, DateTime endTime)

// Удаление сеанса
Task DeleteScreeningAsync(int screeningId)
```

#### Асинхронные методы для бронирований:
```csharp
// Получение всех бронирований с пользователями и сеансами
Task<IEnumerable<Booking>> GetBookingsAsync()

// Получение бронирования по ID
Task<Booking?> GetBookingByIdAsync(int bookingId)

// Добавление бронирования
Task AddBookingAsync(int userId, int screeningId, decimal totalPrice)

// Обновление статуса бронирования
Task UpdateBookingStatusAsync(int bookingId, string status)

// Удаление бронирования
Task DeleteBookingAsync(int bookingId)
```

#### Асинхронные методы для билетов:
```csharp
// Получение билетов по бронированию
Task<IEnumerable<Ticket>> GetTicketsByBookingAsync(int bookingId)

// Добавление билета
Task AddTicketAsync(int bookingId, string seatNumber, decimal price)

// Удаление билета
Task DeleteTicketAsync(int ticketId)
```

---

## Руководство пользователя

### Для администратора

#### Вход в систему:
1. Запустите приложение
2. Введите логин: `admin`
3. Введите пароль: `123`
4. Нажмите "Войти"

#### Управление фильмами:
1. Перейдите на вкладку "Фильмы"
2. Для добавления фильма:
   - Нажмите "Добавить фильм"
   - Заполните форму с данными фильма
   - Нажмите "Сохранить"
3. Для редактирования фильма:
   - Выберите фильм в таблице
   - Нажмите "Редактировать"
   - Измените нужные поля
   - Нажмите "Сохранить"
4. Для удаления фильма:
   - Выберите фильм в таблице
   - Нажмите "Удалить"
   - Подтвердите удаление

#### Управление сеансами:
1. Перейдите на вкладку "Сеансы"
2. Для добавления сеанса:
   - Нажмите "Добавить сеанс"
   - Выберите фильм и зал
   - Укажите время начала и окончания
   - Нажмите "Сохранить"
3. Для редактирования сеанса:
   - Выберите сеанс в таблице
   - Нажмите "Редактировать"
   - Измените нужные поля
   - Нажмите "Сохранить"
4. Для удаления сеанса:
   - Выберите сеанс в таблице
   - Нажмите "Удалить"
   - Подтвердите удаление

#### Управление пользователями:
1. Перейдите на вкладку "Пользователи"
2. Для добавления пользователя:
   - Нажмите "Добавить пользователя"
   - Заполните форму с данными пользователя
   - Выберите роль
   - Нажмите "Сохранить"
3. Для редактирования пользователя:
   - Выберите пользователя в таблице
   - Нажмите "Редактировать"
   - Измените нужные поля
   - Нажмите "Сохранить"
4. Для удаления пользователя:
   - Выберите пользователя в таблице
   - Нажмите "Удалить"
   - Подтвердите удаление

#### Управление бронированиями:
1. Перейдите на вкладку "Бронирования"
2. Для создания бронирования:
   - Нажмите "Новое бронирование"
   - Выберите фильм и сеанс
   - Выберите места
   - Нажмите "Создать бронирование"
3. Для отмены бронирования:
   - Выберите бронирование в таблице
   - Нажмите "Отменить бронь"
   - Подтвердите отмену
4. Для просмотра билетов:
   - Выберите бронирование в таблице
   - Нажмите "Просмотреть билеты"

### Для обычного пользователя

#### Вход в систему:
1. Запустите приложение
2. Введите любое имя (например: `Иван`)
3. Введите любой пароль
4. Нажмите "Войти"

#### Просмотр фильмов:
1. Перейдите на вкладку "Фильмы"
2. Просматривайте список доступных фильмов
3. Информация включает:
   - Название фильма
   - Описание
   - Длительность
   - Жанр
   - Дата выхода
   - Рейтинг

#### Создание бронирования:
1. Перейдите на вкладку "Бронирования"
2. Нажмите "Новое бронирование"
3. Выберите фильм из списка
4. Выберите сеанс из доступных
5. Выберите места на схеме зала
6. Проверьте общую стоимость
7. Нажмите "Создать бронирование"

#### Просмотр бронирований:
1. Перейдите на вкладку "Бронирования"
2. Просматривайте список ваших бронирований
3. Информация включает:
   - Фильм и сеанс
   - Дата бронирования
   - Общая стоимость
   - Статус бронирования

---

## Устранение неполадок

### Проблемы с подключением к базе данных

#### Ошибка: "Ошибка подключения к базе данных"
**Причины:**
- Неверная строка подключения
- SQL Server не запущен
- Неправильные учетные данные
- Проблемы с сетью

**Решения:**
1. Проверьте строку подключения в файлах:
   - `MainForm.cs`
   - `LoginForm.cs`
   - `DatabaseManager.cs`

2. Убедитесь, что SQL Server запущен:
   ```bash
   # Проверьте службы Windows
   services.msc
   # Найдите "SQL Server (SQLEXPRESS)" и убедитесь, что она запущена
   ```

3. Проверьте учетные данные:
   ```sql
   -- Попробуйте подключиться через SQL Server Management Studio
   Server: 192.168.9.203\SQLEXPRESS
   Authentication: SQL Server Authentication
   Login: student1
   Password: 123456
   ```

4. Проверьте настройки SQL Server:
   ```sql
   -- Включите TCP/IP протокол
   -- Включите смешанную аутентификацию
   ```

#### Ошибка: "Login failed for user"
**Причины:**
- Неверный логин или пароль
- Пользователь не существует
- Пользователь заблокирован

**Решения:**
1. Проверьте учетные данные
2. Создайте пользователя в SQL Server:
   ```sql
   CREATE LOGIN student1 WITH PASSWORD = '123456';
   USE [Проект Вакула, Белов, Сухинин];
   CREATE USER student1 FOR LOGIN student1;
   ALTER ROLE db_owner ADD MEMBER student1;
   ```

### Проблемы с авторизацией

#### Ошибка: "Неверный логин или пароль"
**Для администратора:**
- Убедитесь, что вводите `admin` и `123`
- Проверьте регистр букв

**Для пользователя:**
- Убедитесь, что вводите имя (не email)
- Попробуйте другое имя

#### Ошибка: "Доступ запрещен"
**Причины:**
- Попытка доступа к административным функциям
- Неправильная роль пользователя

**Решения:**
- Войдите как администратор для полного доступа
- Используйте только доступные функции для вашей роли

### Проблемы с интерфейсом

#### Ошибка: "Object reference not set to an instance"
**Причины:**
- Неинициализированные компоненты
- Проблемы с nullable reference types

**Решения:**
1. Проверьте инициализацию компонентов:
   ```csharp
   private TextBox txtLogin = null!;
   ```

2. Добавьте проверки на null:
   ```csharp
   if (txtLogin != null)
   {
       // Работа с компонентом
   }
   ```

#### Ошибка: "Form already disposed"
**Причины:**
- Попытка использования формы после закрытия
- Неправильное управление жизненным циклом формы

**Решения:**
1. Используйте using для автоматического освобождения:
   ```csharp
   using (var form = new MovieForm())
   {
       if (form.ShowDialog() == DialogResult.OK)
       {
           // Обработка результата
       }
   }
   ```

2. Проверяйте состояние формы:
   ```csharp
   if (!form.IsDisposed)
   {
       // Работа с формой
   }
   ```

### Проблемы с данными

#### Ошибка: "Cannot insert duplicate key"
**Причины:**
- Попытка вставки дублирующихся данных
- Нарушение уникальных ограничений

**Решения:**
1. Проверьте уникальные поля:
   ```sql
   -- Email должен быть уникальным
   SELECT * FROM [User] WHERE email = 'duplicate@email.com';
   ```

2. Обработайте исключения:
   ```csharp
   try
   {
       await dapperRepo.AddUserAsync(name, email, phone, roleId);
   }
   catch (SqlException ex) when (ex.Number == 2627)
   {
       MessageBox.Show("Пользователь с таким email уже существует");
   }
   ```

#### Ошибка: "Foreign key constraint"
**Причины:**
- Попытка удаления записи, на которую ссылаются другие записи
- Нарушение целостности данных

**Решения:**
1. Проверьте связанные записи:
   ```sql
   -- Проверьте, есть ли бронирования у пользователя
   SELECT * FROM Booking WHERE user_id = @UserId;
   ```

2. Используйте каскадное удаление:
   ```sql
   ALTER TABLE Booking
   ADD CONSTRAINT FK_Booking_User
   FOREIGN KEY (user_id) REFERENCES [User](user_id)
   ON DELETE CASCADE;
   ```

### Проблемы с производительностью

#### Медленная загрузка данных
**Причины:**
- Отсутствие индексов
- Неэффективные запросы
- Большой объем данных

**Решения:**
1. Добавьте индексы:
   ```sql
   CREATE INDEX IX_User_Email ON [User](email);
   CREATE INDEX IX_Screening_StartTime ON Screening(start_time);
   ```

2. Оптимизируйте запросы:
   ```sql
   -- Используйте WHERE для фильтрации
   SELECT * FROM Movie WHERE genre = 'Action';
   
   -- Используйте LIMIT для ограничения результатов
   SELECT TOP 100 * FROM Booking ORDER BY booking_date DESC;
   ```

3. Используйте пагинацию:
   ```csharp
   public async Task<IEnumerable<Movie>> GetMoviesPagedAsync(int page, int pageSize)
   {
       const string sql = @"
           SELECT * FROM Movie 
           ORDER BY title 
           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
       
       using var connection = GetConnection();
       return await connection.QueryAsync<Movie>(sql, new { 
           Offset = (page - 1) * pageSize, 
           PageSize = pageSize 
       });
   }
   ```

---

## Разработка и расширение

### Добавление новых функций

#### 1. Добавление новой роли

**Шаг 1: Обновление базы данных**
```sql
-- Добавьте новую роль
INSERT INTO Roles (role_name, description) VALUES 
('Manager', 'Менеджер - ограниченный доступ к управлению');

-- Обновите пользователей
UPDATE [User] SET role_id = 3 WHERE name = 'ManagerName';
```

**Шаг 2: Обновление модели**
```csharp
// В User.cs добавьте новое свойство
public bool IsManager => Role?.RoleName == "Manager";
```

**Шаг 3: Обновление логики авторизации**
```csharp
// В MainForm.cs добавьте проверку роли
private bool IsCurrentUserManager()
{
    return currentUserRole == "Manager";
}

// Обновите права доступа
private bool CheckManagerAccess()
{
    if (!IsCurrentUserManager() && !IsCurrentUserAdmin())
    {
        MessageBox.Show("Доступ запрещен. Требуются права менеджера или администратора.");
        return false;
    }
    return true;
}
```

#### 2. Добавление новых полей в модель

**Шаг 1: Обновление базы данных**
```sql
-- Добавьте новое поле
ALTER TABLE Movie ADD director NVARCHAR(100);
ALTER TABLE Movie ADD country NVARCHAR(50);
```

**Шаг 2: Обновление модели**
```csharp
// В Movie.cs добавьте новые свойства
public string? Director { get; set; }
public string? Country { get; set; }
```

**Шаг 3: Обновление репозитория**
```csharp
// В DapperRepository.cs обновите SQL запросы
const string sql = @"
    SELECT movie_id as MovieId, title as Title, description as Description, 
           duration as Duration, genre as Genre, release_date as ReleaseDate, 
           rating as Rating, director as Director, country as Country
    FROM Movie ORDER BY title";
```

**Шаг 4: Обновление форм**
```csharp
// В MovieForm.cs добавьте новые поля
private TextBox txtDirector = new TextBox();
private TextBox txtCountry = new TextBox();

// Обновите метод сохранения
private void BtnSave_Click(object? sender, EventArgs e)
{
    // ... существующий код ...
    
    if (movieId == 0)
    {
        dbManager?.AddMovie(title, description, duration, genre, releaseDate, rating, director, country);
    }
    else
    {
        dbManager?.UpdateMovie(movieId, title, description, duration, genre, releaseDate, rating, director, country);
    }
}
```

#### 3. Добавление новых отчетов

**Шаг 1: Создание формы отчета**
```csharp
public partial class ReportForm : Form
{
    private DataGridView dataGridReport = new DataGridView();
    private Button btnGenerateReport = new Button();
    private ComboBox cmbReportType = new ComboBox();
    
    public ReportForm()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        this.Text = "Отчеты";
        this.Size = new Size(800, 600);
        
        // Настройка компонентов
        cmbReportType.Items.AddRange(new string[] { 
            "Отчет по фильмам", 
            "Отчет по бронированиям", 
            "Отчет по пользователям" 
        });
        
        btnGenerateReport.Text = "Сгенерировать отчет";
        btnGenerateReport.Click += BtnGenerateReport_Click;
        
        // Добавление компонентов на форму
        this.Controls.AddRange(new Control[] { 
            cmbReportType, btnGenerateReport, dataGridReport 
        });
    }
    
    private void BtnGenerateReport_Click(object? sender, EventArgs e)
    {
        string reportType = cmbReportType.SelectedItem?.ToString();
        
        switch (reportType)
        {
            case "Отчет по фильмам":
                GenerateMoviesReport();
                break;
            case "Отчет по бронированиям":
                GenerateBookingsReport();
                break;
            case "Отчет по пользователям":
                GenerateUsersReport();
                break;
        }
    }
    
    private void GenerateMoviesReport()
    {
        // Логика генерации отчета по фильмам
    }
    
    private void GenerateBookingsReport()
    {
        // Логика генерации отчета по бронированиям
    }
    
    private void GenerateUsersReport()
    {
        // Логика генерации отчета по пользователям
    }
}
```

**Шаг 2: Добавление кнопки отчета в главную форму**
```csharp
// В MainForm.cs добавьте кнопку отчета
private Button btnReports = new Button();

private void InitializeComponent()
{
    // ... существующий код ...
    
    if (IsCurrentUserAdmin())
    {
        btnReports.Text = "Отчеты";
        btnReports.Location = new Point(400, 420);
        btnReports.Size = new Size(120, 30);
        btnReports.Click += BtnReports_Click;
        
        tabMovies.Controls.Add(btnReports);
    }
}

private void BtnReports_Click(object? sender, EventArgs e)
{
    using (var reportForm = new ReportForm())
    {
        reportForm.ShowDialog();
    }
}
```

### Оптимизация производительности

#### 1. Кэширование данных
```csharp
public class CacheManager
{
    private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
    private static readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
    
    public static T? Get<T>(string key) where T : class
    {
        if (_cache.TryGetValue(key, out var value))
        {
            return value as T;
        }
        return null;
    }
    
    public static void Set<T>(string key, T value) where T : class
    {
        _cache[key] = value;
    }
    
    public static void Clear()
    {
        _cache.Clear();
    }
}

// Использование в репозитории
public async Task<IEnumerable<Movie>> GetMoviesAsync()
{
    const string cacheKey = "movies";
    var cachedMovies = CacheManager.Get<IEnumerable<Movie>>(cacheKey);
    
    if (cachedMovies != null)
    {
        return cachedMovies;
    }
    
    const string sql = "SELECT * FROM Movie ORDER BY title";
    using var connection = GetConnection();
    var movies = await connection.QueryAsync<Movie>(sql);
    
    CacheManager.Set(cacheKey, movies);
    return movies;
}
```

#### 2. Асинхронная загрузка данных
```csharp
public async Task LoadDataAsync()
{
    try
    {
        // Показываем индикатор загрузки
        ShowLoadingIndicator();
        
        // Загружаем данные параллельно
        var moviesTask = dapperRepo.GetMoviesAsync();
        var screeningsTask = dapperRepo.GetScreeningsAsync();
        var usersTask = dapperRepo.GetUsersAsync();
        
        await Task.WhenAll(moviesTask, screeningsTask, usersTask);
        
        // Обновляем UI
        dataGridMovies.DataSource = await moviesTask;
        dataGridScreenings.DataSource = await screeningsTask;
        dataGridUsers.DataSource = await usersTask;
        
        HideLoadingIndicator();
    }
    catch (Exception ex)
    {
        HideLoadingIndicator();
        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
    }
}
```

#### 3. Валидация данных
```csharp
public class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool IsValidPhone(string phone)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phone, 
            @"^\+?[1-9]\d{1,14}$");
    }
    
    public static bool IsValidDate(DateTime date)
    {
        return date >= DateTime.MinValue && date <= DateTime.MaxValue;
    }
}

// Использование в формах
private void BtnSave_Click(object? sender, EventArgs e)
{
    if (!ValidationHelper.IsValidEmail(txtEmail.Text))
    {
        MessageBox.Show("Неверный формат email");
        return;
    }
    
    if (!ValidationHelper.IsValidPhone(txtPhone.Text))
    {
        MessageBox.Show("Неверный формат телефона");
        return;
    }
    
    // Сохранение данных
}
```

### Тестирование

#### 1. Unit тесты
```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CinemaBookingApp.Data;
using CinemaBookingApp.Models;

[TestClass]
public class DapperRepositoryTests
{
    private DapperRepository _repository;
    private string _connectionString = "TestConnectionString";
    
    [TestInitialize]
    public void Setup()
    {
        _repository = new DapperRepository(_connectionString);
    }
    
    [TestMethod]
    public async Task GetUsersAsync_ShouldReturnUsers()
    {
        // Arrange
        var expectedCount = 5;
        
        // Act
        var users = await _repository.GetUsersAsync();
        
        // Assert
        Assert.IsNotNull(users);
        Assert.AreEqual(expectedCount, users.Count());
    }
    
    [TestMethod]
    public async Task AddUserAsync_ShouldAddUser()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";
        var phone = "+1234567890";
        var roleId = 2;
        
        // Act
        await _repository.AddUserAsync(name, email, phone, roleId);
        
        // Assert
        var user = await _repository.GetUserByEmailAsync(email);
        Assert.IsNotNull(user);
        Assert.AreEqual(name, user.Name);
        Assert.AreEqual(email, user.Email);
    }
}
```

#### 2. Integration тесты
```csharp
[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public async Task FullBookingWorkflow_ShouldWork()
    {
        // Arrange
        var repository = new DapperRepository(connectionString);
        var user = await repository.AddUserAsync("Test User", "test@example.com", "+1234567890", 2);
        var movie = await repository.AddMovieAsync("Test Movie", "Test Description", 120, "Action", DateTime.Now, 8.5m);
        
        // Act
        var booking = await repository.AddBookingAsync(user.UserId, screening.ScreeningId, 100.00m);
        
        // Assert
        Assert.IsNotNull(booking);
        Assert.AreEqual(user.UserId, booking.UserId);
        Assert.AreEqual(100.00m, booking.TotalPrice);
    }
}
```

### Развертывание

#### 1. Создание установочного пакета
```xml
<!-- В CinemaBookingApp.csproj -->
<PropertyGroup>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

#### 2. Команды для сборки
```bash
# Создание релизной сборки
dotnet build -c Release

# Создание самораспаковывающегося приложения
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Создание установочного пакета
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

#### 3. Конфигурация для продакшена
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD_SERVER;Database=PROD_DB;User Id=PROD_USER;Password=PROD_PASS;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AppSettings": {
    "AdminLogin": "admin",
    "AdminPassword": "secure_password",
    "CacheExpiryMinutes": 5,
    "MaxBookingDays": 30
  }
}
```

---

## 🆕 Расширенные функции безопасности

### 🔒 Личные бронирования
Система обеспечивает полную изоляцию данных пользователей:

#### Для обычных пользователей:
- **Фильтрация бронирований** по имени пользователя
- **Просмотр только своих бронирований** в таблице
- **Кнопки "Отменить мою бронь"** и "Мои билеты"
- **Защита от просмотра** чужих данных

#### Для администраторов:
- **Просмотр всех бронирований** всех пользователей
- **Полный контроль** над системой
- **Административные функции** без ограничений

### 🎫 Ограниченная форма создания бронирования
Форма создания бронирования адаптируется под роль пользователя:

#### Для обычных пользователей:
- **Заголовок:** "Новое бронирование - [Имя пользователя]"
- **Поле пользователя:** "Пользователь (ваше имя):" - заблокировано
- **ComboBox:** Только свое имя, недоступно для изменения
- **Создание бронирований только на себя**

#### Для администраторов:
- **Заголовок:** "Новое бронирование"
- **Поле пользователя:** "Пользователь:" - доступно для изменения
- **ComboBox:** Все пользователи, доступно для изменения
- **Создание бронирований от имени любого пользователя**

### 🎟️ Управление отдельными билетами
Новая система позволяет отменять отдельные билеты:

#### Возможности:
- **Выбор отдельных билетов** для отмены (1, 2 или все)
- **Автоматический пересчет** суммы бронирования
- **Автоматическая отмена бронирования** если все билеты отменены
- **Проверка доступа** к билетам других пользователей

#### Интерфейс:
- **Колонка с чекбоксами** для выбора билетов
- **Чекбокс "Выбрать все билеты"** для массового выбора
- **Кнопка "Отменить выбранные билеты"** с подтверждением
- **Автоматическое обновление** данных после отмены

### 🔍 Поиск и фильтрация
Мощная система поиска и фильтрации данных:

#### Возможности поиска:
- **Поиск по всем столбцам** таблиц одновременно
- **Фильтрация по операторам:** =, !=, содержит, начинается с, заканчивается на, >, <, >=, <=
- **Статусная строка** с количеством найденных записей
- **Очистка фильтров** одной кнопкой

#### Поддерживаемые типы данных:
- **Текстовые поля:** поиск по подстроке, точное совпадение
- **Числовые поля:** сравнение по величине
- **Даты:** фильтрация по временным интервалам
- **Статусы:** точное совпадение значений

### 🛡️ Система безопасности

#### Принципы безопасности:
- **Изоляция данных:** каждый пользователь видит только свои данные
- **Проверка доступа:** валидация прав на каждом уровне
- **Автоматическая блокировка:** недоступные функции скрыты
- **Транзакции:** обеспечение целостности данных

#### Технические меры:
- **Проверка роли пользователя** при загрузке данных
- **Фильтрация запросов** по ID пользователя
- **Валидация доступа** при открытии форм
- **Автоматическое закрытие** при ошибках доступа

---

## Заключение

CinemaBookingApp - это полнофункциональная система бронирования билетов в кинотеатре, разработанная с использованием современных технологий и лучших практик разработки.

### Ключевые особенности:
- ✅ **Многоуровневая архитектура** с четким разделением ответственности
- ✅ **Система ролей** с разными уровнями доступа
- ✅ **Двойной слой доступа к данным** (ADO.NET + Dapper)
- ✅ **Адаптивный интерфейс** в зависимости от роли пользователя
- ✅ **Валидация данных** и обработка ошибок
- ✅ **Асинхронное программирование** для улучшения производительности
- ✅ **🆕 Расширенная безопасность** с личными бронированиями
- ✅ **🆕 Гибкое управление билетами** с отменой отдельных билетов
- ✅ **🆕 Ограниченный доступ** в формах создания бронирований
- ✅ **🆕 Универсальный поиск** по всем столбцам с фильтрацией

### Технологии:
- **.NET 9.0** - современная платформа разработки
- **Windows Forms** - нативный UI фреймворк
- **SQL Server** - надежная система управления базами данных
- **Dapper** - быстрый микро-ORM
- **ADO.NET** - низкоуровневый доступ к данным

### Возможности расширения:
- Добавление новых ролей и прав доступа
- Интеграция с внешними системами
- Добавление новых типов отчетов
- Мобильная версия приложения
- Веб-интерфейс


