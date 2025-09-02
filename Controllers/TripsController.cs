using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelExpenseTracker.Models;
using TravelExpenseTracker.Services;
using TravelExpenseTracker.Domain;

namespace TravelExpenseTracker.Controllers
{   
    public class TripsController : Controller
    {
        private readonly ILogger<TripsController> _logger;
        private readonly ITripService _tripService;
        private readonly IExpenseService _expenseService;

        public string UserID = "user@user.com";

        public TripsController(ITripService tripService, ILogger<TripsController> logger, IExpenseService expenseService)
        {
            _logger = logger;
            _tripService = tripService;
            _expenseService = expenseService;
        }
        // GET: TripsController
        public async Task<ActionResult> Index()
        {
            var trips = await _tripService.GetAll();
            var tripViewModel = new List<TripViewModel>();
            foreach (var trip in trips)
            {
                tripViewModel.Add(new TripViewModel
                {
                    Trip = trip,
                    ExpenseSummary = await _expenseService.GetExpenseSummary(trip.Id.ToString())
                });
            }
            return View(tripViewModel);
        }

        // GET: TripsController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            TripViewModel model = new TripViewModel
            {
                Trip = await _tripService.GetTripById(id.ToString(), UserID),
                Expenses = await _expenseService.GetExpensesByTripId(id.ToString()),
                ExpenseSummary = await _expenseService.GetExpenseSummary(id.ToString())
            };
            
            return View(model);
        }

        // GET: TripsController/Create
        public ActionResult Create()
        {
            var trip = new Trip
            {                
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7)
            };
            return View(trip);

        }

        // POST: TripsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Trip trip)
        {
            trip.UserId = UserID;
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            { 
                try
                {
                    //need to assign currentuser() to UserId
                    await _tripService.AddTrip(trip);
                    return RedirectToAction("Index", "Home");                        
                }
               catch(Exception ex) 
                {
                    _logger.LogError(ex, "Error creating a trip");
                    ModelState.AddModelError("", "An error occurred while creating a trip. Please try again.");
                }      
            }
            return View(trip);
        }

        // GET: TripsController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {           
            Trip trip = await _tripService.GetTripById(id, UserID);
            
            if (trip == null)
                return NotFound();
            
            return View(trip);
        }

        // POST: TripsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Trip trip)
        {
            if (id != trip.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _tripService.UpdateTrip(trip);
                    return RedirectToAction("Details", new {id = trip.Id});
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error updating trip {Id}", id);
                    ModelState.AddModelError("", "An error occurred while updating the trip.");
                }
            }
            return View(trip);
        }


        // POST: TripsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _tripService.DeleteTrip(id, UserID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Trip {tripId}", id);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
