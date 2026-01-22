using IkinciElSatis.Data;
using IkinciElSatis.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IkinciElSatis.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string description)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();

            var log = new AdminLog
            {
                AdminId = userId,
                Action = action,
                Description = description,
                IpAddress = ip,
                Date = DateTime.Now
            };

            _context.AdminLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}