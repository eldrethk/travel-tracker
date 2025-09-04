using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TravelExpenseTracker.Domain
{
    public class Expense
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Foreign key to Trip
        [Display(Name = "Trip")]
        [JsonProperty("tripId")]
        public string? TripId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Company's Name")]
        [JsonProperty("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [JsonProperty("description")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Category")]
        [JsonProperty("category")]
        public ExpenseCategory Category { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expense Date")]
        [JsonProperty("expenseDate")]
        public DateTime ExpenseDate { get; set; }

        [Display(Name = "Receipt")]
        [JsonProperty("blobName")]
        public string? BlobName { get; set; }

        [JsonIgnore]
        public string? BlobUrl { get; set; }

        [Display(Name = "Guest Names")]
        [JsonProperty("guestNames")]
        public string? GuestNames { get; set; }

        [Display(Name = "Created")]
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("TripId")]
        [JsonIgnore]
        public virtual Trip? Trip { get; set; }
    }

    public enum ExpenseCategory
    {
        [Display(Name = "Accommodation")]
        Accommodation = 1,

        [Display(Name = "Transportation")]
        Transportation = 2,

        [Display(Name = "Meals")]
        Meals = 3,

        [Display(Name = "Entertainment")]
        Entertainment = 4,

        [Display(Name = "Other")]
        Other = 5
    }
}
