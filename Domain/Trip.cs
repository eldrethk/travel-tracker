using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace TravelExpenseTracker.Domain
{
    public class Trip
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "User ID")]
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Trip Name")]
        [JsonProperty("tripName")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        [JsonProperty("description")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Destination")]
        [JsonProperty("destination")]
        public string? Destination { get; set; }

        [Display(Name = "Created")]
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [JsonIgnore]
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

        // Calculated properties
        [JsonIgnore]
        public decimal TotalExpenses => Expenses?.Sum(e => e.Amount) ?? 0;

    }
}
