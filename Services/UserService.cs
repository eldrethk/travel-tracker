using System.Security.Claims;

namespace TravelExpenseTracker.Services
{
    public interface IUserService
    {
        string GetCurrentUserId();
        string GetCurrentUserEmail();
        string GetCurrentUserName();
    }
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true) return null;

            /*return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value
                ?? user?.FindFirst("oid")?.Value
                ?? throw new InvalidOperationException("User not authenticated");*/
            return user?.FindFirst(ClaimTypes.Email)?.Value
                ?? user?.FindFirst("email")?.Value
                ?? user?.FindFirst("preferred_username")?.Value
                ?? throw new InvalidOperationException("User not authenticated");

        }

        public string GetCurrentUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Email)?.Value
                ?? user?.FindFirst("email")?.Value
                ?? user?.FindFirst("preferred_username")?.Value
                ?? "unknown@example.com";
        }

        public string GetCurrentUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.FindFirst("name")?.Value
                ?? "Unknown User";
        }
    }
}
