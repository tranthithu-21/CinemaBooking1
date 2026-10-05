namespace CoreBusiness
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int ShowtimeId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled

        // Header-Line relationship
        public List<Ticket> Tickets { get; set; } = new();
    }
}
