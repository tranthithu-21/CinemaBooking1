using CoreBusiness;
using CoreBusiness.Exceptions;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    /// <summary>
    /// USE CASE: Tạo suất chiếu mới cho phòng chiếu
    /// Cài đặt LUẬT NGHIỆP VỤ 2 (Tránh trùng lịch phòng - K1.3):
    /// Khi tạo suất chiếu mới, hệ thống tự kiểm tra thời gian bắt đầu và kết thúc
    /// (Thời lượng phim + 15 phút dọn dẹp phòng) để không bị chồng lấn với bất kỳ
    /// suất chiếu nào khác trong cùng một phòng chiếu.
    /// </summary>
    public class CreateShowtimeUseCase
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IAuditoriumRepository _auditoriumRepository;

        // Thời gian dọn phòng vệ sinh quy định là 15 phút
        public const int CleaningBreakMinutes = 15;

        public CreateShowtimeUseCase(
            IShowtimeRepository showtimeRepository,
            IMovieRepository movieRepository,
            IAuditoriumRepository auditoriumRepository)
        {
            _showtimeRepository = showtimeRepository;
            _movieRepository = movieRepository;
            _auditoriumRepository = auditoriumRepository;
        }

        public async Task<int> ExecuteAsync(int movieId, int auditoriumId, DateTime startTime, decimal price)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                throw new InvalidOperationException($"Không tìm thấy phim có mã {movieId}.");
            }

            var auditorium = await _auditoriumRepository.GetAuditoriumByIdAsync(auditoriumId);
            if (auditorium == null)
            {
                throw new InvalidOperationException($"Không tìm thấy phòng chiếu có mã {auditoriumId}.");
            }

            // Luật 2: Tự động tính EndTime = StartTime + Thời lượng phim + 15 phút dọn phòng
            var totalDurationWithCleaning = movie.DurationMinutes + CleaningBreakMinutes;
            var endTime = startTime.AddMinutes(totalDurationWithCleaning);

            // Kiểm tra chồng lấn thời gian trong cùng phòng chiếu
            var hasOverlap = await _showtimeRepository.HasOverlapAsync(auditoriumId, startTime, endTime);
            if (hasOverlap)
            {
                throw new ShowtimeOverlapException(
                    auditoriumId,
                    startTime,
                    endTime,
                    $"Phòng chiếu '{auditorium.Name}' đã có suất chiếu khác trong khoảng thời gian {startTime:HH:mm dd/MM/yyyy} - {endTime:HH:mm dd/MM/yyyy} (đã bao gồm 15 phút dọn phòng). Vui lòng chọn khung giờ khác!");
            }

            var showtime = new Showtime
            {
                MovieId = movieId,
                AuditoriumId = auditoriumId,
                StartTime = startTime,
                EndTime = endTime,
                Price = price
            };

            return await _showtimeRepository.AddShowtimeAsync(showtime);
        }
    }
}
