namespace BackEndProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs.OrderByDescending(b => b.CreatedAt).ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(t => t.Id == id);

            if (blog is null)
                return NotFound();

            return View(blog);
        }
    }
}
