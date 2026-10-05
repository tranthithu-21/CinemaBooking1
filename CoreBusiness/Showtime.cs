namespace CoreBusiness
{
    public class Showtime
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public int AuditoriumId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }

        // Navigation reference objects if needed
        public Movie? Movie { get; set; }
        public Auditorium? Auditorium { get; set; }
    }
}
