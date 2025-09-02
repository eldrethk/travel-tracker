using TravelExpenseTracker.Domain;

namespace TravelExpenseTracker.Models
{
    public class TripViewModel
    {
        public Trip Trip { get; set; } = new Trip();
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public ExpenseSummary ExpenseSummary { get; set; } = new ExpenseSummary();

        public static Dictionary<string, string> CategoryClasses => new()
        {
            { "Accommodation", "bg-accom" },
            { "Transportation", "bg-trans" },
            { "Meals", "bg-meals" },
            { "Entertainment", "bg-entert" },
            { "Business", "bg-business" },
            { "Other", "bg-other" }
        };

        public class CategorySummary
        {
            public string Category { get; set; }
            public decimal Amount { get; set; }
            public int Count { get; set; }
        }


        public List<CategorySummary> GetAllCategoriesWithSummary()
        {
            return Enum.GetValues<ExpenseCategory>()
                .Select(cat => new CategorySummary
                {
                    Category = cat.ToString(),
                    Amount = ExpenseSummary.CategoryBreakdown.GetValueOrDefault(cat.ToString(), 0m),
                    Count = ExpenseSummary.CategoryCounts.GetValueOrDefault(cat.ToString(), 0)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();
        }

    }
}
