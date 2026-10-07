-- ========================================================================
-- CINEMA BOOKING DATABASE - SCHEMA & SEED DATA (ECO2415 - Capstone Project)
-- Dat chuan 3NF, khoa ngoai va Unique Constraint ngan tranh chap ghe
-- ========================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CinemaBookingDB')
BEGIN
    CREATE DATABASE CinemaBookingDB;
END
GO

USE CinemaBookingDB;
GO

-- 1. XOA CAC BANG NEU DA TON TAI (Theo thu tu phu thuoc khoa ngoai)
IF OBJECT_ID('dbo.Tickets', 'U') IS NOT NULL DROP TABLE dbo.Tickets;
IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.Showtimes', 'U') IS NOT NULL DROP TABLE dbo.Showtimes;
IF OBJECT_ID('dbo.Seats', 'U') IS NOT NULL DROP TABLE dbo.Seats;
IF OBJECT_ID('dbo.Auditoriums', 'U') IS NOT NULL DROP TABLE dbo.Auditoriums;
IF OBJECT_ID('dbo.Movies', 'U') IS NOT NULL DROP TABLE dbo.Movies;
GO

-- 2. TAO CAC BANG

-- Bang 1: Phim (Movies)
CREATE TABLE dbo.Movies (
    MovieId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(250) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DurationMinutes INT NOT NULL,
    PosterUrl NVARCHAR(500) NULL,
    Genre NVARCHAR(100) NOT NULL,
    ReleaseDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Bang 2: Phong chieu (Auditoriums)
CREATE TABLE dbo.Auditoriums (
    AuditoriumId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    TotalSeats INT NOT NULL DEFAULT 0
);
GO

-- Bang 3: Ghe ngoi (Seats)
CREATE TABLE dbo.Seats (
    SeatId INT IDENTITY(1,1) PRIMARY KEY,
    AuditoriumId INT NOT NULL,
    SeatNumber NVARCHAR(10) NOT NULL,
    [Row] NVARCHAR(5) NOT NULL,
    Number INT NOT NULL,
    SeatType NVARCHAR(50) NOT NULL DEFAULT 'Standard',
    CONSTRAINT FK_Seats_Auditoriums FOREIGN KEY (AuditoriumId) 
        REFERENCES dbo.Auditoriums(AuditoriumId) ON DELETE CASCADE
);
GO

-- Bang 4: Suat chieu (Showtimes)
CREATE TABLE dbo.Showtimes (
    ShowtimeId INT IDENTITY(1,1) PRIMARY KEY,
    MovieId INT NOT NULL,
    AuditoriumId INT NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NOT NULL, -- StartTime + DurationMinutes + 15 phut don phong
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Showtimes_Movies FOREIGN KEY (MovieId) 
        REFERENCES dbo.Movies(MovieId),
    CONSTRAINT FK_Showtimes_Auditoriums FOREIGN KEY (AuditoriumId) 
        REFERENCES dbo.Auditoriums(AuditoriumId)
);
GO

-- Bang 5: Don dat ve - Header (Bookings)
CREATE TABLE dbo.Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    ShowtimeId INT NOT NULL,
    CustomerName NVARCHAR(150) NOT NULL,
    CustomerEmail NVARCHAR(150) NOT NULL,
    CustomerPhone NVARCHAR(50) NOT NULL,
    BookingTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Confirmed',
    CONSTRAINT FK_Bookings_Showtimes FOREIGN KEY (ShowtimeId) 
        REFERENCES dbo.Showtimes(ShowtimeId)
);
GO

-- Bang 6: Ve - Line (Tickets)
-- Bao gom UNIQUE (ShowtimeId, SeatId) de dam bao Luat nghiep vu 1 o cap CSDL
CREATE TABLE dbo.Tickets (
    TicketId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    ShowtimeId INT NOT NULL,
    SeatId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Tickets_Bookings FOREIGN KEY (BookingId) 
        REFERENCES dbo.Bookings(BookingId) ON DELETE CASCADE,
    CONSTRAINT FK_Tickets_Showtimes FOREIGN KEY (ShowtimeId) 
        REFERENCES dbo.Showtimes(ShowtimeId),
    CONSTRAINT FK_Tickets_Seats FOREIGN KEY (SeatId) 
        REFERENCES dbo.Seats(SeatId),
    -- LUAT 1 CAP CSDL: 1 ghe trong 1 suat chieu chi duoc ban 1 lan duy nhat
    CONSTRAINT UQ_Tickets_Showtime_Seat UNIQUE (ShowtimeId, SeatId)
);
GO

