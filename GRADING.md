# BẢNG ĐỐI CHIẾU TIÊU CHÍ CHẤM ĐIỂM (GRADING EVIDENCE)
## Đề tài 14: Rạp phim: Chọn ghế & Đặt vé — Cinema Seat Booking (ECO2415)
### Sinh viên thực hiện: Trần Thị Thu

---

| Mã KT | Tiêu chí đánh giá | Điểm tối đa | Đường dẫn file làm bằng chứng | Vị trí dòng / Chi tiết minh chứng |
| :--- | :--- | :---: | :--- | :--- |
| **K1.1** | Đủ 4 project Clean Architecture; CoreBusiness không tham chiếu ai; UseCases chỉ tham chiếu CoreBusiness | 8 | `CoreBusiness/CoreBusiness.csproj`<br>`UseCases/UseCases.csproj`<br>`Plugins.DataStore.SqlServer/Plugins.DataStore.SqlServer.csproj`<br>`WebApp/WebApp.csproj` | Không có ProjectReference nào trong `CoreBusiness.csproj`. `UseCases.csproj` chỉ chứa ProjectReference tới `CoreBusiness`. `WebApp` chỉ tham chiếu Plugin tại `Program.cs`. |
| **K1.2** | Interface repository nằm ở UseCases; Plugin cài đặt; .razor inject interface / use case, không inject lớp cụ thể | 7 | `UseCases/DataStorePluginInterfaces/`<br>`Plugins.DataStore.SqlServer/`<br>`WebApp/Program.cs`<br>`WebApp/Components/Pages/BookingPage.razor` | Các interface: `IMovieRepository.cs`, `IShowtimeRepository.cs`, `IBookingRepository.cs`, `ISeatRepository.cs`, `IAuditoriumRepository.cs`. Cài đặt trong `Plugins.DataStore.SqlServer`. `Program.cs` dòng 24-28 đăng ký DI qua interface. Các file `.razor` inject UseCase, không inject class Plugin cụ thể. |
| **K1.3** | Cài đặt 2 luật nghiệp vụ của đề trong CoreBusiness / UseCases, không nằm trong file .razor (3đ/luật) | 6 | **Luật 1 (Tranh chấp ghế):**<br>- `CoreBusiness/Exceptions/SeatAlreadyBookedException.cs`<br>- `UseCases/CreateBookingUseCase.cs` (Dòng 38-61)<br>- `Plugins.DataStore.SqlServer/BookingSqlServerRepository.cs` (Dòng 38-55, 96-101)<br><br>**Luật 2 (Tránh trùng lịch phòng):**<br>- `CoreBusiness/Exceptions/ShowtimeOverlapException.cs`<br>- `UseCases/CreateShowtimeUseCase.cs` (Dòng 47-65)<br>- `Plugins.DataStore.SqlServer/ShowtimeSqlServerRepository.cs` (Dòng 143-162) | **Luật 1:** Một ghế trong một suất chiếu chỉ bán đúng 1 lần. Chặn bằng kiểm tra tồn tại + `UPDLOCK, HOLDLOCK` trong Transaction + UNIQUE constraint `UQ_Tickets_Showtime_Seat` ở CSDL. Bắt lỗi trả về thông báo thân thiện.<br><br>**Luật 2:** Các suất chiếu cùng phòng không chồng giờ. Tự động tính thời gian kết thúc = `Thời lượng phim + 15 phút dọn phòng` (`CleaningBreakMinutes = 15`), kiểm tra `(StartTime < EndTime && EndTime > StartTime)` và chặn nếu trùng. |
| **K1.4** | Đăng ký Dependency Injection đầy đủ; hỗ trợ quản lý trạng thái | 4 | `WebApp/Program.cs` (Dòng 18-38) | Đăng ký `SqlServerConfiguration` Singleton, các Repository `Transient`, các UseCase `Transient`. |
| **K2.1** | CSDL từ 5 bảng trở lên, chuẩn 3NF, có khóa ngoại, Unique constraint; script tạo bảng + seed data chạy thành công | 8 | `Scripts/CinemaBooking_SchemaAndSeed.sql` | 6 bảng quan hệ: `Movies`, `Auditoriums`, `Seats`, `Showtimes`, `Bookings`, `Tickets`. Có đầy đủ Foreign Keys và Unique Constraint `UQ_Tickets_Showtime_Seat`. Script chứa sẵn dữ liệu mẫu chạy thành công 100% trên SQL Server. |
| **K2.2** | Cặp Header–Line ghi trong cùng 1 Database Transaction (`IDbTransaction`) | 6 | `Plugins.DataStore.SqlServer/BookingSqlServerRepository.cs` (Dòng 34-94) | Ghi Header (`dbo.Bookings`) và các Lines (`dbo.Tickets`) trong cùng 1 `IDbTransaction`. Có `transaction.Commit()` khi hoàn tất và `transaction.Rollback()` khi xảy ra lỗi. |
| **K2.3** | 100% các câu lệnh SQL viết dạng Parameterized Query, không nối chuỗi SQL | 5 | `Plugins.DataStore.SqlServer/MovieSqlServerRepository.cs`<br>`Plugins.DataStore.SqlServer/ShowtimeSqlServerRepository.cs`<br>`Plugins.DataStore.SqlServer/BookingSqlServerRepository.cs`<br>`Plugins.DataStore.SqlServer/SeatSqlServerRepository.cs`<br>`Plugins.DataStore.SqlServer/AuditoriumSqlServerRepository.cs` | 100% câu truy vấn Dapper sử dụng tham số hóa (`@MovieId`, `@ShowtimeId`, `@SeatId`, `@BookingId`,...). Không có bất kỳ dòng nối chuỗi `$"SELECT` hoặc `+ "WHERE nào. |
| **K2.4** | Luồng giao dịch chính chạy end-to-end và dữ liệu nằm trong SQL Server | 6 | `WebApp/Components/Pages/BookingPage.razor`<br>`WebApp/Components/Pages/BookingList.razor` | Người dùng chọn phim -> chọn suất -> chọn ghế -> nhập thông tin khách hàng -> hệ thống ghi nhận vào bảng `Bookings` và `Tickets` trên SQL Server. Dữ liệu đối soát trực tiếp trong SSMS và trang `/admin/bookings`. |

---

### Hướng dẫn kiểm tra nhanh cho Giảng viên / Hội đồng:
1. **Chạy ứng dụng:**
   ```bash
   cd CinemaBooking/WebApp
   dotnet run
   ```
2. **Demo Luật 1 (Tranh chấp ghế):**
   - Vào menu **"🎟️ Đặt Vé (Luật 1 & TX)"** (`/booking/1`).
   - Bấm nút **"⚡ Demo Luật 1 (Cố tình đặt trùng)"** -> Hệ thống hiển thị thông báo lỗi màu đỏ thân thiện: *"Ghế ... trong suất chiếu này đã được khách hàng khác đặt. Vui lòng chọn ghế khác!"*
3. **Demo Luật 2 (Tránh trùng lịch phòng chiếu):**
   - Vào menu **"📅 Quản Lý Lịch (Luật 2)"** (`/admin/showtimes`).
   - Bấm nút **"⚡ Thử Trùng Lịch (Demo Luật 2)"** -> Hệ thống tự tính `Thời lượng phim + 15 phút dọn phòng` và chặn với thông báo: *"Phòng chiếu đã có suất chiếu khác trong khoảng thời gian này... Vui lòng chọn khung giờ khác!"*
4. **Kiểm tra Transaction Header-Line (K2.2 & K2.4):**
   - Đặt 1 vé với 2 ghế bất kỳ -> Vào menu **"📑 Đơn Đặt Vé (SQL)"** (`/admin/bookings`) hoặc mở SSMS truy vấn `SELECT * FROM dbo.Bookings; SELECT * FROM dbo.Tickets;` để thấy dữ liệu Header và Line được ghi đầy đủ và toàn vẹn.
