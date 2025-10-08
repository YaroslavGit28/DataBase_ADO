using System;

namespace CinemaBookingApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending";
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