-- 3. CHEN DU LIEU MAU (SEED DATA)
SET IDENTITY_INSERT dbo.Movies ON;
INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, PosterUrl, Genre, ReleaseDate) VALUES
(1, N'Dune: Hành Tinh Cát - Phần Hai', N'Paul Atreides hợp lực cùng Chani và người Fremen để trả thù những kẻ đã hủy hoại gia đình mình.', 166, N'https://images.unsplash.com/photo-1534447677768-be436bb09401?w=600&auto=format&fit=crop&q=80', N'Khoa học viễn tưởng / Phiêu lưu', DATEADD(day, -10, GETDATE())),
(2, N'Kung Fu Panda 4', N'Po được chọn trở thành Thủ lĩnh Tinh thần của Thung lũng Bình Yên và phải tìm kiếm một Chiến binh Rồng mới.', 94, N'https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=600&auto=format&fit=crop&q=80', N'Hoạt hình / Hài hước', DATEADD(day, -5, GETDATE())),
(3, N'Godzilla x Kong: Đế Chế Mới', N'Hai titan cổ đại Godzilla và Kong bước vào cuộc chiến sinh tử chống lại một mối đe dọa khổng lồ ẩn sâu trong Trái Đất.', 115, N'https://images.unsplash.com/photo-1518676590629-3dcbd9c5a5c9?w=600&auto=format&fit=crop&q=80', N'Hành động / Viễn tưởng', GETDATE());
SET IDENTITY_INSERT dbo.Movies OFF;
GO

SET IDENTITY_INSERT dbo.Auditoriums ON;
INSERT INTO dbo.Auditoriums (AuditoriumId, Name, TotalSeats) VALUES
(1, N'Phòng Chiếu 01 (IMAX Laser)', 24),
(2, N'Phòng Chiếu 02 (Standard Dolby Atmos)', 24);
SET IDENTITY_INSERT dbo.Auditoriums OFF;
GO

-- Sinh so do 24 ghe cho Phong 1 (A1->A8, B1->B8, C1->C8)
DECLARE @r NVARCHAR(5), @n INT, @aud INT = 1;
DECLARE cur_row CURSOR FOR SELECT 'A' UNION ALL SELECT 'B' UNION ALL SELECT 'C';
OPEN cur_row;
FETCH NEXT FROM cur_row INTO @r;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @n = 1;
    WHILE @n <= 8
    BEGIN
        INSERT INTO dbo.Seats (AuditoriumId, SeatNumber, [Row], Number, SeatType)
        VALUES (@aud, @r + CAST(@n AS NVARCHAR(5)), @r, @n, CASE WHEN @r = 'A' THEN 'Standard' ELSE 'VIP' END);
        SET @n = @n + 1;
    END
    FETCH NEXT FROM cur_row INTO @r;
END
CLOSE cur_row;
DEALLOCATE cur_row;
GO

-- Sinh so do 24 ghe cho Phong 2
DECLARE @r2 NVARCHAR(5), @n2 INT, @aud2 INT = 2;
DECLARE cur_row2 CURSOR FOR SELECT 'A' UNION ALL SELECT 'B' UNION ALL SELECT 'C';
OPEN cur_row2;
FETCH NEXT FROM cur_row2 INTO @r2;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @n2 = 1;
    WHILE @n2 <= 8
    BEGIN
        INSERT INTO dbo.Seats (AuditoriumId, SeatNumber, [Row], Number, SeatType)
        VALUES (@aud2, @r2 + CAST(@n2 AS NVARCHAR(5)), @r2, @n2, CASE WHEN @r2 = 'A' THEN 'Standard' ELSE 'VIP' END);
        SET @n2 = @n2 + 1;
    END
    FETCH NEXT FROM cur_row2 INTO @r2;
END
CLOSE cur_row2;
DEALLOCATE cur_row2;
GO

-- Seed Suat chieu mau hop le
DECLARE @TodayStart DATETIME2 = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), DAY(GETDATE()));
INSERT INTO dbo.Showtimes (MovieId, AuditoriumId, StartTime, EndTime, Price)
VALUES 
(1, 1, DATEADD(hour, 9, @TodayStart), DATEADD(minute, 181, DATEADD(hour, 9, @TodayStart)), 110000),
(2, 1, DATEADD(hour, 13, @TodayStart), DATEADD(minute, 109, DATEADD(hour, 13, @TodayStart)), 85000),
(3, 2, DATEADD(hour, 10, @TodayStart), DATEADD(minute, 130, DATEADD(hour, 10, @TodayStart)), 95000);
GO
