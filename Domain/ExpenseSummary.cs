namespace TravelExpenseTracker.Domain
{
    public class ExpenseSummary
    {
        public decimal TotalAmount { get; set; }
        public int TotalExpenses { get; set; }
        public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
    }
}
