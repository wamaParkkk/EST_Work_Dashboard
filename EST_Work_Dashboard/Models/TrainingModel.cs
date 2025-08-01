namespace EST_Work_Dashboard.Models
{
    public class TrainingModel
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CP { get; set; }
        public string? Manager { get; set; }
        public string? Site { get; set; }        
        public string? Plant { get; set; }        
        public string? Process { get; set; }
        public string? Eq_Manufacturer { get; set; }
        public string? Model_Name { get; set; }
        public string? Training_Content { get; set; }
        public string? Trainer { get; set; }
        public string? Trainee { get; set; }
        public string? Remark { get; set; }
    }
}
