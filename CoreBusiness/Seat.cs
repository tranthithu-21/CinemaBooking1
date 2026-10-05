namespace CoreBusiness
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int AuditoriumId { get; set; }
        public string SeatNumber { get; set; } = string.Empty; // e.g., "A1", "B5"
        public string Row { get; set; } = string.Empty;        // e.g., "A", "B"
        public int Number { get; set; }                        // e.g., 1, 2
        public string SeatType { get; set; } = "Standard";     // Standard, VIP, Couple
    }
}
