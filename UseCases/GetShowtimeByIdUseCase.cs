using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    public class GetShowtimeByIdUseCase
    {
        private readonly IShowtimeRepository _showtimeRepository;

        public GetShowtimeByIdUseCase(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
        }

        public async Task<Showtime?> ExecuteAsync(int showtimeId)
        {
            return await _showtimeRepository.GetShowtimeByIdAsync(showtimeId);
        }
    }
}
