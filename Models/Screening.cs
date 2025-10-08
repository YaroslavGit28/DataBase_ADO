using System;

namespace CinemaBookingApp.Models
{
    public class Screening
    {
        public int ScreeningId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public Movie? Movie { get; set; }
        public CinemaHall? CinemaHall { get; set; }
    }
}
