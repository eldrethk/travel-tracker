using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelExpenseTracker.Domain;
using TravelExpenseTracker.Models;
using TravelExpenseTracker.Services;

namespace TravelExpenseTracker.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITripService _tripService;
    private readonly IExpenseService _expenseService;

    public HomeController(ILogger<HomeController> logger, ITripService tripService, IExpenseService expenseService)
    {
        _logger = logger;
        _tripService = tripService;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        List<TripViewModel> list = new List<TripViewModel>();

        List<Trip> triplist = await _tripService.GetTripsByUserId();
        foreach (Trip trip in triplist) {
            list.Add(new TripViewModel
            {
                Trip = trip,
                ExpenseSummary = await _expenseService.GetExpenseSummary(trip.Id.ToString()),
                
            });
        }
       
        return View(list);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
