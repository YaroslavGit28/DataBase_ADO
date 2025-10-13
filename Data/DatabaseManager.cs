using System;                    // Базовые типы и классы .NET Framework
using System.Data;               // Классы для работы с данными (DataTable, DataSet)
using System.Collections.Generic; // Коллекции (List, Dictionary)
using Microsoft.Data.SqlClient;  // Классы для работы с SQL Server
using CinemaBookingApp.Models;    // Наши модели данных

namespace CinemaBookingApp.Data
{
    /// <summary>
    /// Менеджер базы данных для работы с данными через ADO.NET
    /// Предоставляет методы для выполнения SQL запросов и управления данными
    /// </summary>
    public class DataBaseManager
    {
        /// <summary>
        /// Строка подключения к базе данных SQL Server
        /// Содержит адрес сервера, имя базы данных, учетные данные и параметры безопасности
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Конструктор менеджера базы данных
        /// Инициализирует строку подключения к базе данных
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных</param>
        public DataBaseManager(string connectionString)
        {
            this.connectionString = connectionString;  // Сохраняем строку подключения
        }

        // =============================================
        // МЕТОДЫ ДЛЯ РАБОТЫ С ПОЛЬЗОВАТЕЛЯМИ
        // =============================================
        
        /// <summary>
        /// Добавляет нового пользователя в базу данных
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="email">Email адрес пользователя</param>
        /// <param name="phone">Номер телефона (необязательный параметр)</param>
        /// <returns>ID созданного пользователя</returns>
        public int AddUser(string name, string email, string? phone = null)
        {
            using (var connection = new SqlConnection(connectionString))  // Создаем подключение к БД
            {
                connection.Open();  // Открываем соединение с базой данных
                
                // SQL запрос для добавления пользователя с возвратом ID
                string query = @"INSERT INTO [User] (name, email, phone) 
                               OUTPUT INSERTED.user_id 
                               VALUES (@name, @email, @phone)";
                
                var command = new SqlCommand(query, connection);  // Создаем команду SQL
                
                // Добавляем параметры к команде для защиты от SQL инъекций
                command.Parameters.AddWithValue("@name", name);                    // Имя пользователя
                command.Parameters.AddWithValue("@email", email);                 // Email адрес
                command.Parameters.AddWithValue("@phone", phone ?? (object)DBNull.Value);  // Телефон или NULL
                
                // Выполняем команду и возвращаем ID созданного пользователя
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Обновляет информацию о существующем пользователе
        /// </summary>
        /// <param name="userId">ID пользователя для обновления</param>
        /// <param name="name">Новое имя пользователя</param>
        /// <param name="email">Новый email адрес</param>
        /// <param name="phone">Новый номер телефона (необязательный параметр)</param>
        public void UpdateUser(int userId, string name, string email, string? phone = null)
        {
            using (var connection = new SqlConnection(connectionString))  // Создаем подключение к БД
            {
                connection.Open();  // Открываем соединение с базой данных
                
                // SQL запрос для обновления данных пользователя
                string query = @"UPDATE [User] 
                               SET name = @name, email = @email, phone = @phone 
                               WHERE user_id = @user_id";
                
                var command = new SqlCommand(query, connection);  // Создаем команду SQL
                
                // Добавляем параметры к команде для защиты от SQL инъекций
                command.Parameters.AddWithValue("@user_id", userId);              // ID пользователя
                command.Parameters.AddWithValue("@name", name);                    // Новое имя
                command.Parameters.AddWithValue("@email", email);                 // Новый email
                command.Parameters.AddWithValue("@phone", phone ?? (object)DBNull.Value);  // Новый телефон или NULL
                
                command.ExecuteNonQuery();  // Выполняем команду обновления
            }
        }

        public void DeleteUser(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM [User] WHERE user_id = @user_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@user_id", userId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetUsers()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id, name, email, phone FROM [User] ORDER BY name";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataRow? GetUserById(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id, name, email, phone FROM [User] WHERE user_id = @user_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@user_id", userId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }

        public DataRow? GetUserByEmail(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id, name, email, phone FROM [User] WHERE email = @email";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@email", email);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }
        #endregion

        #region Movie Methods
        public int AddMovie(string title, string genre, int durationMinutes, string? description = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Movie (title, genre, duration_minutes, description) 
                               OUTPUT INSERTED.movie_id 
                               VALUES (@title, @genre, @duration_minutes, @description)";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@genre", genre);
                command.Parameters.AddWithValue("@duration_minutes", durationMinutes);
                command.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void UpdateMovie(int movieId, string title, string genre, int durationMinutes, string? description = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Movie 
                               SET title = @title, genre = @genre, 
                                   duration_minutes = @duration_minutes, description = @description 
                               WHERE movie_id = @movie_id";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@movie_id", movieId);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@genre", genre);
                command.Parameters.AddWithValue("@duration_minutes", durationMinutes);
                command.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }

        public void DeleteMovie(int movieId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Movie WHERE movie_id = @movie_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@movie_id", movieId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetMovies()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT movie_id, title, genre, duration_minutes, description FROM Movie ORDER BY title";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataRow? GetMovieById(int movieId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT movie_id, title, genre, duration_minutes, description FROM Movie WHERE movie_id = @movie_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@movie_id", movieId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }
        #endregion

        #region Hall Type Methods
        public DataTable GetHallTypes()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT type_id, type_name, price_multiplier, description FROM Hall_Type ORDER BY type_name";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataRow? GetHallTypeById(int typeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT type_id, type_name, price_multiplier, description FROM Hall_Type WHERE type_id = @type_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@type_id", typeId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }
        #endregion

        #region Cinema Hall Methods
        public DataTable GetCinemaHalls()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT h.hall_id, h.hall_name, h.capacity, h.type_id, 
                                       ht.type_name, ht.price_multiplier
                                FROM Cinema_Hall h
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                ORDER BY h.hall_name";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataRow? GetCinemaHallById(int hallId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT h.hall_id, h.hall_name, h.capacity, h.type_id, 
                                       ht.type_name, ht.price_multiplier
                                FROM Cinema_Hall h
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                WHERE h.hall_id = @hall_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@hall_id", hallId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }

        public DataTable GetAvailableHalls(DateTime startTime, DateTime endTime)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT h.hall_id, h.hall_name, h.capacity, ht.type_name
                                FROM Cinema_Hall h
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                WHERE h.hall_id NOT IN (
                                    SELECT s.hall_id 
                                    FROM Screening s 
                                    WHERE NOT (s.end_time <= @start_time OR s.start_time >= @end_time)
                                )";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@start_time", startTime);
                adapter.SelectCommand.Parameters.AddWithValue("@end_time", endTime);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        #endregion

        #region Screening Methods
        public int AddScreening(DateTime startTime, DateTime endTime, int movieId, int hallId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Screening (start_time, end_time, movie_id, hall_id) 
                               OUTPUT INSERTED.screening_id 
                               VALUES (@start_time, @end_time, @movie_id, @hall_id)";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@start_time", startTime);
                command.Parameters.AddWithValue("@end_time", endTime);
                command.Parameters.AddWithValue("@movie_id", movieId);
                command.Parameters.AddWithValue("@hall_id", hallId);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void UpdateScreening(int screeningId, DateTime startTime, DateTime endTime, int movieId, int hallId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Screening 
                               SET start_time = @start_time, end_time = @end_time, 
                                   movie_id = @movie_id, hall_id = @hall_id 
                               WHERE screening_id = @screening_id";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@screening_id", screeningId);
                command.Parameters.AddWithValue("@start_time", startTime);
                command.Parameters.AddWithValue("@end_time", endTime);
                command.Parameters.AddWithValue("@movie_id", movieId);
                command.Parameters.AddWithValue("@hall_id", hallId);
                
                command.ExecuteNonQuery();
            }
        }

