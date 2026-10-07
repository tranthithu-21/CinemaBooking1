using CoreBusiness;

namespace UseCases.DataStorePluginInterfaces
{
    public interface IBookingRepository
    {
        /// <summary>
        /// Ghi đồng thời Booking (Header) và Tickets (Lines) trong cùng 1 Database Transaction (K2.2).
        /// Đảm bảo tính toàn vẹn và ngăn chặn tranh chấp ghế (Luật 1).
        /// </summary>
        Task<int> CreateBookingAsync(Booking booking, IEnumerable<int> seatIds);

        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
    }
}
