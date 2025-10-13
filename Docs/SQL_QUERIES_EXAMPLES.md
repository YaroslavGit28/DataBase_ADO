# SQL Запросы для CinemaBookingApp - Примеры работы с базой данных

## Обзор

Этот файл содержит примеры SQL запросов разной сложности для демонстрации работы базы данных системы бронирования билетов в кинотеатре.

---

## 🟢 **Запрос 1: Простой SELECT (Базовый уровень)**

### Получить список всех фильмов с их жанрами

```sql
SELECT 
    movie_id,
    title,
    genre,
    duration_minutes,
    description
FROM Movie
ORDER BY title;
```

**Результат:** Список всех фильмов, отсортированный по названию.

**Пример вывода:**
```
movie_id | title           | genre      | duration_minutes | description
---------|-----------------|------------|------------------|-------------
1        | Аватар          | Фантастика | 162              | Эпическая фантастика
2        | Топ Ган         | Боевик     | 131              | Военная драма
3        | Бэтмен          | Боевик     | 176              | Супергеройский фильм
```

---

## 🟡 **Запрос 2: JOIN с фильтрацией (Средний уровень)**

### Найти все сеансы на завтра с информацией о фильмах и залах

```sql
SELECT 
    s.screening_id,
    m.title AS movie_title,
    m.genre,
    h.hall_name,
    ht.type_name AS hall_type,
    s.start_time,
    s.end_time,
    h.capacity
FROM Screening s
INNER JOIN Movie m ON s.movie_id = m.movie_id
INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
INNER JOIN Hall_Type ht ON h.hall_type_id = ht.hall_type_id
WHERE CAST(s.start_time AS DATE) = CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)
ORDER BY s.start_time, h.hall_name;
```

**Результат:** Все сеансы на завтра с полной информацией о фильмах и залах.

**Пример вывода:**
```
screening_id | movie_title | genre   | hall_name | hall_type | start_time          | end_time            | capacity
-------------|-------------|---------|-----------|-----------|---------------------|---------------------|----------
5            | Аватар      | Фантастика | Зал 1    | 3D        | 2024-01-15 18:00:00 | 2024-01-15 20:42:00 | 150
6            | Топ Ган     | Боевик  | Зал 2    | Обычный   | 2024-01-15 19:30:00 | 2024-01-15 21:41:00 | 200
```

---

## 🟠 **Запрос 3: Агрегация с GROUP BY (Средний уровень)**

### Статистика бронирований по пользователям за последний месяц

```sql
SELECT 
    u.name AS user_name,
    u.email,
    COUNT(b.booking_id) AS total_bookings,
    SUM(b.total_amount) AS total_spent,
    AVG(b.total_amount) AS avg_booking_amount,
    MAX(b.order_date) AS last_booking_date
FROM [User] u
INNER JOIN Booking b ON u.user_id = b.user_id
WHERE b.order_date >= DATEADD(MONTH, -1, GETDATE())
GROUP BY u.user_id, u.name, u.email
HAVING COUNT(b.booking_id) > 0
ORDER BY total_spent DESC;
```

**Результат:** Статистика активности пользователей за последний месяц.

**Пример вывода:**
```
user_name | email              | total_bookings | total_spent | avg_booking_amount | last_booking_date
----------|--------------------|----------------|-------------|--------------------|--------------------
Иван Петров | ivan@email.com   | 5              | 7500.00     | 1500.00            | 2024-01-10 15:30:00
Мария Сидорова | maria@email.com | 3              | 4500.00     | 1500.00            | 2024-01-08 20:15:00
Алексей Козлов | alex@email.com  | 2              | 2400.00     | 1200.00            | 2024-01-05 18:45:00
```

---

## 🔴 **Запрос 4: Сложный запрос с подзапросами (Высокий уровень)**

### Найти самые популярные места в залах с детальной статистикой

```sql
WITH SeatStatistics AS (
    SELECT 
        s.hall_id,
        h.hall_name,
        t.seat_number,
        COUNT(t.ticket_id) AS booking_count,
        SUM(t.price) AS total_revenue,
        AVG(t.price) AS avg_price
    FROM Ticket t
    INNER JOIN Screening s ON t.screening_id = s.screening_id
    INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
    INNER JOIN Booking b ON t.booking_id = b.booking_id
    WHERE b.status = 'COMPLETED'
    GROUP BY s.hall_id, h.hall_name, t.seat_number
),
HallCapacity AS (
    SELECT 
        hall_id,
        hall_name,
        capacity
    FROM Cinema_Hall
)
SELECT 
    ss.hall_name,
    ss.seat_number,
    ss.booking_count,
    ss.total_revenue,
    ss.avg_price,
    hc.capacity,
    CAST(ss.booking_count AS FLOAT) / hc.capacity * 100 AS utilization_percentage
FROM SeatStatistics ss
INNER JOIN HallCapacity hc ON ss.hall_id = hc.hall_id
WHERE ss.booking_count >= 3  -- Места, забронированные минимум 3 раза
ORDER BY ss.booking_count DESC, ss.total_revenue DESC;
```