        public void DeleteScreening(int screeningId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Screening WHERE screening_id = @screening_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@screening_id", screeningId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetScreenings()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT s.screening_id, m.title, s.start_time, s.end_time, 
                                       h.hall_name, ht.type_name, m.duration_minutes, h.type_id
                                FROM Screening s
                                INNER JOIN Movie m ON s.movie_id = m.movie_id
                                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                ORDER BY s.start_time";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataRow? GetScreeningById(int screeningId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT s.screening_id, s.start_time, s.end_time, 
                                       s.movie_id, s.hall_id, m.title, h.hall_name,
                                       ht.type_name, ht.price_multiplier, h.capacity, h.type_id
                                FROM Screening s
                                INNER JOIN Movie m ON s.movie_id = m.movie_id
                                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                WHERE s.screening_id = @screening_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@screening_id", screeningId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }

        public DataTable GetScreeningsByDate(DateTime date)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT s.screening_id, m.title, s.start_time, s.end_time, 
                                       h.hall_name, ht.type_name, m.duration_minutes
                                FROM Screening s
                                INNER JOIN Movie m ON s.movie_id = m.movie_id
                                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                                WHERE CAST(s.start_time AS DATE) = @date
                                ORDER BY s.start_time";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@date", date.Date);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        #endregion

