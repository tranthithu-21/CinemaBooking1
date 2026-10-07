namespace CoreBusiness.Exceptions
{
    public class SeatAlreadyBookedException : Exception
    {
        public int ShowtimeId { get; }
        public int SeatId { get; }
        public string? SeatNumber { get; }

        public SeatAlreadyBookedException(int showtimeId, int seatId, string? seatNumber = null)
            : base(seatNumber != null
                ? $"Ghế {seatNumber} trong suất chiếu này đã được khách hàng khác đặt. Vui lòng chọn ghế khác!"
                : $"Một hoặc nhiều ghế bạn chọn trong suất chiếu này đã có người đặt trước. Vui lòng chọn ghế khác!")
        {
            ShowtimeId = showtimeId;
            SeatId = seatId;
            SeatNumber = seatNumber;
        }
    }
}
