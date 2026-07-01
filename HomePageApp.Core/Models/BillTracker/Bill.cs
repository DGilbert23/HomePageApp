namespace HomePageApp.Core.Models.BillTracker
{
    public class Bill
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Reoccurring { get; set; } = true;
        public string? Frequency { get; set; }
        public DateTime StartDue { get; set; }
        public DateTime? NextDue { get; set; }
        public DateTime? LastPaid { get; set; }
        public double? EstimatedAmountDue { get; set; } = 999.999;
        public string? PaymentUrl { get; set; }
        public bool AutoDraft { get; set; }
    }
}
