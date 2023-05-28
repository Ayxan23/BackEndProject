using System.Xml.Linq;

namespace BackEndProject.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var courses = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).OrderByDescending(c => c.ModifiedAt).ToListAsync();

            if (categoryId != null)
            {
                courses = await _context.Courses.Where(c => c.CourseCategories.Any(cc => cc.CategoryId == categoryId)).Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).OrderByDescending(c => c.ModifiedAt).ToListAsync();
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                courses = await _context.Courses.Where(c => c.Name.Contains(search)).OrderByDescending(c => c.ModifiedAt).ToListAsync();
            }


            return View(courses);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);

            if (course is null)
                return NotFound();

            CourseCategoryViewModel courseCategoryViewModel = new()
            {
                Course = course,
                Categories = await _context.Categories.Include(c => c.CourseCategories).ThenInclude(cc => cc.Course).ToListAsync(),
            };

            ViewBag.All = await _context.Courses.ToListAsync();

            return View(courseCategoryViewModel);
        }

    }
}
