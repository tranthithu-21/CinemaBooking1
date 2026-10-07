using CoreBusiness;

namespace UseCases.DataStorePluginInterfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetSeatsByAuditoriumIdAsync(int auditoriumId);
        Task<IEnumerable<int>> GetBookedSeatIdsByShowtimeIdAsync(int showtimeId);
        Task<Seat?> GetSeatByIdAsync(int seatId);
    }
}
