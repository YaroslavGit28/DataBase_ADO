using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using CinemaBookingApp.Models;

namespace CinemaBookingApp.Data
{
    /// <summary>
    /// Репозиторий для работы с базой данных через Dapper
    /// </summary>
    public class DapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Получить соединение с базой данных
        /// </summary>
        private IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region User Operations

        /// <summary>
        /// Добавить пользователя
        /// </summary>
        public async Task<int> AddUserAsync(User user)
        {
            const string sql = @"
                INSERT INTO [User] (name, email, phone) 
                OUTPUT INSERTED.user_id 
                VALUES (@Name, @Email, @Phone)";

            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(sql, user);
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            const string sql = @"
                UPDATE [User] 
                SET name = @Name, email = @Email, phone = @Phone 
                WHERE user_id = @UserId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, user);
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        public async Task DeleteUserAsync(int userId)
        {
            const string sql = "DELETE FROM [User] WHERE user_id = @UserId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId });
        }

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
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

        /// <summary>
        /// Получить пользователя по ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            const string sql = "SELECT user_id as UserId, name as Name, email as Email, phone as Phone FROM [User] WHERE user_id = @UserId";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }

        /// <summary>
        /// Получить пользователя по email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            const string sql = "SELECT user_id as UserId, name as Name, email as Email, phone as Phone FROM [User] WHERE email = @Email";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        #endregion

        #region Movie Operations

        /// <summary>
        /// Добавить фильм
        /// </summary>
        public async Task<int> AddMovieAsync(Movie movie)
        {
            const string sql = @"
                INSERT INTO Movie (title, genre, duration_minutes, description) 
                OUTPUT INSERTED.movie_id 
                VALUES (@Title, @Genre, @DurationMinutes, @Description)";

            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(sql, movie);
        }