**Результат:** Анализ популярности мест в залах с процентом использования.

**Пример вывода:**
```
hall_name | seat_number | booking_count | total_revenue | avg_price | capacity | utilization_percentage
----------|-------------|---------------|---------------|-----------|----------|-----------------------
Зал 1     | A10         | 8             | 2400.00       | 300.00    | 150      | 5.33
Зал 1     | B15         | 7             | 2100.00       | 300.00    | 150      | 4.67
Зал 2     | C20         | 6             | 1800.00       | 300.00    | 200      | 3.00
```

---

## 🟣 **Запрос 5: Очень сложный запрос с оконными функциями (Экспертный уровень)**

### Полный анализ эффективности кинотеатра с трендами и прогнозами

```sql
WITH MonthlyRevenue AS (
    SELECT 
        YEAR(b.order_date) AS year,
        MONTH(b.order_date) AS month,
        DATENAME(MONTH, b.order_date) AS month_name,
        COUNT(b.booking_id) AS total_bookings,
        SUM(b.total_amount) AS total_revenue,
        COUNT(DISTINCT b.user_id) AS unique_customers,
        AVG(b.total_amount) AS avg_booking_value
    FROM Booking b
    WHERE b.status = 'COMPLETED'
        AND b.order_date >= DATEADD(YEAR, -1, GETDATE())
    GROUP BY YEAR(b.order_date), MONTH(b.order_date), DATENAME(MONTH, b.order_date)
),
MoviePerformance AS (
    SELECT 
        m.title,
        m.genre,
        COUNT(DISTINCT s.screening_id) AS total_screenings,
        COUNT(t.ticket_id) AS total_tickets_sold,
        SUM(t.price) AS movie_revenue,
        COUNT(DISTINCT b.user_id) AS unique_viewers,
        AVG(t.price) AS avg_ticket_price
    FROM Movie m
    INNER JOIN Screening s ON m.movie_id = s.movie_id
    INNER JOIN Ticket t ON s.screening_id = t.screening_id
    INNER JOIN Booking b ON t.booking_id = b.booking_id
    WHERE b.status = 'COMPLETED'
        AND s.start_time >= DATEADD(MONTH, -6, GETDATE())
    GROUP BY m.movie_id, m.title, m.genre
),
RevenueTrends AS (
    SELECT 
        *,
        LAG(total_revenue) OVER (ORDER BY year, month) AS prev_month_revenue,
        LEAD(total_revenue) OVER (ORDER BY year, month) AS next_month_revenue
    FROM MonthlyRevenue
)
SELECT 
    '=== МЕСЯЧНАЯ СТАТИСТИКА ===' AS section,
    NULL AS title, NULL AS genre, NULL AS total_screenings, NULL AS total_tickets_sold,
    NULL AS movie_revenue, NULL AS unique_viewers, NULL AS avg_ticket_price,
    NULL AS prev_month_revenue, NULL AS next_month_revenue, NULL AS revenue_growth

UNION ALL

SELECT 
    CONCAT(year, '-', RIGHT('0' + CAST(month AS VARCHAR), 2)) AS section,
    month_name AS title,
    NULL AS genre,
    total_bookings AS total_screenings,
    unique_customers AS total_tickets_sold,
    total_revenue AS movie_revenue,
    NULL AS unique_viewers,
    avg_booking_value AS avg_ticket_price,
    prev_month_revenue,
    next_month_revenue,
    CASE 
        WHEN prev_month_revenue IS NOT NULL 
        THEN ROUND((total_revenue - prev_month_revenue) / prev_month_revenue * 100, 2)
        ELSE NULL 
    END AS revenue_growth
FROM RevenueTrends

UNION ALL

SELECT 
    '=== ТОП ФИЛЬМЫ ПО ДОХОДНОСТИ ===' AS section,
    NULL AS title, NULL AS genre, NULL AS total_screenings, NULL AS total_tickets_sold,
    NULL AS movie_revenue, NULL AS unique_viewers, NULL AS avg_ticket_price,
    NULL AS prev_month_revenue, NULL AS next_month_revenue, NULL AS revenue_growth

UNION ALL

SELECT 
    NULL AS section,
    title,
    genre,
    total_screenings,
    total_tickets_sold,
    movie_revenue,
    unique_viewers,
    avg_ticket_price,
    NULL AS prev_month_revenue,
    NULL AS next_month_revenue,
    NULL AS revenue_growth
FROM MoviePerformance
WHERE total_tickets_sold >= 10  -- Фильмы с минимум 10 проданными билетами
ORDER BY 
    CASE WHEN section LIKE '===%' THEN 1 ELSE 2 END,
    CASE WHEN section = '=== МЕСЯЧНАЯ СТАТИСТИКА ===' THEN 1
         WHEN section = '=== ТОП ФИЛЬМЫ ПО ДОХОДНОСТИ ===' THEN 2
         ELSE 3 END,
    movie_revenue DESC;
```

