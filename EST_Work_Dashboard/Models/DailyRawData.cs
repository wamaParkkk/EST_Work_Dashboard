namespace EST_Work_Dashboard.Models
{
    public class DailyRawData
    {
        // ?는 입력값이 없어도 서버에서 거르지 않고 저장
        // Edit.cshtml.cs의 OnPostAsync()함수에서 ModelState.IsValid = false여도 서버에 저장됨
        public string? ww { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CP { get; set; }
        public string? Manager { get; set; }
        public string? Classification { get; set; }
        public string? Site { get; set; }
        public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? Process { get; set; }
        public string? Eq_Manufacturer { get; set; }
        public string? Model_Name { get; set; }
        public string? MC { get; set; }
        public string? Problem_Description { get; set; }
        public string? Actions { get; set; }
        public string? Cause { get; set; }
        public string? Status { get; set; }
        public string? TTL_Qty { get; set; }
        public string? Done_Qty { get; set; }
        public string? Remark { get; set; }
        public string? Qty { get; set; }
        public string? Outsourced_Cost { get; set; }
        public string? Inhouse_Cost { get; set; }
        public string? Cost_Save { get; set; }
        public string? TTL_Cost_Save { get; set; }
    }
}