        /// <summary>
        /// Обновить фильм
        /// </summary>
        public async Task UpdateMovieAsync(Movie movie)
        {
            const string sql = @"
                UPDATE Movie 
                SET title = @Title, genre = @Genre, duration_minutes = @DurationMinutes, description = @Description 
                WHERE movie_id = @MovieId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, movie);
        }

        /// <summary>
        /// Удалить фильм
        /// </summary>
        public async Task DeleteMovieAsync(int movieId)
        {
            const string sql = "DELETE FROM Movie WHERE movie_id = @MovieId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Получить все фильмы
        /// </summary>
        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            const string sql = "SELECT movie_id as MovieId, title as Title, genre as Genre, duration_minutes as DurationMinutes, description as Description FROM Movie ORDER BY title";

            using var connection = GetConnection();
            return await connection.QueryAsync<Movie>(sql);
        }

        /// <summary>
        /// Получить фильм по ID
        /// </summary>
        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            const string sql = "SELECT movie_id as MovieId, title as Title, genre as Genre, duration_minutes as DurationMinutes, description as Description FROM Movie WHERE movie_id = @MovieId";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Movie>(sql, new { MovieId = movieId });
        }

        #endregion

        #region Screening Operations

        /// <summary>
        /// Добавить сеанс
        /// </summary>
        public async Task<int> AddScreeningAsync(Screening screening)
        {
            const string sql = @"
                INSERT INTO Screening (start_time, end_time, movie_id, hall_id) 
                OUTPUT INSERTED.screening_id 
                VALUES (@StartTime, @EndTime, @MovieId, @HallId)";

            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(sql, screening);
        }

        /// <summary>
        /// Обновить сеанс
        /// </summary>
        public async Task UpdateScreeningAsync(Screening screening)
        {
            const string sql = @"
                UPDATE Screening 
                SET start_time = @StartTime, end_time = @EndTime, movie_id = @MovieId, hall_id = @HallId 
                WHERE screening_id = @ScreeningId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, screening);
        }

        /// <summary>
        /// Удалить сеанс
        /// </summary>
        public async Task DeleteScreeningAsync(int screeningId)
        {
            const string sql = "DELETE FROM Screening WHERE screening_id = @ScreeningId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, new { ScreeningId = screeningId });
        }

        /// <summary>
        /// Получить все сеансы с информацией о фильмах и залах
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetScreeningsWithDetailsAsync()
        {
            const string sql = @"
                SELECT s.screening_id as ScreeningId, s.start_time as StartTime, s.end_time as EndTime,
                       m.title as MovieTitle, h.hall_name as HallName, ht.type_name as HallTypeName
                FROM Screening s
                INNER JOIN Movie m ON s.movie_id = m.movie_id
                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                ORDER BY s.start_time";

            using var connection = GetConnection();
            return await connection.QueryAsync(sql);
        }

        /// <summary>
        /// Получить сеанс по ID с деталями
        /// </summary>
        public async Task<dynamic?> GetScreeningByIdWithDetailsAsync(int screeningId)
        {
            const string sql = @"
                SELECT s.screening_id as ScreeningId, s.start_time as StartTime, s.end_time as EndTime,
                       s.movie_id as MovieId, s.hall_id as HallId, m.title as MovieTitle, 
                       h.hall_name as HallName, h.capacity as HallCapacity,
                       ht.type_name as HallTypeName, ht.price_multiplier as PriceMultiplier
                FROM Screening s
                INNER JOIN Movie m ON s.movie_id = m.movie_id
                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                INNER JOIN Hall_Type ht ON h.type_id = ht.type_id
                WHERE s.screening_id = @ScreeningId";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync(sql, new { ScreeningId = screeningId });
        }

        #endregion

        #region Booking Operations

        /// <summary>
        /// Создать бронирование
        /// </summary>
        public async Task<int> CreateBookingAsync(Booking booking)
        {
            const string sql = @"
                INSERT INTO Booking (order_date, total_amount, status, user_id) 
                OUTPUT INSERTED.booking_id 
                VALUES (@OrderDate, @TotalAmount, @Status, @UserId)";

            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(sql, booking);
        }

        /// <summary>
        /// Обновить статус бронирования
        /// </summary>
        public async Task UpdateBookingStatusAsync(int bookingId, string status)
        {
            const string sql = "UPDATE Booking SET status = @Status WHERE booking_id = @BookingId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, new { BookingId = bookingId, Status = status });
        }

        /// <summary>
        /// Получить все бронирования с информацией о пользователях
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetBookingsWithDetailsAsync()
        {
            const string sql = @"
                SELECT b.booking_id as BookingId, u.name as CustomerName, u.email as Email, u.phone as Phone,
                       b.order_date as OrderDate, b.total_amount as TotalAmount, b.status as Status
                FROM Booking b
                INNER JOIN [User] u ON b.user_id = u.user_id
                ORDER BY b.order_date DESC";

            using var connection = GetConnection();
            return await connection.QueryAsync(sql);
        }

        #endregion

        #region Ticket Operations

        /// <summary>
        /// Добавить билет
        /// </summary>
        public async Task<int> AddTicketAsync(Ticket ticket)
        {
            const string sql = @"
                INSERT INTO Ticket (seat_number, price, base_price, final_price, booking_id, screening_id) 
                OUTPUT INSERTED.ticket_id 
                VALUES (@SeatNumber, @Price, @BasePrice, @FinalPrice, @BookingId, @ScreeningId)";

            using var connection = GetConnection();
            return await connection.QuerySingleAsync<int>(sql, ticket);
        }

        /// <summary>
        /// Получить билеты по бронированию
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetTicketsByBookingAsync(int bookingId)
        {
            const string sql = @"
                SELECT t.ticket_id as TicketId, t.seat_number as SeatNumber, t.base_price as BasePrice, t.final_price as FinalPrice,
                       m.title as MovieTitle, s.start_time as StartTime, h.hall_name as HallName
                FROM Ticket t
                INNER JOIN Screening s ON t.screening_id = s.screening_id
                INNER JOIN Movie m ON s.movie_id = m.movie_id
                INNER JOIN Cinema_Hall h ON s.hall_id = h.hall_id
                WHERE t.booking_id = @BookingId
                ORDER BY t.seat_number";

            using var connection = GetConnection();
            return await connection.QueryAsync(sql, new { BookingId = bookingId });
        }

        /// <summary>
        /// Получить занятые места для сеанса
        /// </summary>
        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int screeningId)
        {
            const string sql = @"
                SELECT t.seat_number 
                FROM Ticket t
                INNER JOIN Booking b ON t.booking_id = b.booking_id
                WHERE t.screening_id = @ScreeningId AND b.status != 'cancelled'";

            using var connection = GetConnection();
            return await connection.QueryAsync<string>(sql, new { ScreeningId = screeningId });
        }

        #endregion

        #region Role Operations

        /// <summary>
        /// Получить все роли
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles ORDER BY role_name";

            using var connection = GetConnection();
            return await connection.QueryAsync<Role>(sql);
        }

        /// <summary>
        /// Получить роль по ID
        /// </summary>
        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles WHERE role_id = @RoleId";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { RoleId = roleId });
        }

        /// <summary>
        /// Получить роль по названию
        /// </summary>
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            const string sql = "SELECT role_id as RoleId, role_name as RoleName, description as Description, created_date as CreatedDate FROM Roles WHERE role_name = @RoleName";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { RoleName = roleName });
        }

        /// <summary>
        /// Получить пользователей с ролями
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetUsersWithRolesAsync()
        {
            const string sql = @"
                SELECT u.user_id as UserId, u.name as Name, u.email as Email, u.phone as Phone,
                       u.role_id as RoleId, r.role_name as RoleName, r.description as RoleDescription
                FROM [User] u
                INNER JOIN Roles r ON u.role_id = r.role_id
                ORDER BY u.name";

            using var connection = GetConnection();
            return await connection.QueryAsync(sql);
        }

        /// <summary>
        /// Изменить роль пользователя
        /// </summary>
        public async Task ChangeUserRoleAsync(int userId, int newRoleId)
        {
            const string sql = "UPDATE [User] SET role_id = @NewRoleId WHERE user_id = @UserId";

            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, new { UserId = userId, NewRoleId = newRoleId });
        }

        /// <summary>
        /// Получить пользователей по роли
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            const string sql = @"
                SELECT u.user_id as UserId, u.name as Name, u.email as Email, u.phone as Phone, u.role_id as RoleId
                FROM [User] u
                INNER JOIN Roles r ON u.role_id = r.role_id
                WHERE r.role_name = @RoleName
                ORDER BY u.name";

            using var connection = GetConnection();
            return await connection.QueryAsync<User>(sql, new { RoleName = roleName });
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Проверить соединение с базой данных
        /// </summary>
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

        /// <summary>
        /// Выполнить произвольный SQL запрос
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// Выполнить команду (INSERT, UPDATE, DELETE)
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, parameters);
        }

        #endregion
    }
}
