namespace CoreBusiness
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }

        public Seat? Seat { get; set; }
    }
}
