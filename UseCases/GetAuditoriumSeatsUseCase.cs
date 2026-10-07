using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    public class SeatStatusDto
    {
        public Seat Seat { get; set; } = new();
        public bool IsBooked { get; set; }
    }

    public class GetAuditoriumSeatsUseCase
    {
        private readonly ISeatRepository _seatRepository;

        public GetAuditoriumSeatsUseCase(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<List<SeatStatusDto>> ExecuteAsync(int auditoriumId, int showtimeId)
        {
            var seats = await _seatRepository.GetSeatsByAuditoriumIdAsync(auditoriumId);
            var bookedSeatIds = (await _seatRepository.GetBookedSeatIdsByShowtimeIdAsync(showtimeId)).ToHashSet();

            return seats.Select(s => new SeatStatusDto
            {
                Seat = s,
                IsBooked = bookedSeatIds.Contains(s.SeatId)
            }).ToList();
        }
    }
}
