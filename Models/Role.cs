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
