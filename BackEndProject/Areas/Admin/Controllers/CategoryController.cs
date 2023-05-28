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
            var categories = await _context.Categories.OrderByDescending(c => c.ModifiedAt).ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            if (await _context.Courses.AnyAsync(c => c.Name == categoryViewModel.Name))
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


        public async Task<IActionResult> Detail(int id)
        {
            var category = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            return View(category);
        }


        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            CategoryViewModel categoryViewModel = new()
            {
                Id = category.Id,
                Name = category.Name,
            };

            return View(categoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var category = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            if (await _context.Courses.AnyAsync(c => c.Name == categoryViewModel.Name && c.Name != category.Name))
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            category.Name = categoryViewModel.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public async Task<IActionResult> DeletePost(int id)
        {
            var category = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            category.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
