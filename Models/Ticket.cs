using System;

namespace CinemaBookingApp.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int BookingId { get; set; }
        public int ScreeningId { get; set; }
        public Booking? Booking { get; set; }
        public Screening? Screening { get; set; }
    }
}
