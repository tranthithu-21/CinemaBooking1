using CoreBusiness;

namespace UseCases.DataStorePluginInterfaces
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<Showtime>> GetAllShowtimesAsync();
        Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId);
        Task<Showtime?> GetShowtimeByIdAsync(int showtimeId);
        Task<int> AddShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        Task DeleteShowtimeAsync(int showtimeId);
        Task<bool> HasOverlapAsync(int auditoriumId, DateTime startTime, DateTime endTime, int? excludeShowtimeId = null);
    }
}
