using System;

namespace CinemaBookingApp.Models
{
    public class HallType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal PriceMultiplier { get; set; }
        public string? Description { get; set; }
    }
}
