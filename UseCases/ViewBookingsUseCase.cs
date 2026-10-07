using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    public class ViewBookingsUseCase
    {
        private readonly IBookingRepository _bookingRepository;

        public ViewBookingsUseCase(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Booking>> ExecuteAsync()
        {
            return await _bookingRepository.GetAllBookingsAsync();
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetBookingByIdAsync(bookingId);
        }
    }
}
