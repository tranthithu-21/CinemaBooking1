namespace CoreBusiness.Exceptions
{
    public class ShowtimeOverlapException : Exception
    {
        public int AuditoriumId { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        public ShowtimeOverlapException(int auditoriumId, DateTime startTime, DateTime endTime, string message)
            : base(message)
        {
            AuditoriumId = auditoriumId;
            StartTime = startTime;
            EndTime = endTime;
        }

        public ShowtimeOverlapException(int auditoriumId, DateTime startTime, DateTime endTime)
            : base($"Phòng chiếu {auditoriumId} đã có suất chiếu khác trong khoảng thời gian {startTime:HH:mm dd/MM/yyyy} - {endTime:HH:mm dd/MM/yyyy} (đã tính kèm 15 phút dọn phòng). Vui lòng chọn khung giờ khác!")
        {
            AuditoriumId = auditoriumId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
