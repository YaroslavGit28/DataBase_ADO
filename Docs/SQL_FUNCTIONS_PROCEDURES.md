# SQL Функции и Процедуры для CinemaBookingApp

## Обзор

Этот файл содержит простые SQL функции и хранимые процедуры для системы бронирования билетов в кинотеатре.

---

## 🔧 **Функция 1: Расчет стоимости билета**

### Описание
Функция рассчитывает финальную стоимость билета на основе базовой цены и типа зала.

```sql
-- =============================================
-- Функция: Расчет стоимости билета
-- Назначение: Рассчитывает финальную стоимость билета на основе базовой цены и типа зала
-- Параметры: @BasePrice - базовая цена билета, @HallTypeId - ID типа зала
-- Возвращает: DECIMAL(10,2) - финальная стоимость билета
-- =============================================
CREATE FUNCTION dbo.CalculateTicketPrice(
    @BasePrice DECIMAL(10,2),    -- Входной параметр: базовая цена билета (например, 300.00)
    @HallTypeId INT              -- Входной параметр: идентификатор типа зала (1=Обычный, 2=3D, 3=VIP)
)
RETURNS DECIMAL(10,2)            -- Возвращаемый тип: десятичное число с 2 знаками после запятой
AS
BEGIN
    -- Объявляем локальные переменные для хранения промежуточных значений
    DECLARE @Multiplier DECIMAL(3,2);  -- Переменная для хранения множителя цены (например, 1.5 для 3D)
    DECLARE @FinalPrice DECIMAL(10,2); -- Переменная для хранения финальной рассчитанной цены
    
    -- Получаем множитель цены для указанного типа зала из таблицы Hall_Type
    SELECT @Multiplier = price_per_seat  -- Извлекаем значение поля price_per_seat (множитель цены)
    FROM Hall_Type                       -- Из таблицы типов залов
    WHERE hall_type_id = @HallTypeId;    -- Где ID типа зала совпадает с переданным параметром
    
    -- Проверяем, был ли найден тип зала (если SELECT не вернул результат, @Multiplier будет NULL)
    IF @Multiplier IS NULL
        SET @Multiplier = 1.0;           -- Если тип зала не найден, устанавливаем множитель по умолчанию = 1.0
    
    -- Рассчитываем финальную цену билета: базовая цена умножается на множитель типа зала
    SET @FinalPrice = @BasePrice * @Multiplier;  -- Например: 300.00 * 1.5 = 450.00
    
    -- Возвращаем рассчитанную финальную цену билета
    RETURN @FinalPrice;
END;
GO
```

### Использование функции:
```sql
-- Пример использования функции
SELECT 
    m.title AS movie_title,
    ht.type_name AS hall_type,
    dbo.CalculateTicketPrice(300.00, ht.hall_type_id) AS ticket_price
FROM Movie m
CROSS JOIN Hall_Type ht
WHERE m.movie_id = 1;
```

**Результат:**
```
movie_title | hall_type | ticket_price
------------|-----------|-------------
Аватар      | Обычный   | 300.00
Аватар      | 3D        | 450.00
Аватар      | VIP       | 600.00
```

---

## 📊 **Процедура 1: Создание нового бронирования**

### Описание
Хранимая процедура для создания нового бронирования с автоматическим расчетом стоимости и созданием билетов.

