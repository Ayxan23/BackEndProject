

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.OrderByDescending(c => c.ModifiedAt).ToListAsync();

            return View(courses);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(CourseViewModel courseViewModel)
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (courseViewModel.CategoryIds == null)
            {
                ModelState.AddModelError("CategoryIds", "The Categories field is required");
                return View();
            }

            foreach (var categoryId in courseViewModel.CategoryIds)
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                    return BadRequest();
            }

            if (await _context.Courses.AnyAsync(c => c.Name == courseViewModel.Name))
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            if (courseViewModel.Image == null)
            {
                ModelState.AddModelError("Image", "Image bosh ola bilmez");
                return View();
            }
            if (!courseViewModel.Image.CheckFileSize(300))
            {
                ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                return View();
            }
            if (!courseViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
            {
                ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                return View();
            }

            string fileName = $"{Guid.NewGuid()}-{courseViewModel.Image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "course", fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await courseViewModel.Image.CopyToAsync(stream);
            }

            Course course = new()
            {
                Name = courseViewModel.Name,
                Description = courseViewModel.Description,
                Image = fileName,
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            return View(course);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            ViewBag.Categories = _context.Categories.AsEnumerable();

            CourseViewModel courseViewModel = new()
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
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
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id, CourseViewModel courseViewModel)
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (courseViewModel.CategoryIds != null)
            {
                foreach (var categoryId in courseViewModel.CategoryIds)
                {
                    if (!_context.Categories.Any(c => c.Id == categoryId))
                        return BadRequest();
                }
            }

            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            if (await _context.Courses.AnyAsync(c => c.Name == courseViewModel.Name && c.Name != course.Name))
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            if (courseViewModel.Image != null)
            {
                if (!courseViewModel.Image.CheckFileSize(300))
                {
                    ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                    return View();
                }
                if (!courseViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
                {
                    ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                    return View();
                }

                FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "course", course.Image);
                string fileName = $"{Guid.NewGuid()}-{courseViewModel.Image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "course", fileName);
                using (FileStream stream = new(path, FileMode.Create))
                {
                    await courseViewModel.Image.CopyToAsync(stream);
                }
                course.Image = fileName;
            }

            course.Name = courseViewModel.Name;
            course.Description = courseViewModel.Description;
            course.Start = courseViewModel.Start;
            course.Duration = courseViewModel.Duration;
            course.ClassDuration = courseViewModel.ClassDuration;
            course.SkillLevel = courseViewModel.SkillLevel;
            course.Language = courseViewModel.Language;
            course.Students = courseViewModel.Students;
            course.Price = courseViewModel.Price;

            if (courseViewModel.CategoryIds != null)
            {
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "course", course.Image);
            course.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
