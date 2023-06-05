using BackEndProject.Areas.Admin.ViewModels;
using BackEndProject.Models;
using BackEndProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsLog = User.Identity.IsAuthenticated;

            var courses = await _context.Courses.OrderByDescending(c => c.ModifiedAt).Take(3).ToListAsync();
            var events = await _context.Events.OrderByDescending(e => e.StartTime).Take(4).ToListAsync();
            var blogs = await _context.Blogs.OrderByDescending(b => b.CreatedAt).Take(3).ToListAsync();

            HomeViewModel homeViewModel = new()
            {
                Courses = courses,
                Events = events,
                Blogs = blogs
            };

            return View(homeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HomeViewModel homeViewModel)
        {
            ViewBag.IsLog = User.Identity.IsAuthenticated;
            homeViewModel.Courses = await _context.Courses.OrderByDescending(c => c.ModifiedAt).Take(3).ToListAsync();
            homeViewModel.Events = await _context.Events.OrderByDescending(e => e.StartTime).Take(4).ToListAsync();
            homeViewModel.Blogs = await _context.Blogs.OrderByDescending(b => b.CreatedAt).Take(3).ToListAsync();

            if (!ModelState.IsValid && !User.Identity.IsAuthenticated)
                return View(homeViewModel);

            Subscribe subscribe = new();
            var userName = HttpContext?.User?.Identity?.Name;

            if (userName != null)
            {
                var user = await _userManager.FindByNameAsync(userName);
                subscribe.Email = user.Email;
            }
            else if (homeViewModel.SubscribeViewModel.Email == null)
            {
                ModelState.AddModelError("SubscribeViewModel.Email", "The Email field is required");
                return View(homeViewModel);
            }
            else
            {
                subscribe.Email = homeViewModel.SubscribeViewModel.Email;
            }

            if (await _context.Subscribes.AnyAsync(s => s.Email == subscribe.Email))
            {
                ModelState.AddModelError("SubscribeViewModel.Email", "This Email already exist");
                return View(homeViewModel);
            }

            await _context.Subscribes.AddAsync(subscribe);
            await _context.SaveChangesAsync();

            return View(homeViewModel);
        }
    }
    
}
