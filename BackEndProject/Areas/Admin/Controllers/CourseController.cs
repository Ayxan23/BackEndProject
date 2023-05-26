using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.OrderByDescending(p => p.ModifiedAt).ToListAsync();

            return View(courses);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel courseViewModel)
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            foreach (var categoryId in courseViewModel.CategoryIds)
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                    return BadRequest();
            }

            Course course = new()
            {
                Name = courseViewModel.Name,
                Description = courseViewModel.Description,
                Image = courseViewModel.Image,
                Start = courseViewModel.Start,
                Duration = courseViewModel.Duration,
                ClassDuration = courseViewModel.ClassDuration,
                SkillLevel = courseViewModel.SkillLevel,
                Language = courseViewModel.Language,
                Students = courseViewModel.Students,
                Price = courseViewModel.Price,
                IsDeleted = false
            };

            List<CourseCategory> courseCategories = new();
            foreach (var categoryId in courseViewModel.CategoryIds)
            {
                CourseCategory courseCategory = new()
                {
                    CourseId = courseViewModel.Id,
                    CategoryId = categoryId
                };
                courseCategories.Add(courseCategory);
            }

            course.CourseCategories = courseCategories;

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (course is null)
                return NotFound();

            return View(course);
        }

        public async Task<IActionResult> Update(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (course is null)
                return NotFound();

            ViewBag.Categories = _context.Categories.AsEnumerable();

            CourseViewModel courseViewModel = new()
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Image = course.Image,
                Start = course.Start,
                Duration = course.Duration,
                ClassDuration = course.ClassDuration,
                SkillLevel = course.SkillLevel,
                Language = course.Language,
                Students = course.Students,
                Price = course.Price,
            };

            return View(courseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CourseViewModel courseViewModel)
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            foreach (var categoryId in courseViewModel.CategoryIds)
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                    return BadRequest();
            }

            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (course is null)
                return NotFound();

            course.Name = courseViewModel.Name;
            course.Description = courseViewModel.Description;
            course.Image = courseViewModel.Image;
            course.Start = courseViewModel.Start;
            course.Duration = courseViewModel.Duration;
            course.ClassDuration = courseViewModel.ClassDuration;
            course.SkillLevel = courseViewModel.SkillLevel;
            course.Language = courseViewModel.Language;
            course.Students = courseViewModel.Students;
            course.Price = courseViewModel.Price;

            List<CourseCategory> courseCategories = new();
            foreach (var categoryId in courseViewModel.CategoryIds)
            {
                CourseCategory courseCategory = new()
                {
                    CourseId = courseViewModel.Id,
                    CategoryId = categoryId
                };
                courseCategories.Add(courseCategory);
            }

            course.CourseCategories = courseCategories;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (course is null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public async Task<IActionResult> DeletePost(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (course is null)
                return NotFound();

            course.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
