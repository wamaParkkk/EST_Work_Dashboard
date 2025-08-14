namespace EST_Work_Dashboard.Models
{
    // Dropdown용 설정
    public class EquipmentListConfig
    {
        public List<string> equipmentTypes { get; set; } = new();
    }

    public class LineListConfig
    {
        public List<string> lineCodes { get; set; } = new();
    }

    // FTP 설정
    public class FtpConfig
    {
        public string host { get; set; } = "";
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string rootFolder { get; set; } = "MTBF Log";
    }

    // CSV 한 줄 구조 (헤더명은 서비스에서 유연하게 매핑)
    public class MtbfCsvRow
    {
        public DateTime Timestamp { get; set; }     // 이벤트 시간
        public string AssetNo { get; set; } = "";   // 자산번호
        public string Model { get; set; } = "";     // 모델
        public string LotNo { get; set; } = "";     // 라트번호
        public string EventType { get; set; } = ""; // RUN/ALARM 등 이벤트
        public int? OutputQty { get; set; }         // 생산 수량
    }

    // 라트별 요약 결과
    public class LotSummary
    {
        public string LotNo { get; set; } = "";
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public TimeSpan RunDuration { get; set; }     // 가동 시간 합계
        public TimeSpan AlarmDuration { get; set; }   // 알람 시간 합계
        //public TimeSpan NetRun => RunDuration - AlarmDuration; // 실가동(단순차)                                                      
        public TimeSpan NetRun
            => (RunDuration > AlarmDuration) ? (RunDuration - AlarmDuration) : TimeSpan.Zero;
        public int TotalOutput { get; set; }          // 생산량 합
        public int AlarmCount { get; set; }           // 알람 횟수
    }
}
