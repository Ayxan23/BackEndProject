using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Data;

namespace BackEndProject.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            var userName = HttpContext?.User?.Identity?.Name;
            ViewBag.UserName = userName;
            var isLog = User.Identity.IsAuthenticated;
            ViewBag.IsLog = isLog;
            var adminAccess = User.IsInRole("Member");
            ViewBag.AdminAccess = adminAccess;

            return View(settings);

        }
    }
}

