using System;

namespace CinemaBookingApp.Models
{
    public class CinemaHall
    {
        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int TypeId { get; set; }
        public HallType? HallType { get; set; }
    }
}
