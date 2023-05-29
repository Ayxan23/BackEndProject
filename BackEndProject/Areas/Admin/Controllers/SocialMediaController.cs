namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SocialMediaController : Controller
    {
        private readonly AppDbContext _context;

        public SocialMediaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var socialMedias = await _context.SocialMedias.OrderByDescending(sm => sm.ModifiedAt).ToListAsync();

            return View(socialMedias);
        }

        public IActionResult Create()
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SocialMediaViewModel socialMediaViewModel)
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (socialMediaViewModel.TeacherId == 0)
            {
                ModelState.AddModelError("TeacherId", "The Teachers field is required");
                return View();
            }
            if (!_context.Teachers.Any(t => t.Id == socialMediaViewModel.TeacherId))
                return BadRequest();

            SocialMedia socialMedia = new()
            {
                ClassName = socialMediaViewModel.ClassName,
                Link = socialMediaViewModel.Link,
                TeacherId = socialMediaViewModel.TeacherId,
                IsDeleted = false
            };

            await _context.SocialMedias.AddAsync(socialMedia);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var socialMedia = await _context.SocialMedias.Include(sm => sm.Teacher).FirstOrDefaultAsync(sm => sm.Id == id);
            if (socialMedia is null)
                return NotFound();

            return View(socialMedia);
        }

        public async Task<IActionResult> Update(int id)
        {
            var socialMedia = await _context.SocialMedias.Include(sm => sm.Teacher).FirstOrDefaultAsync(sm => sm.Id == id);
            if (socialMedia is null)
                return NotFound();

            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            SocialMediaViewModel socialMediaViewModel = new()
            {
                Id = socialMedia.Id,
                ClassName = socialMedia.ClassName,
                Link = socialMedia.Link,
                TeacherId = socialMedia.TeacherId,
            };

            return View(socialMediaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SocialMediaViewModel socialMediaViewModel)
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (socialMediaViewModel.TeacherId == 0)
            {
                ModelState.AddModelError("TeacherId", "The Teachers field is required");
                return View();
            }

            if (!_context.Teachers.Any(t => t.Id == socialMediaViewModel.TeacherId))
                return BadRequest();

            var socialMedia = await _context.SocialMedias.Include(sm => sm.Teacher).FirstOrDefaultAsync(sm => sm.Id == id);
            if (socialMedia is null)
                return NotFound();

            socialMedia.ClassName = socialMediaViewModel.ClassName;
            socialMedia.Link = socialMediaViewModel.Link;
            socialMedia.TeacherId = socialMediaViewModel.TeacherId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var socialMedia = await _context.SocialMedias.Include(sm => sm.Teacher).FirstOrDefaultAsync(sm => sm.Id == id);
            if (socialMedia is null)
                return NotFound();

            return View(socialMedia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public async Task<IActionResult> DeletePost(int id)
        {
            var socialMedia = await _context.SocialMedias.Include(sm => sm.Teacher).FirstOrDefaultAsync(sm => sm.Id == id);
            if (socialMedia is null)
                return NotFound();

            socialMedia.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
