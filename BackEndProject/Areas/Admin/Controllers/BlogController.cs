using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs.OrderByDescending(b => b.ModifiedAt).ToListAsync();

            return View(blogs);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(BlogViewModel blogViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            if (blogViewModel.Image == null)
            {
                ModelState.AddModelError("Image", "Image bosh ola bilmez");
                return View();
            }
            if (!blogViewModel.Image.CheckFileSize(300))
            {
                ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                return View();
            }
            if (!blogViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
            {
                ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                return View();
            }

            string fileName = $"{Guid.NewGuid()}-{blogViewModel.Image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "blog", fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await blogViewModel.Image.CopyToAsync(stream);
            }

            var userName = HttpContext?.User?.Identity?.Name;
            Blog Blog = new()
            {
                Name = blogViewModel.Name,
                Description = blogViewModel.Description,
                Image = fileName,
                CreatedBy = userName,
                IsDeleted = false
            };

            await _context.Blogs.AddAsync(Blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null)
                return NotFound();

            return View(blog);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null)
                return NotFound();

            BlogViewModel blogViewModel = new()
            {
                Id = blog.Id,
                Name = blog.Name,
                Description = blog.Description,
            };

            return View(blogViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id, BlogViewModel blogViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null)
                return NotFound();

            if (blogViewModel.Image != null)
            {
                if (!blogViewModel.Image.CheckFileSize(300))
                {
                    ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                    return View();
                }
                if (!blogViewModel.Image.CheckFileType(ContentTypes.image.ToString()))
                {
                    ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                    return View();
                }

                FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "blog", blog.Image);
                string fileName = $"{Guid.NewGuid()}-{blogViewModel.Image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "blog", fileName);
                using (FileStream stream = new(path, FileMode.Create))
                {
                    await blogViewModel.Image.CopyToAsync(stream);
                }
                blog.Image = fileName;
            }

            blog.Name = blogViewModel.Name;
            blog.Description = blogViewModel.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null)
                return NotFound();

            FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "blog", blog.Image);
            blog.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
