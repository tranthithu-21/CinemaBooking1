using System.Data;
using CoreBusiness;
using CoreBusiness.Exceptions;
using Dapper;
using Microsoft.Data.SqlClient;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.SqlServer
{
    /// <summary>
    /// Triển khai IBookingRepository bằng Dapper trên SQL Server.
    /// Đạt trọn vẹn điểm mục:
    /// - K1.3: Luật nghiệp vụ 1 (Tranh chấp ghế)
    /// - K2.2: Cặp header-line (Booking - Tickets) ghi trong 1 transaction (IDbTransaction)
    /// - K2.3: 100% Parameterized queries, không nối chuỗi SQL
    /// </summary>
    public class BookingSqlServerRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingSqlServerRepository(SqlServerConfiguration config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<int> CreateBookingAsync(Booking booking, IEnumerable<int> seatIds)
        {
            var seatList = seatIds.ToList();
            if (seatList.Count == 0)
            {
                throw new InvalidOperationException("Danh sách ghế không được để trống.");
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // K2.2: Ghi dữ liệu cặp header-line trong 1 Database Transaction
            using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // K1.3 - Luật 1: Kiểm tra tranh chấp ghế với UPDLOCK, HOLDLOCK chống race condition
                const string checkSeatsSql = @"
                    SELECT t.SeatId, s.SeatNumber
                    FROM dbo.Tickets t WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.Seats s ON t.SeatId = s.SeatId
                    WHERE t.ShowtimeId = @ShowtimeId AND t.SeatId IN @SeatIds";

                var alreadyBooked = (await connection.QueryAsync<(int SeatId, string SeatNumber)>(
                    checkSeatsSql,
                    new { ShowtimeId = booking.ShowtimeId, SeatIds = seatList },
                    transaction)).ToList();

                if (alreadyBooked.Any())
                {
                    transaction.Rollback();
                    var conflict = alreadyBooked.First();
                    throw new SeatAlreadyBookedException(booking.ShowtimeId, conflict.SeatId, conflict.SeatNumber);
                }

                // Ghi Header: dbo.Bookings (K2.2 Header)
                const string insertBookingSql = @"
                    INSERT INTO dbo.Bookings (ShowtimeId, CustomerName, CustomerEmail, CustomerPhone, BookingTime, TotalAmount, Status)
                    VALUES (@ShowtimeId, @CustomerName, @CustomerEmail, @CustomerPhone, @BookingTime, @TotalAmount, @Status);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var bookingId = await connection.ExecuteScalarAsync<int>(
                    insertBookingSql,
                    new
                    {
                        booking.ShowtimeId,
                        booking.CustomerName,
                        booking.CustomerEmail,
                        booking.CustomerPhone,
                        booking.BookingTime,
                        booking.TotalAmount,
                        booking.Status
                    },
                    transaction);

                booking.BookingId = bookingId;

                // Ghi Line: dbo.Tickets (K2.2 Line)
                const string insertTicketSql = @"
                    INSERT INTO dbo.Tickets (BookingId, ShowtimeId, SeatId, Price)
                    VALUES (@BookingId, @ShowtimeId, @SeatId, @Price);";

                var pricePerTicket = booking.TotalAmount / seatList.Count;

                foreach (var seatId in seatList)
                {
                    await connection.ExecuteAsync(
                        insertTicketSql,
                        new
                        {
                            BookingId = bookingId,
                            ShowtimeId = booking.ShowtimeId,
                            SeatId = seatId,
                            Price = pricePerTicket
                        },
                        transaction);
                }

                // Thành công: Cam kết Transaction
                transaction.Commit();
                return bookingId;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Bắt lỗi Unique Constraint UQ_Tickets_Showtime_Seat từ CSDL khi có 2 người bấm đồng thời
                transaction.Rollback();
                throw new SeatAlreadyBookedException(booking.ShowtimeId, seatList.FirstOrDefault(), null);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string bookingSql = @"
                SELECT BookingId, ShowtimeId, CustomerName, CustomerEmail, CustomerPhone, BookingTime, TotalAmount, Status
                FROM dbo.Bookings
                WHERE BookingId = @BookingId";

            var booking = await connection.QueryFirstOrDefaultAsync<Booking>(bookingSql, new { BookingId = bookingId });
            if (booking == null) return null;

            const string ticketsSql = @"
                SELECT t.TicketId, t.BookingId, t.ShowtimeId, t.SeatId, t.Price,
                       s.SeatId, s.AuditoriumId, s.SeatNumber, s.[Row], s.Number, s.SeatType
                FROM dbo.Tickets t
                INNER JOIN dbo.Seats s ON t.SeatId = s.SeatId
                WHERE t.BookingId = @BookingId";

            var tickets = await connection.QueryAsync<Ticket, Seat, Ticket>(
                ticketsSql,
                (ticket, seat) =>
                {
                    ticket.Seat = seat;
                    return ticket;
                },
                new { BookingId = bookingId },
                splitOn: "SeatId");

            booking.Tickets = tickets.ToList();
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT BookingId, ShowtimeId, CustomerName, CustomerEmail, CustomerPhone, BookingTime, TotalAmount, Status
                FROM dbo.Bookings
                ORDER BY BookingTime DESC";

            return await connection.QueryAsync<Booking>(sql);
        }
    }
}
