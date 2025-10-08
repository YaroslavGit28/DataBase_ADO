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
