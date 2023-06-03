using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeacherController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers.OrderByDescending(t => t.ModifiedAt).ToListAsync();

            return View(teachers);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(TeacherViewModel teacherViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            if (teacherViewModel.Image == null)
            {
                ModelState.AddModelError("Image", "Image bosh ola bilmez");
                return View();
            }
            if (!teacherViewModel.Image.CheckFileSize(300))
            {
                ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                return View();
            }
            if (!teacherViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
            {
                ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                return View();
            }

            string fileName = $"{Guid.NewGuid()}-{teacherViewModel.Image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "teacher", fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await teacherViewModel.Image.CopyToAsync(stream);
            }

            Teacher teacher = new()
            {
                FullName = teacherViewModel.FullName,
                Job = teacherViewModel.Job,
                Description = teacherViewModel.Description,
                Image = fileName,
                Degree = teacherViewModel.Degree,
                Experience = teacherViewModel.Experience,
                Hobbies = teacherViewModel.Hobbies,
                Faculty = teacherViewModel.Faculty,
                Mail = teacherViewModel.Mail,
                Phone = teacherViewModel.Phone,
                IsDeleted = false
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.SocialMedias).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return NotFound();

            return View(teacher);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.SocialMedias).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return NotFound();

            TeacherViewModel teacherViewModel = new()
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Job = teacher.Job,
                Description = teacher.Description,
                Degree = teacher.Degree,
                Experience = teacher.Experience,
                Hobbies = teacher.Hobbies,
                Faculty = teacher.Faculty,
                Mail = teacher.Mail,
                Phone = teacher.Phone,
            };

            return View(teacherViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id, TeacherViewModel teacherViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var teacher = await _context.Teachers.Include(t => t.SocialMedias).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return NotFound();

            if (teacherViewModel.Image != null)
            {
                if (!teacherViewModel.Image.CheckFileSize(300))
                {
                    ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                    return View();
                }
                if (!teacherViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
                {
                    ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                    return View();
                }

                FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "teacher", teacher.Image);
                string fileName = $"{Guid.NewGuid()}-{teacherViewModel.Image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "teacher", fileName);
                using (FileStream stream = new(path, FileMode.Create))
                {
                    await teacherViewModel.Image.CopyToAsync(stream);
                }
                teacher.Image = fileName;
            }

            teacher.FullName = teacherViewModel.FullName;
            teacher.Job = teacherViewModel.Job;
            teacher.Description = teacherViewModel.Description;
            teacher.Degree = teacherViewModel.Degree;
            teacher.Experience = teacherViewModel.Experience;
            teacher.Hobbies = teacherViewModel.Hobbies;
            teacher.Faculty = teacherViewModel.Faculty;
            teacher.Mail = teacherViewModel.Mail;
            teacher.Phone = teacherViewModel.Phone;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.SocialMedias).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return NotFound();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.SocialMedias).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return NotFound();

            FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "teacher", teacher.Image);
            teacher.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