        #region Booking Methods
        public int CreateBooking(int userId, decimal totalAmount, string status = "pending")
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Booking (order_date, total_amount, status, user_id) 
                               OUTPUT INSERTED.booking_id 
                               VALUES (@order_date, @total_amount, @status, @user_id)";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@order_date", DateTime.Now);
                command.Parameters.AddWithValue("@total_amount", totalAmount);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@user_id", userId);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void UpdateBookingStatus(int bookingId, string status)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Booking SET status = @status WHERE booking_id = @booking_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@booking_id", bookingId);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteBooking(int bookingId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Booking WHERE booking_id = @booking_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@booking_id", bookingId);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Отменить отдельные билеты в бронировании
        /// </summary>
        public void CancelTickets(int bookingId, List<int> ticketIds)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Удаляем выбранные билеты
                        foreach (int ticketId in ticketIds)
                        {
                            string deleteTicketQuery = "DELETE FROM Ticket WHERE ticket_id = @ticket_id AND booking_id = @booking_id";
                            var deleteCommand = new SqlCommand(deleteTicketQuery, connection, transaction);
                            deleteCommand.Parameters.AddWithValue("@ticket_id", ticketId);
                            deleteCommand.Parameters.AddWithValue("@booking_id", bookingId);
                            deleteCommand.ExecuteNonQuery();
                        }

                        // Пересчитываем общую сумму бронирования
                        string recalculateQuery = @"
                            UPDATE Booking 
                            SET total_amount = (
                                SELECT ISNULL(SUM(price), 0) 
                                FROM Ticket 
                                WHERE booking_id = @booking_id
                            )
                            WHERE booking_id = @booking_id";
                        
                        var recalculateCommand = new SqlCommand(recalculateQuery, connection, transaction);
                        recalculateCommand.Parameters.AddWithValue("@booking_id", bookingId);
                        recalculateCommand.ExecuteNonQuery();

                        // Проверяем, остались ли билеты в бронировании
                        string checkTicketsQuery = "SELECT COUNT(*) FROM Ticket WHERE booking_id = @booking_id";
                        var checkCommand = new SqlCommand(checkTicketsQuery, connection, transaction);
                        checkCommand.Parameters.AddWithValue("@booking_id", bookingId);
                        int remainingTickets = Convert.ToInt32(checkCommand.ExecuteScalar());

                        // Если билетов не осталось, отменяем все бронирование
                        if (remainingTickets == 0)
                        {
                            string cancelBookingQuery = "UPDATE Booking SET status = 'CANCELLED' WHERE booking_id = @booking_id";
                            var cancelCommand = new SqlCommand(cancelBookingQuery, connection, transaction);
                            cancelCommand.Parameters.AddWithValue("@booking_id", bookingId);
                            cancelCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Получить все билеты для конкретного бронирования
        /// </summary>
        public DataTable GetTicketsByBookingId(int bookingId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT t.ticket_id, t.seat_number, t.price, 
                           s.start_time, m.title as movie_title, h.hall_name
                    FROM Ticket t
                    INNER JOIN Screening s ON t.screening_id = s.screening_id
                    INNER JOIN Movie m ON s.movie_id = m.movie_id
                    INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                    WHERE t.booking_id = @booking_id
                    ORDER BY t.seat_number";
                
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@booking_id", bookingId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetBookings()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.booking_id, u.name as customer_name, u.email, u.phone,
                                       b.order_date, b.total_amount, b.status
                                FROM Booking b
                                INNER JOIN [User] u ON b.user_id = u.user_id
                                ORDER BY b.order_date DESC";
                var adapter = new SqlDataAdapter(query, connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        /// <summary>
        /// Получить бронирования конкретного пользователя
        /// </summary>
        public DataTable GetBookingsByUserId(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.booking_id, u.name as customer_name, u.email, u.phone,
                                       b.order_date, b.total_amount, b.status
                                FROM Booking b
                                INNER JOIN [User] u ON b.user_id = u.user_id
                                WHERE b.user_id = @user_id
                                ORDER BY b.order_date DESC";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@user_id", userId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        /// <summary>
        /// Получить ID пользователя по имени
        /// </summary>
        public int? GetUserIdByName(string userName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM [User] WHERE name = @name";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", userName);
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : null;
            }
        }

        public DataRow? GetBookingById(int bookingId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.booking_id, u.name as customer_name, u.email, u.phone,
                                       b.order_date, b.total_amount, b.status, b.user_id
                                FROM Booking b
                                INNER JOIN [User] u ON b.user_id = u.user_id
                                WHERE b.booking_id = @booking_id";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@booking_id", bookingId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
            }
        }

        public DataTable GetBookingsByUser(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.booking_id, b.order_date, b.total_amount, b.status
                                FROM Booking b
                                WHERE b.user_id = @user_id
                                ORDER BY b.order_date DESC";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@user_id", userId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        #endregion

        #region Ticket Methods
        public int AddTicket(string seatNumber, decimal basePrice, decimal finalPrice, int bookingId, int screeningId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Ticket (seat_number, price, base_price, final_price, booking_id, screening_id) 
                               OUTPUT INSERTED.ticket_id 
                               VALUES (@seat_number, @price, @base_price, @final_price, @booking_id, @screening_id)";
                
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@seat_number", seatNumber);
                command.Parameters.AddWithValue("@price", finalPrice);
                command.Parameters.AddWithValue("@base_price", basePrice);
                command.Parameters.AddWithValue("@final_price", finalPrice);
                command.Parameters.AddWithValue("@booking_id", bookingId);
                command.Parameters.AddWithValue("@screening_id", screeningId);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteTicket(int ticketId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Ticket WHERE ticket_id = @ticket_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ticket_id", ticketId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetTicketsByBooking(int bookingId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT t.ticket_id, t.seat_number, t.base_price, t.final_price,
                                       m.title, s.start_time, h.hall_name
                                FROM Ticket t
                                INNER JOIN Screening s ON t.screening_id = s.screening_id
                                INNER JOIN Movie m ON s.movie_id = m.movie_id
                                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                                WHERE t.booking_id = @booking_id
                                ORDER BY t.seat_number";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@booking_id", bookingId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetTicketsByScreening(int screeningId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT t.seat_number, t.final_price, u.name as customer_name
                                FROM Ticket t
                                INNER JOIN Booking b ON t.booking_id = b.booking_id
                                INNER JOIN [User] u ON b.user_id = u.user_id
                                WHERE t.screening_id = @screening_id AND b.status != 'cancelled'
                                ORDER BY t.seat_number";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@screening_id", screeningId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public List<string> GetOccupiedSeats(int screeningId)
        {
            var occupiedSeats = new List<string>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT t.seat_number 
                                FROM Ticket t
                                INNER JOIN Booking b ON t.booking_id = b.booking_id
                                WHERE t.screening_id = @screening_id AND b.status != 'cancelled'";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@screening_id", screeningId);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        occupiedSeats.Add(reader["seat_number"].ToString() ?? "");
                    }
                }
            }
            return occupiedSeats;
        }
        #endregion

        // =============================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // =============================================
        
        /// <summary>
        /// Тестирует подключение к базе данных
        /// Проверяет, можно ли установить соединение с SQL Server
        /// </summary>
        /// <returns>true если подключение успешно, false если произошла ошибка</returns>
        public bool TestConnection()
        {
            try  // Обрабатываем возможные ошибки подключения
            {
                using (var connection = new SqlConnection(connectionString))  // Создаем подключение
                {
                    connection.Open();  // Пытаемся открыть соединение
                    return true;        // Если успешно - возвращаем true
                }
            }
            catch  // Если произошла любая ошибка
            {
                return false;  // Возвращаем false
            }
        }

        public decimal CalculateTicketPrice(decimal basePrice, int hallTypeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT price_multiplier FROM Hall_Type WHERE type_id = @type_id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@type_id", hallTypeId);
                
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    var multiplier = Convert.ToDecimal(result);
                    return basePrice * multiplier;
                }
                return basePrice;
            }
        }

        public DataTable GetBookingDetails(int bookingId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.booking_id, u.name, u.email, u.phone,
                                       b.order_date, b.total_amount, b.status,
                                       STRING_AGG(CONCAT(m.title, ' (', t.seat_number, ')'), ', ') as tickets_info
                                FROM Booking b
                                INNER JOIN [User] u ON b.user_id = u.user_id
                                INNER JOIN Ticket t ON b.booking_id = t.booking_id
                                INNER JOIN Screening s ON t.screening_id = s.screening_id
                                INNER JOIN Movie m ON s.movie_id = m.movie_id
                                WHERE b.booking_id = @booking_id
                                GROUP BY b.booking_id, u.name, u.email, u.phone, b.order_date, b.total_amount, b.status";
                var adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@booking_id", bookingId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        #endregion
    }
}