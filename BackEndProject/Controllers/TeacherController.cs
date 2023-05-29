namespace BackEndProject.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers.Include(t => t.Skills).Include(t => t.SocialMedias).OrderByDescending(t => t.ModifiedAt).ToListAsync();

            return View(teachers);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.Skills).Include(t => t.SocialMedias).FirstOrDefaultAsync(t => t.Id == id);

            if (teacher is null)
                return NotFound();

            return View(teacher);
        }
    }
}
