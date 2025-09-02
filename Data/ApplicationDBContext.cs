using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using TravelExpenseTracker.Domain;

namespace TravelExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the one-to-many relationship between Trip and Expense
            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Expenses)
                .WithOne(e => e.Trip)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.Cascade); // Set TripId to null when a Trip is deleted

            // Configure Expense entity
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                //entity.Property(e => e.Description).HasMaxLength(500);

                // Configure relationship with Trip
                entity.HasOne(e => e.Trip)
                      .WithMany(t => t.Expenses)
                      .HasForeignKey(e => e.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Trip entity
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.UserId).IsRequired().HasMaxLength(450);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Description).HasMaxLength(500);
                entity.Property(t => t.Destination).HasMaxLength(100);
               
                // Index for better query performance
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.StartDate);
            });

            // Add some seed data
            modelBuilder.Entity<Trip>().HasData(
                new Trip
                {
                    Id = 1,
                    UserId = "demo@user.com",
                    Name = "Europe Business Trip",
                    Description = "Quarterly business meetings in London and Paris",
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(-3),
                    Destination = "London, Paris",
                    
                },
                new Trip
                {
                    Id = 2,
                    UserId = "demo@user.com",
                    Name = "NYC Conference",
                    Description = "Tech conference in New York",
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(-1),
                    Destination = "New York, NY",
                  
                },
                new Trip
                {
                    Id = 3,
                    UserId = "demo@user.com",
                    Name = "West Coast Sales Trip",
                    Description = "Client meetings in San Francisco and Seattle",
                    StartDate = DateTime.Now.AddDays(5),
                    EndDate = DateTime.Now.AddDays(12),
                    Destination = "San Francisco, Seattle",
                 
                }
            );

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                   
                    TripId = 1,
                    Description = "Hotel in Paris",
                    Amount = 150.00m,
                    Category = ExpenseCategory.Accommodation,
                    ExpenseDate = DateTime.Now.AddDays(-5),
                   
                },
                new Expense
                {
                    Id = 2,
                    TripId = 1,
                    Description = "Flight to London",
                    Amount = 350.00m,
                    Category = ExpenseCategory.Transportation,
                    ExpenseDate = DateTime.Now.AddDays(-7),
                  
                },
                new Expense
                {
                    Id = 3,
                    TripId = 2,
                    Description = "Business Dinner",
                    Amount = 85.50m,
                    Category = ExpenseCategory.Meals,
                    ExpenseDate = DateTime.Now.AddDays(-3),
                  
                },
                new Expense
                {
                    Id = 4,
                    TripId = 2,
                    Description = "Conference Registration",
                    Amount = 499.00m,
                    Category = ExpenseCategory.Other,
                    ExpenseDate = DateTime.Now.AddDays(-4),
                
                },
                     new Expense
                     {
                         Id = 6,
                         TripId = 2,
                         Description = "Conference Registration",
                         Amount = 200.00m,
                         Category = ExpenseCategory.Other,
                         ExpenseDate = DateTime.Now.AddDays(-4),

                     },
                new Expense
                {
                    Id = 5,
                    TripId = 1, // Expense not associated with a trip
                    Description = "Office Supplies",
                    Amount = 45.75m,
                    Category = ExpenseCategory.Other,
                    ExpenseDate = DateTime.Now.AddDays(-1),
              
                }
            );
        }*/
    }
}
