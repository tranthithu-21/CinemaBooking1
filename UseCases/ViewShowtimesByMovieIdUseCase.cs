using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    public class ViewShowtimesByMovieIdUseCase
    {
        private readonly IShowtimeRepository _showtimeRepository;

        public ViewShowtimesByMovieIdUseCase(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
        }

        public async Task<IEnumerable<Showtime>> ExecuteAsync(int movieId)
        {
            return await _showtimeRepository.GetShowtimesByMovieIdAsync(movieId);
        }
    }
}