```sql
-- =============================================
-- Процедура: Создание бронирования с билетами
-- Назначение: Создает новое бронирование с автоматическим расчетом стоимости и созданием билетов
-- Параметры: @UserId - ID пользователя, @ScreeningId - ID сеанса, @SeatNumbers - список мест через запятую
--           @BasePrice - базовая цена, @BookingId - выходной параметр с ID созданного бронирования
-- =============================================
CREATE PROCEDURE dbo.CreateBookingWithTickets
    @UserId INT,                           -- Входной параметр: идентификатор пользователя
    @ScreeningId INT,                      -- Входной параметр: идентификатор сеанса
    @SeatNumbers NVARCHAR(MAX),            -- Входной параметр: список мест через запятую (например: "A1,A2,A3")
    @BasePrice DECIMAL(10,2),              -- Входной параметр: базовая цена билета
    @BookingId INT OUTPUT                  -- Выходной параметр: ID созданного бронирования (передается по ссылке)
AS
BEGIN
    SET NOCOUNT ON;                        -- Отключаем вывод количества обработанных строк для оптимизации
    
    -- Объявляем локальные переменные для хранения промежуточных значений
    DECLARE @TotalAmount DECIMAL(10,2) = 0;    -- Переменная для общей суммы бронирования (инициализируем нулем)
    DECLARE @TicketPrice DECIMAL(10,2);        -- Переменная для цены одного билета
    DECLARE @HallTypeId INT;                   -- Переменная для ID типа зала
    DECLARE @SeatNumber NVARCHAR(10);          -- Переменная для текущего номера места в цикле
    DECLARE @SeatList TABLE (seat_number NVARCHAR(10)); -- Временная таблица для хранения списка мест
    
    BEGIN TRY                                -- Начинаем блок обработки ошибок
        BEGIN TRANSACTION;                   -- Начинаем транзакцию для обеспечения целостности данных
        
        -- Получаем тип зала для указанного сеанса из базы данных
        SELECT @HallTypeId = h.hall_type_id  -- Извлекаем ID типа зала
        FROM Screening s                     -- Из таблицы сеансов (алиас s)
        INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id  -- Соединяем с таблицей залов по hall_id
        WHERE s.screening_id = @ScreeningId; -- Где ID сеанса совпадает с переданным параметром
        
        -- Проверяем, был ли найден сеанс (если SELECT не вернул результат, @HallTypeId будет NULL)
        IF @HallTypeId IS NULL
        BEGIN
            RAISERROR('Сеанс не найден', 16, 1);  -- Выбрасываем ошибку с сообщением и кодом ошибки
            RETURN;                                -- Выходим из процедуры
        END
        
        -- Рассчитываем цену билета с помощью нашей функции CalculateTicketPrice
        SET @TicketPrice = dbo.CalculateTicketPrice(@BasePrice, @HallTypeId);  -- Вызываем функцию расчета цены
        
        -- Разбиваем строку с номерами мест на отдельные места и сохраняем в временную таблицу
        INSERT INTO @SeatList (seat_number)  -- Вставляем данные в временную таблицу @SeatList
        SELECT LTRIM(RTRIM(value))           -- Убираем пробелы слева и справа от каждого значения
        FROM STRING_SPLIT(@SeatNumbers, ','); -- Разбиваем строку по запятой на отдельные значения
        
        -- Проверяем доступность всех указанных мест на сеансе
        IF EXISTS (                          -- Проверяем, существует ли хотя бы одно занятое место
            SELECT 1                         -- Выбираем константу 1 (для EXISTS важен только факт существования)
            FROM @SeatList sl                -- Из нашей временной таблицы мест (алиас sl)
            INNER JOIN Ticket t ON sl.seat_number = t.seat_number  -- Соединяем с таблицей билетов по номеру места
            INNER JOIN Screening s ON t.screening_id = s.screening_id  -- Соединяем с таблицей сеансов
            INNER JOIN Booking b ON t.booking_id = b.booking_id      -- Соединяем с таблицей бронирований
            WHERE s.screening_id = @ScreeningId  -- Где ID сеанса совпадает с нашим сеансом
                AND b.status = 'COMPLETED'        -- И статус бронирования "завершено" (место занято)
        )
        BEGIN
            RAISERROR('Одно или несколько мест уже заняты', 16, 1);  -- Выбрасываем ошибку о занятых местах
            RETURN;                                                      -- Выходим из процедуры
        END
        
        -- Рассчитываем общую сумму бронирования
        SET @TotalAmount = @TicketPrice * (SELECT COUNT(*) FROM @SeatList);  -- Цена билета умножается на количество мест
        
        -- Создаем новое бронирование в таблице Booking
        INSERT INTO Booking (user_id, order_date, total_amount, status)  -- Вставляем новую запись в таблицу бронирований
        VALUES (@UserId, GETDATE(), @TotalAmount, 'PENDING');             -- Значения: ID пользователя, текущая дата, общая сумма, статус "ожидает"
        
        SET @BookingId = SCOPE_IDENTITY();  -- Получаем ID только что созданной записи (автоинкремент)
        
        -- Создаем билеты для каждого места с помощью курсора
        DECLARE seat_cursor CURSOR FOR       -- Объявляем курсор для перебора мест
        SELECT seat_number FROM @SeatList;   -- Выбираем все номера мест из временной таблицы
        
        OPEN seat_cursor;                    -- Открываем курсор для работы
        FETCH NEXT FROM seat_cursor INTO @SeatNumber;  -- Читаем первое значение в переменную @SeatNumber
        
        WHILE @@FETCH_STATUS = 0            -- Пока курсор не достиг конца данных (@@FETCH_STATUS = 0)
        BEGIN
            -- Создаем билет для текущего места
            INSERT INTO Ticket (booking_id, seat_number, price, screening_id)  -- Вставляем новый билет
            VALUES (@BookingId, @SeatNumber, @TicketPrice, @ScreeningId);    -- Значения: ID бронирования, номер места, цена, ID сеанса
            
            FETCH NEXT FROM seat_cursor INTO @SeatNumber;  -- Читаем следующее значение из курсора
        END
        
        CLOSE seat_cursor;                   -- Закрываем курсор
        DEALLOCATE seat_cursor;              -- Освобождаем память, занятую курсором
        
        COMMIT TRANSACTION;                  -- Подтверждаем транзакцию (сохраняем все изменения)
        
        -- Возвращаем информацию о созданном бронировании пользователю
        SELECT 
            @BookingId AS booking_id,       -- ID созданного бронирования
            @TotalAmount AS total_amount,   -- Общая сумма бронирования
            @TicketPrice AS ticket_price,    -- Цена одного билета
            (SELECT COUNT(*) FROM @SeatList) AS tickets_count;  -- Количество билетов
            
    END TRY                                  -- Конец блока обработки ошибок
    BEGIN CATCH                              -- Блок обработки ошибок
        IF @@TRANCOUNT > 0                   -- Если есть активная транзакция
            ROLLBACK TRANSACTION;            -- Откатываем все изменения (возвращаем базу в исходное состояние)
            
        -- Получаем информацию об ошибке для передачи пользователю
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();  -- Сообщение об ошибке
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();         -- Уровень серьезности ошибки
        DECLARE @ErrorState INT = ERROR_STATE();               -- Состояние ошибки
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); -- Перебрасываем ошибку наверх
    END CATCH                                -- Конец блока обработки ошибок
END;
GO
```

