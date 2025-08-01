namespace EST_Work_Dashboard.Models
{
    public class LayOutModel
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CP { get; set; }
        public string? Manager { get; set; }
        public string? CR { get; set; }
        public string? Project_Name { get; set; }
        public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? Process { get; set; }
        public string? Eq_Manufacturer { get; set; }
        public string? Model_Name { get; set; }
        public string? Classification { get; set; }
        public string? Remark { get; set; }
    }
}
