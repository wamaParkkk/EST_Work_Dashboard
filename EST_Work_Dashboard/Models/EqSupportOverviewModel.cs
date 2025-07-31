namespace EST_Work_Dashboard.Models
{
    public class EqSupportOverviewModel
    {
        public int Id { get; set; }
        public string? Classification { get; set; }
        public string? CP { get; set; }
        public string? Manager { get; set; }
        public string? Process { get; set; }
        public string? Equipment { get; set; }
        public string? EquipmentNo { get; set; }
        public string? Down_Reason { get; set; }
        public string? Actions { get; set; }
        public string? Status { get; set; }
        public DateTime? Down_Date { get; set; }
        public DateTime? Recovery_Date { get; set; }
        public string? Down_Time { get; set; }
        public string? Technician { get; set; }
    }
}