### Использование процедуры:
```sql
-- Пример использования процедуры
DECLARE @NewBookingId INT;
DECLARE @UserId INT = 1;
DECLARE @ScreeningId INT = 1;
DECLARE @SeatNumbers NVARCHAR(MAX) = 'A1,A2,A3';
DECLARE @BasePrice DECIMAL(10,2) = 300.00;

EXEC dbo.CreateBookingWithTickets 
    @UserId = @UserId,
    @ScreeningId = @ScreeningId,
    @SeatNumbers = @SeatNumbers,
    @BasePrice = @BasePrice,
    @BookingId = @NewBookingId OUTPUT;

PRINT 'Создано бронирование с ID: ' + CAST(@NewBookingId AS VARCHAR(10));
```

**Результат:**
```
booking_id | total_amount | ticket_price | tickets_count
-----------|--------------|--------------|--------------
123        | 900.00       | 300.00       | 3
```

---

## 🔍 **Дополнительные простые функции**

### Функция 2: Проверка доступности места

```sql
-- =============================================
-- Функция: Проверка доступности места
-- Назначение: Проверяет, свободно ли указанное место на конкретном сеансе
-- Параметры: @ScreeningId - ID сеанса, @SeatNumber - номер места
-- Возвращает: BIT - 1 если место свободно, 0 если занято
-- =============================================
CREATE FUNCTION dbo.IsSeatAvailable(
    @ScreeningId INT,                    -- Входной параметр: идентификатор сеанса
    @SeatNumber NVARCHAR(10)             -- Входной параметр: номер места (например, "A15")
)
RETURNS BIT                              -- Возвращаемый тип: битовое значение (0 или 1)
AS
BEGIN
    DECLARE @IsAvailable BIT = 1;       -- Объявляем переменную и инициализируем значением 1 (место свободно)
    
    -- Проверяем, существует ли билет на это место с завершенным бронированием
    IF EXISTS (                          -- Проверяем условие существования записи
        SELECT 1                        -- Выбираем константу 1 (для EXISTS важен только факт существования)
        FROM Ticket t                   -- Из таблицы билетов (алиас t)
        INNER JOIN Booking b ON t.booking_id = b.booking_id  -- Соединяем с таблицей бронирований по ID бронирования
        WHERE t.screening_id = @ScreeningId    -- Где ID сеанса совпадает с переданным параметром
            AND t.seat_number = @SeatNumber   -- И номер места совпадает с переданным параметром
            AND b.status = 'COMPLETED'         -- И статус бронирования "завершено" (место занято)
    )
        SET @IsAvailable = 0;           -- Если место занято, устанавливаем значение 0
    
    RETURN @IsAvailable;                -- Возвращаем результат проверки (1 - свободно, 0 - занято)
END;
GO
```

### Использование:
```sql
-- Проверка доступности места
SELECT 
    s.screening_id,
    m.title,
    'A10' AS seat_number,
    dbo.IsSeatAvailable(s.screening_id, 'A10') AS is_available
FROM Screening s
INNER JOIN Movie m ON s.movie_id = m.movie_id
WHERE s.screening_id = 1;
```

### Функция 3: Получение информации о пользователе

