using Microsoft.AspNetCore.Mvc;
using TravelExpenseTracker.Services;

namespace TravelExpenseTracker.ViewComponents
{
    public class TripsDropDownViewComponent : ViewComponent
    {
        private readonly ITripService _tripService;

        public TripsDropDownViewComponent(ITripService tripService)
        {
            _tripService = tripService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var UserId = GetCurrentUserId();
            var trips = await _tripService.GetAll();
            return View(trips.Take(10).OrderByDescending(t => t.StartDate).ToList());
        }
    }
}
