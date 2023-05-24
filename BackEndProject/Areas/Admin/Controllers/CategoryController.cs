using Microsoft.AspNetCore.Mvc;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            bool isExist = _context.Categories.Any(t => t.Name == categoryViewModel.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            Category category = new()
            {
                Name = categoryViewModel.Name,
                IsDeleted = false,
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
}
