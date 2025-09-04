using Microsoft.EntityFrameworkCore;
using System.Security;
using TravelExpenseTracker.Data;
using TravelExpenseTracker.Domain;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace TravelExpenseTracker.Services
{
    public interface ITripService
    {
        Task<Trip> AddTrip(Trip trip);
        Task<Trip?> GetTripById(string id);
        Task<List<Trip>> GetTripsByUserId();
        //Task<List<Trip>> GetAll();
        Task<bool> UpdateTrip(Trip trip);
        Task<bool> DeleteTrip(string id);
        //Task<ExpenseSummary> GetExpenseSummary(int tripId);
    }
    public class TripService : ITripService
    {
        private readonly Container _tripContainer;
        private readonly IExpenseService _expenseService;
        private readonly IUserService _userService;
        private readonly ILogger<TripService> _logger;
        public TripService(CosmosClient cosmosClient, IUserService userService, IExpenseService expenseService, ILogger<TripService> logger)
        {
            _tripContainer = cosmosClient.GetContainer("travelapp", "trips");
            _userService = userService;
            _expenseService = expenseService;
            _logger = logger;
        }
        public async Task<Trip> AddTrip(Trip trip)
        {
            try
            {
                trip.UserId = _userService.GetCurrentUserId();
                var response = await _tripContainer.CreateItemAsync(trip, new PartitionKey(trip.UserId));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message, "CosmosDB error: statuscode= {StatusCode}, Message={Message}");
                return trip;
            
            }

        }
        public async Task<Trip?> GetTripById(string id)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                var response = await _tripContainer.ReadItemAsync<Trip>(id, new PartitionKey(userId));
                var trip = response.Resource;

                trip.Expenses = await _expenseService.GetExpensesByTripId(id);
                return trip;
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        public async Task<List<Trip>> GetTripsByUserId()
        {
            var userId = _userService.GetCurrentUserId();
            var trips = new List<Trip>();

            if (string.IsNullOrEmpty(userId))
                return trips;

            var query = new QueryDefinition("Select * from c Where c.userId = @userId")
                .WithParameter("@userId", userId);
            try
            {
                var iterator = _tripContainer.GetItemQueryIterator<Trip>(query);

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    trips.AddRange(response);
                }
            }
            catch(CosmosException ex)
            {
                _logger.LogError(ex.Message);
            }

            return trips;
        }
        /*public async Task<List<Trip>> GetAll()
        {
            var trips = new List<Trip>();
            var query = new QueryDefinition("Select * from c");
               
            try
            {
                var iterator = _tripContainer.GetItemQueryIterator<Trip>(query);

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    trips.AddRange(response);
                }
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
            }

            return trips;
        }*/
        public async Task<bool> UpdateTrip(Trip trip)
        {
            try
            {
                trip.UserId = _userService.GetCurrentUserId();
                var response = await _tripContainer.UpsertItemAsync(trip, new PartitionKey(trip.UserId));
                return true;
            }
            catch(CosmosException ex)
            {
                _logger.LogError(ex.Message); 
                return false;
            }
        }
        public async Task<bool> DeleteTrip(string id)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                ItemResponse<object> resp =
                await _tripContainer.DeleteItemAsync<object>(id, new PartitionKey(userId));
                return resp.StatusCode == HttpStatusCode.NoContent;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
      
     
    }
}