**Результат:** Комплексный анализ эффективности кинотеатра с трендами и топ-фильмами.

**Пример вывода:**
```
section                        | title      | genre      | total_screenings | total_tickets_sold | movie_revenue | unique_viewers | avg_ticket_price | prev_month_revenue | next_month_revenue | revenue_growth
-------------------------------|------------|------------|------------------|-------------------|---------------|----------------|------------------|--------------------|--------------------|---------------
=== МЕСЯЧНАЯ СТАТИСТИКА ===   | NULL       | NULL       | NULL             | NULL              | NULL          | NULL           | NULL             | NULL               | NULL               | NULL
2024-01                        | Январь     | NULL       | 45               | 120               | 36000.00      | NULL           | 300.00           | NULL               | 42000.00           | NULL
2024-02                        | Февраль    | NULL       | 52               | 140               | 42000.00      | NULL           | 300.00           | 36000.00           | 38000.00           | 16.67
=== ТОП ФИЛЬМЫ ПО ДОХОДНОСТИ === | NULL     | NULL       | NULL             | NULL              | NULL          | NULL           | NULL             | NULL               | NULL               | NULL
Аватар                         | Аватар     | Фантастика | 15               | 45                | 13500.00      | 42             | 300.00           | NULL               | NULL               | NULL
Топ Ган                        | Топ Ган    | Боевик     | 12               | 36                | 10800.00      | 35             | 300.00           | NULL               | NULL               | NULL
```

---

## 📊 **Дополнительные полезные запросы**

### Запрос для проверки доступных мест на конкретный сеанс

```sql
DECLARE @ScreeningId INT = 1;  -- ID сеанса

SELECT 
    s.screening_id,
    m.title AS movie_title,
    h.hall_name,
    s.start_time,
    h.capacity,
    COUNT(t.ticket_id) AS booked_seats,
    h.capacity - COUNT(t.ticket_id) AS available_seats,
    CAST(COUNT(t.ticket_id) AS FLOAT) / h.capacity * 100 AS occupancy_percentage
FROM Screening s
INNER JOIN Movie m ON s.movie_id = m.movie_id
INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
LEFT JOIN Ticket t ON s.screening_id = t.screening_id
    AND EXISTS (SELECT 1 FROM Booking b WHERE t.booking_id = b.booking_id AND b.status = 'COMPLETED')
WHERE s.screening_id = @ScreeningId
GROUP BY s.screening_id, m.title, h.hall_name, s.start_time, h.capacity;
```

### Запрос для поиска пользователей с наибольшим количеством отмененных бронирований

```sql
SELECT 
    u.name,
    u.email,
    COUNT(CASE WHEN b.status = 'CANCELLED' THEN 1 END) AS cancelled_bookings,
    COUNT(CASE WHEN b.status = 'COMPLETED' THEN 1 END) AS completed_bookings,
    COUNT(b.booking_id) AS total_bookings,
    CAST(COUNT(CASE WHEN b.status = 'CANCELLED' THEN 1 END) AS FLOAT) / COUNT(b.booking_id) * 100 AS cancellation_rate
FROM [User] u
INNER JOIN Booking b ON u.user_id = b.user_id
GROUP BY u.user_id, u.name, u.email
HAVING COUNT(b.booking_id) >= 3  -- Пользователи с минимум 3 бронированиями
ORDER BY cancellation_rate DESC, cancelled_bookings DESC;
```

---

## 🎯 **Заключение**

Эти запросы демонстрируют различные уровни сложности работы с базой данных:

1. **🟢 Простой SELECT** - базовая выборка данных
2. **🟡 JOIN с фильтрацией** - объединение таблиц с условиями
3. **🟠 Агрегация** - группировка и статистические функции
4. **🔴 Подзапросы** - сложная логика с CTE
5. **🟣 Оконные функции** - продвинутая аналитика с трендами

Каждый запрос решает конкретную бизнес-задачу и может быть адаптирован под различные потребности системы бронирования кинотеатра.

