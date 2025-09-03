using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelExpenseTracker.Domain;
using TravelExpenseTracker.Services;

namespace TravelExpenseTracker.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly ITripService _tripService;
        private readonly IExpenseService _expenseService;
        private readonly IBlobService _blobService;
        private readonly ILogger<ExpensesController> _logger;

        public string UserID = "user@user.com";

        public ExpensesController(ITripService tripService, IExpenseService expenseService, IBlobService blobService, ILogger<ExpensesController> logger)
        {
            _tripService = tripService;
            _expenseService = expenseService;
            _logger = logger;
            _blobService = blobService;
        }

        // GET: ExpensesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExpensesController/Details/5
        public async Task<ActionResult> Details(string id, string tripId)
        {
            var expense = await _expenseService.GetExpenseById(id.ToString(), tripId );
            if (expense == null)  return NotFound();

            if (expense.TripId != null)
            {
                if(expense.BlobName != null)
                    expense.BlobUrl = await _blobService.GetFile(nameof(AzureBlobContainers.Receipts), expense.BlobName);
                
                var trip = await _tripService.GetTripById(expense.TripId, UserID);
                if (trip != null)
                {
                    ViewBag.TripName = trip.Name;
                    ViewBag.TripDates = $"{trip.StartDate:MMM dd} - {trip.EndDate:MMM dd}";
                }
            }
            return View(expense);
        }

        // GET: ExpensesController/Create
        public async Task<ActionResult> Create(string? tripid, ExpenseCategory? category)
        {
            var expense = new Expense { 
                ExpenseDate = DateTime.Now
            };

            if(category.HasValue) { expense.Category = category.Value; }

            if (tripid != null)
            {
                var trip = await _tripService.GetTripById(tripid.ToString(), UserID);
                if (trip != null)
                {
                    ViewBag.TripId = trip.Id;
                    ViewBag.TripName = trip.Name;
                    string dates = $"{trip.StartDate:MMM dd} - {trip.EndDate:MMM dd}";
                    ViewBag.TripDates = dates;
                }
            }

            return View(expense);
        }

        // POST: ExpensesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Expense expense, IFormFile? receiptFile, string? tripId)
        {
            if(receiptFile != null && receiptFile.Length > 0)
            {
                expense.BlobName = await _blobService.UploadFile(receiptFile, nameof(AzureBlobContainers.Receipts));
            }

            if(tripId != null) {expense.TripId = tripId;}

            if (ModelState.IsValid)
            {
                try
                {
                    await _expenseService.AddExpense(expense);
                    return RedirectToAction("Details", "Trips", new {id = tripId});
                }
                catch(Exception ex) 
                {
                    _logger.LogError(ex, "Error creating expense");
                    ModelState.AddModelError("", "There was an error creating the expesense");
                }
            }
            return View(expense);
        }

        // GET: ExpensesController/Edit/5
        public async Task<ActionResult> Edit(string id, string tripId)
        {
            var expense = await _expenseService.GetExpenseById(id.ToString(), tripId.ToString());
            
            if (expense == null) return NotFound();

            if (expense.TripId != null)
            {
                if(expense.BlobName != null)
                    expense.BlobUrl = await _blobService.GetFile(nameof(AzureBlobContainers.Receipts), expense.BlobName);
                
                var trip = await _tripService.GetTripById(expense.TripId.ToString(), UserID);
                if (trip != null)
                {
                    ViewBag.TripName = trip.Name;
                    ViewBag.TripDates = $"{trip.StartDate:MMM dd} - {trip.EndDate:MMM dd, yyyy}";
                }
            }
            return View(expense);
        }

        // POST: ExpensesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, IFormFile? receiptFile, Expense expense)
        {
            if (id != expense.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                 
                    try
                    {
                        if (receiptFile != null && receiptFile.Length > 0)
                        {
                            if(expense.BlobName != null)
                                await _blobService.DeleteFile(expense.BlobName, nameof(AzureBlobContainers.Receipts));
                            
                            expense.BlobName = await _blobService.UploadFile(receiptFile, nameof(AzureBlobContainers.Receipts));                           

                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting image {imagename}", expense.BlobName);
                        ModelState.AddModelError("", "There was an error removing image associated with this expense");
                    }
                    await _expenseService.UpdateExpense(expense);
                    return RedirectToAction("Details", "Trips", new {id = expense.TripId});
                }
                catch(Exception ex) 
                {
                    _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the expense.");
                    
                }
            }
            return View(expense);   
        }
             

        // POST: ExpensesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, string? tripId)
        {
            try
            {
                var result = await _expenseService.DeleteExpense(id.ToString(), tripId.ToString());
               
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {expenseId}", id);
               
            }
            return tripId != null ? RedirectToAction("Details", "Trips", new {id=tripId }) 
                                    : RedirectToAction("Index", "Home");
        }
    }
}
