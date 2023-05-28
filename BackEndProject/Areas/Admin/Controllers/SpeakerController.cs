namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpeakerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SpeakerController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var speakers = await _context.Speakers.OrderByDescending(s => s.ModifiedAt).ToListAsync();

            return View(speakers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpeakerViewModel speakerViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            if (speakerViewModel.Image == null)
            {
                ModelState.AddModelError("Image", "Image bosh ola bilmez");
                return View();
            }
            if (!speakerViewModel.Image.CheckFileSize(300))
            {
                ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                return View();
            }
            if (!speakerViewModel.Image.CheckFileType(ContentType.image.ToString()))
            {
                ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                return View();
            }

            string fileName = $"{Guid.NewGuid()}-{speakerViewModel.Image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await speakerViewModel.Image.CopyToAsync(stream);
            }

            Speaker speaker = new()
            {
                FullName = speakerViewModel.FullName,
                Job = speakerViewModel.Job,
                Image = fileName,
                IsDeleted = false
            };

            await _context.Speakers.AddAsync(speaker);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var speaker = await _context.Speakers.Include(c => c.EventSpeakers).ThenInclude(es => es.Event).FirstOrDefaultAsync(s => s.Id == id);
            if (speaker is null)
                return NotFound();

            return View(speaker);
        }

        public async Task<IActionResult> Update(int id)
        {
            var speaker = await _context.Speakers.Include(c => c.EventSpeakers).ThenInclude(es => es.Event).FirstOrDefaultAsync(s => s.Id == id);
            if (speaker is null)
                return NotFound();

            SpeakerViewModel speakerViewModel = new()
            {
                Id = speaker.Id,
                FullName = speaker.FullName,
                Job = speaker.Job,
            };

            return View(speakerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SpeakerViewModel speakerViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var speaker = await _context.Speakers.Include(c => c.EventSpeakers).ThenInclude(es => es.Event).FirstOrDefaultAsync(s => s.Id == id);
            if (speaker is null)
                return NotFound();

            if (speakerViewModel.Image != null)
            {
                if (!speakerViewModel.Image.CheckFileSize(300))
                {
                    ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                    return View();
                }
                if (!speakerViewModel.Image.CheckFileType(ContentType.image.ToString()))
                {
                    ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                    return View();
                }

                FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "event", speaker.Image);
                string fileName = $"{Guid.NewGuid()}-{speakerViewModel.Image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", fileName);
                using (FileStream stream = new(path, FileMode.Create))
                {
                    await speakerViewModel.Image.CopyToAsync(stream);
                }
                speaker.Image = fileName;
            }

            speaker.FullName = speakerViewModel.FullName;
            speaker.Job = speakerViewModel.Job;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var speaker = await _context.Speakers.Include(c => c.EventSpeakers).ThenInclude(es => es.Event).FirstOrDefaultAsync(s => s.Id == id);
            if (speaker is null)
                return NotFound();

            return View(speaker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public async Task<IActionResult> DeletePost(int id)
        {
            var speaker = await _context.Speakers.Include(c => c.EventSpeakers).ThenInclude(es => es.Event).FirstOrDefaultAsync(s => s.Id == id);
            if (speaker is null)
                return NotFound();

            FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "event", speaker.Image);
            speaker.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
