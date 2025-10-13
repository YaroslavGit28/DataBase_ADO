# Cinema Booking Application - Подключение к БД

## Технологии подключения к базе данных

### 1. **Microsoft.Data.SqlClient** (Базовый провайдер)
```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.1.1" />
```

**Что это:**
- Низкоуровневый провайдер для работы с SQL Server
- Основа для всех других технологий доступа к данным
- Прямая работа с ADO.NET

**Использование:**
```csharp
using var connection = new SqlConnection(connectionString);
connection.Open();
var command = new SqlCommand(query, connection);
var result = command.ExecuteScalar();
```

### 2. **Dapper** (Micro-ORM)
```xml
<PackageReference Include="Dapper" Version="2.1.66" />
```

**Что это:**
- Легковесный micro-ORM
- Автоматическое маппирование объектов
- Поддержка async/await
- Минимальный overhead

**Преимущества Dapper:**
- ✅ **Простота** - минимум кода
- ✅ **Производительность** - быстрее Entity Framework
- ✅ **Гибкость** - полный контроль над SQL
- ✅ **Автомаппинг** - автоматическое преобразование в объекты
- ✅ **Async поддержка** - современные асинхронные операции

**Пример использования:**
```csharp
// Старый способ (ADO.NET)
var dataTable = new DataTable();
var adapter = new SqlDataAdapter(query, connection);
adapter.Fill(dataTable);

// Новый способ (Dapper)
var users = await connection.QueryAsync<User>("SELECT * FROM [User]");
```

## Структура подключения

### Строка подключения:
```csharp
"Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;"
```

### Компоненты:
- **Server** - адрес сервера БД
- **Database** - название базы данных
- **User Id** - имя пользователя
- **Password** - пароль
- **TrustServerCertificate** - доверие сертификату сервера

## Архитектура доступа к данным

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   WinForms UI   │───▶│  Data Layer      │───▶│   SQL Server    │
│                 │    │                  │    │                 │
│ • MainForm      │    │ • DatabaseManager│    │ • Tables        │
│ • UserForm      │    │ • DapperRepository│    │ • Stored Procs  │
│ • BookingForm   │    │                  │    │ • Views         │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## Сравнение подходов

| Аспект | ADO.NET (DatabaseManager) | Dapper (DapperRepository) |
|--------|---------------------------|----------------------------|
| **Код** | Много boilerplate | Минимум кода |
| **Производительность** | Хорошая | Отличная |
| **Типобезопасность** | Низкая | Высокая |
| **Автомаппинг** | Ручной | Автоматический |
| **Async поддержка** | Ограниченная | Полная |
| **Сложность** | Высокая | Низкая |

## Примеры использования

### Получение пользователей (ADO.NET):
```csharp
public DataTable GetUsers()
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    string query = "SELECT user_id, name, email, phone FROM [User] ORDER BY name";
    var adapter = new SqlDataAdapter(query, connection);
    var dataTable = new DataTable();
    adapter.Fill(dataTable);
    return dataTable;
}
```

### Получение пользователей (Dapper):
```csharp
public async Task<IEnumerable<User>> GetUsersAsync()
{
    const string sql = "SELECT user_id as UserId, name as Name, email as Email, phone as Phone FROM [User] ORDER BY name";
    using var connection = GetConnection();
    return await connection.QueryAsync<User>(sql);
}
```

## Рекомендации

1. **Для новых проектов** - используйте Dapper
2. **Для существующих проектов** - постепенно мигрируйте на Dapper
3. **Для сложных запросов** - Dapper + SQL
4. **Для простых CRUD** - Dapper с автомаппингом

## Миграция на Dapper

Проект поддерживает оба подхода одновременно:
- `DatabaseManager` - старый ADO.NET подход
- `DapperRepository` - новый Dapper подход

