using TravelExpenseTracker.Data;
using TravelExpenseTracker.Domain;
using Microsoft.Azure.Cosmos;
using System.Net;
using Microsoft.Azure.Cosmos.Core.Collections;

namespace TravelExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<Expense> AddExpense(Expense expense);
        Task<Expense?> GetExpenseById(string id, string tripId);
        Task<List<Expense>> GetExpensesByTripId(string tripId);
        Task<bool> UpdateExpense(Expense expense);
        Task<bool> DeleteExpense(string id, string tripId);
        Task<ExpenseSummary> GetExpenseSummary(string tripId);

    }
    public class ExpenseService : IExpenseService
    {
        private readonly Container _expenseContainer;
        private readonly ILogger<ExpenseService> _logger;
        public ExpenseService(CosmosClient cosmosClient, ILogger<ExpenseService> logger)
        {
            _expenseContainer = cosmosClient.GetContainer("travelapp", "expenses");
            _logger = logger;
        }
        public async Task<Expense> AddExpense(Expense expense)
        {
            try
            {
                var response = await _expenseContainer.CreateItemAsync(expense, new PartitionKey(expense.TripId));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, "CosmosDB error: statuscode= {StatusCode}, Message={Message}");
                throw;
                //return expense;
            }
        }
        public async Task<Expense?> GetExpenseById(string id, string tripId)
        {
            try
            {
                var response = await _expenseContainer.ReadItemAsync<Expense>(id, new PartitionKey(tripId));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        public async Task<List<Expense>> GetExpensesByTripId(string tripId)
        {
            var expenses = new List<Expense>();
            var query = new QueryDefinition("Select * from c Where c.tripId = @tripId")
                .WithParameter("@tripId", tripId);

            try
            {
                var iterator = _expenseContainer.GetItemQueryIterator<Expense>(query);
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    expenses.AddRange(response);
                }
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
            }
            return expenses;
        }
        public async Task<bool> UpdateExpense(Expense expense)
        {
            try
            {
                var response = await _expenseContainer.UpsertItemAsync(expense, new PartitionKey(expense.TripId));
                return true;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteExpense(string id, string tripId)
        {
            try
            {
                ItemResponse<object> response =
                    await _expenseContainer.DeleteItemAsync<object>(id, new PartitionKey(tripId));
                return response.StatusCode == HttpStatusCode.NoContent;
            }
            catch(CosmosException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<ExpenseSummary> GetExpenseSummary(string tripId)
        {
            decimal total = 0m;
            int totalCount = 0;
            var byCatAmount = new Dictionary<string, decimal>();
            var byCatCount = new Dictionary<string, int>();
            var query = new QueryDefinition("Select c.amount, c.category from c where c.tripId = @tripId")
                .WithParameter("@tripId", tripId);

            try
            {
                var iterator = _expenseContainer.GetItemQueryIterator<dynamic>(query);
                while (iterator.HasMoreResults)
                {
                    var page = await iterator.ReadNextAsync();
                    foreach( var row in page)
                    {
                        decimal amount = (decimal)row.amount;
                        string category = (string)row.category;

                        total += amount;
                        totalCount++;
                        
                        if (byCatAmount.ContainsKey(category))
                        {
                            byCatAmount[category] += amount;
                            byCatCount[category]++;
                        }
                        else
                        {
                            byCatAmount[category] = amount;
                            byCatCount[category] = 1;
                        }

                    }
                }            
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
            }
            return new ExpenseSummary
            {
                TotalAmount = total,
                TotalExpenses = totalCount,
                CategoryBreakdown = byCatAmount,
                CategoryCounts = byCatCount
            };
        }
    }
}