```sql
-- =============================================
-- Функция: Получение информации о пользователе
-- Назначение: Возвращает полную информацию о пользователе с статистикой бронирований
-- Параметры: @UserId - ID пользователя
-- Возвращает: TABLE - таблица с информацией о пользователе и статистикой
-- =============================================
CREATE FUNCTION dbo.GetUserInfo(@UserId INT)  -- Входной параметр: идентификатор пользователя
RETURNS TABLE                                  -- Возвращаемый тип: таблица (table-valued function)
AS
RETURN
(
    SELECT 
        u.user_id,                            -- ID пользователя
        u.name,                               -- Имя пользователя
        u.email,                              -- Email пользователя
        u.phone,                              -- Телефон пользователя
        r.role_name,                          -- Название роли пользователя
        COUNT(b.booking_id) AS total_bookings, -- Общее количество бронирований пользователя
        SUM(CASE WHEN b.status = 'COMPLETED' THEN b.total_amount ELSE 0 END) AS total_spent  -- Общая потраченная сумма (только завершенные бронирования)
    FROM [User] u                             -- Из таблицы пользователей (алиас u)
    INNER JOIN Roles r ON u.role_id = r.role_id  -- Соединяем с таблицей ролей по ID роли
    LEFT JOIN Booking b ON u.user_id = b.user_id  -- Левый джойн с таблицей бронирований (может быть NULL)
    WHERE u.user_id = @UserId                 -- Где ID пользователя совпадает с переданным параметром
    GROUP BY u.user_id, u.name, u.email, u.phone, r.role_name  -- Группируем по полям пользователя для агрегации
);
GO
```

### Использование:
```sql
-- Получение информации о пользователе
SELECT * FROM dbo.GetUserInfo(1);
```

---

## 📋 **Примеры использования всех функций**

### Создание бронирования с проверками:
```sql
-- =============================================
-- Полный пример использования всех функций
-- Демонстрирует создание бронирования с проверками и получение статистики
-- =============================================

-- Объявляем переменные для хранения параметров бронирования
DECLARE @UserId INT = 1;                    -- ID пользователя (например, 1)
DECLARE @ScreeningId INT = 1;               -- ID сеанса (например, 1)
DECLARE @SeatNumber NVARCHAR(10) = 'B15';   -- Номер места (например, "B15")
DECLARE @BasePrice DECIMAL(10,2) = 300.00;  -- Базовая цена билета (например, 300.00 рублей)
DECLARE @NewBookingId INT;                  -- Переменная для хранения ID созданного бронирования

-- Проверяем доступность места с помощью функции IsSeatAvailable
IF dbo.IsSeatAvailable(@ScreeningId, @SeatNumber) = 1  -- Если функция вернула 1 (место свободно)
BEGIN
    -- Создаем бронирование с помощью процедуры CreateBookingWithTickets
    EXEC dbo.CreateBookingWithTickets 
        @UserId = @UserId,                   -- Передаем ID пользователя
        @ScreeningId = @ScreeningId,         -- Передаем ID сеанса
        @SeatNumbers = @SeatNumber,         -- Передаем номер места
        @BasePrice = @BasePrice,            -- Передаем базовую цену
        @BookingId = @NewBookingId OUTPUT;  -- Получаем ID созданного бронирования
    
    -- Показываем обновленную информацию о пользователе с помощью функции GetUserInfo
    SELECT * FROM dbo.GetUserInfo(@UserId); -- Выводим статистику пользователя
    
    -- Выводим сообщение об успешном создании бронирования
    PRINT 'Бронирование успешно создано!';  -- Выводим сообщение в консоль
END
ELSE                                        -- Если место занято (функция вернула 0)
BEGIN
    -- Выводим сообщение о том, что место занято
    PRINT 'Место ' + @SeatNumber + ' уже занято!';  -- Формируем сообщение с номером места
END
```

---

## 🎯 **Заключение**

Созданные функции и процедуры обеспечивают:

### **🔧 Функция CalculateTicketPrice:**
- Автоматический расчет стоимости билета
- Учет типа зала и базовой цены
- Обработка ошибок (если тип зала не найден)

### **📊 Процедура CreateBookingWithTickets:**
- Создание бронирования с автоматическим расчетом
- Проверка доступности мест
- Создание билетов для каждого места
- Транзакционная безопасность
- Обработка ошибок с откатом изменений

### **💡 Преимущества:**
- **Переиспользование кода** - функции можно вызывать из разных мест
- **Безопасность** - транзакции и проверки
- **Производительность** - оптимизированные запросы
- **Читаемость** - понятная структура и комментарии

**Функции готовы к использованию в системе бронирования!** 🚀

