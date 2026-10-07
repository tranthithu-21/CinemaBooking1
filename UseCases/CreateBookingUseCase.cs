using CoreBusiness;
using CoreBusiness.Exceptions;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    /// <summary>
    /// USE CASE: Đặt vé xem phim
    /// Cài đặt LUẬT NGHIỆP VỤ 1 (Tranh chấp ghế - K1.3):
    /// Một ghế trong một suất chiếu chỉ bán đúng 1 lần duy nhất.
    /// Nếu 2 người cùng đặt 1 ghế, chỉ 1 người thành công, người còn lại nhận thông báo thân thiện.
    /// Dữ liệu Booking (Header) và Tickets (Lines) được ghi trong 1 Transaction (K2.2).
    /// </summary>
    public class CreateBookingUseCase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;

        public CreateBookingUseCase(
            IBookingRepository bookingRepository,
            ISeatRepository seatRepository)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
        }

        public async Task<int> ExecuteAsync(Booking booking, List<int> seatIds)
        {
            // Kiểm tra tính hợp lệ cơ bản
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Thông tin đặt vé không được để trống.");
            }

            if (seatIds == null || seatIds.Count == 0)
            {
                throw new InvalidOperationException("Vui lòng chọn ít nhất một ghế để tiếp tục đặt vé.");
            }

            // Kiểm tra trước danh sách ghế đã được bán trong suất chiếu này
            var bookedSeatIds = await _seatRepository.GetBookedSeatIdsByShowtimeIdAsync(booking.ShowtimeId);
            var overlappingSeats = seatIds.Intersect(bookedSeatIds).ToList();
            if (overlappingSeats.Any())
            {
                var firstConflictSeat = await _seatRepository.GetSeatByIdAsync(overlappingSeats.First());
                var seatLabel = firstConflictSeat?.SeatNumber ?? overlappingSeats.First().ToString();
                throw new SeatAlreadyBookedException(booking.ShowtimeId, overlappingSeats.First(), seatLabel);
            }

            // Ghi dữ liệu Booking và Tickets vào Database trong 1 Database Transaction
            // Tầng Repository có UPDLOCK/HOLDLOCK và DB có UNIQUE KEY để bảo vệ an toàn 100% khi có race condition
            try
            {
                return await _bookingRepository.CreateBookingAsync(booking, seatIds);
            }
            catch (SeatAlreadyBookedException)
            {
                // Ném lại exception nghiệp vụ thân thiện cho UI hiển thị
                throw;
            }
        }
    }
}
