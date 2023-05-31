using BackEndProject.Areas.Admin.ViewModels;
using BackEndProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.OrderByDescending(e => e.ModifiedAt).ToListAsync();

            return View(events);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            ViewBag.Speakers = _context.Speakers.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(EventViewModel eventViewModel)
        {
            ViewBag.Speakers = _context.Speakers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if(eventViewModel.StartTime > eventViewModel.EndTime)
            {
                ModelState.AddModelError("StartTime", "StartTime EndTime'dan kicik olmalidir");
                ModelState.AddModelError("EndTime", "EndTime StartTime'dan boyuk olmalidir");
                return View();
            }

            if (eventViewModel.SpeakerIds == null)
            {
                ModelState.AddModelError("SpeakerIds", "The Speakers field is required");
                return View();
            }

            foreach (var speakersId in eventViewModel.SpeakerIds)
            {
                if (!_context.Speakers.Any(s => s.Id == speakersId))
                    return BadRequest();
            }

            if (await _context.Events.AnyAsync(c => c.Name == eventViewModel.Name))
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            if (eventViewModel.Image == null)
            {
                ModelState.AddModelError("Image", "Image bosh ola bilmez");
                return View();
            }
            if (!eventViewModel.Image.CheckFileSize(300))
            {
                ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                return View();
            }
            if (!eventViewModel.Image.CheckFileType(ContentType.image.ToString()))
            {
                ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                return View();
            }

            string fileName = $"{Guid.NewGuid()}-{eventViewModel.Image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await eventViewModel.Image.CopyToAsync(stream);
            }

            Event newEvent = new()
            {
                Name = eventViewModel.Name,
                Description = eventViewModel.Description,
                Image = fileName,
                StartTime = eventViewModel.StartTime,
                EndTime = eventViewModel.EndTime,
                Venue = eventViewModel.Venue,
                IsDeleted = false
            };

            List<EventSpeaker> eventSpeakers = new();
            foreach (var spekerId in eventViewModel.SpeakerIds)
            {
                EventSpeaker eventSpeaker = new()
                {
                    EventId = eventViewModel.Id,
                    SpeakerId = spekerId
                };
                eventSpeakers.Add(eventSpeaker);
            }
            newEvent.EventSpeakers = eventSpeakers;

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);
            if (foundEvent is null)
                return NotFound();

            return View(foundEvent);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);
            if (foundEvent is null)
                return NotFound();

            ViewBag.Speakers = _context.Speakers.AsEnumerable();

            EventViewModel eventViewModel = new()
            {
                Id = foundEvent.Id,
                Name = foundEvent.Name,
                Description = foundEvent.Description,
                StartTime = foundEvent.StartTime,
                EndTime = foundEvent.EndTime,
                Venue = foundEvent.Venue,
            };

            return View(eventViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id, EventViewModel eventViewModel)
        {
            ViewBag.Speakers = _context.Speakers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (eventViewModel.SpeakerIds != null)
            {
                foreach (var speakerId in eventViewModel.SpeakerIds)
                {
                    if (!_context.Speakers.Any(s => s.Id == speakerId))
                        return BadRequest();
                }
            }

            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);
            if (foundEvent is null)
                return NotFound();

            if (await _context.Events.AnyAsync(e => e.Name == eventViewModel.Name && e.Name != foundEvent.Name))
            {
                ModelState.AddModelError("Name", "Name already exist");
                return View();
            }

            if (eventViewModel.Image != null)
            {
                if (!eventViewModel.Image.CheckFileSize(300))
                {
                    ModelState.AddModelError("Image", "Faylin hecmi 300kb-dan kicik olmalidir.");
                    return View();
                }
                if (!eventViewModel.Image.CheckFileType(ContentType.image.ToString()))
                {
                    ModelState.AddModelError("Image", "Faylin tipi image olmalidir.");
                    return View();
                }

                FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "event", foundEvent.Image);
                string fileName = $"{Guid.NewGuid()}-{eventViewModel.Image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", fileName);
                using (FileStream stream = new(path, FileMode.Create))
                {
                    await eventViewModel.Image.CopyToAsync(stream);
                }
                foundEvent.Image = fileName;
            }

            foundEvent.Name = eventViewModel.Name;
            foundEvent.Description = eventViewModel.Description;
            foundEvent.Venue = eventViewModel.Venue;
            foundEvent.StartTime = eventViewModel.StartTime;
            foundEvent.EndTime = eventViewModel.EndTime;

            if (eventViewModel.SpeakerIds != null)
            {
                List<EventSpeaker> eventSpeakers = new();
                foreach (var speakerId in eventViewModel.SpeakerIds)
                {
                    EventSpeaker eventSpeaker = new()
                    {
                        EventId = eventViewModel.Id,
                        SpeakerId = speakerId
                    };
                    eventSpeakers.Add(eventSpeaker);
                }
                foundEvent.EventSpeakers = eventSpeakers;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);
            if (foundEvent is null)
                return NotFound();

            return View(foundEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);
            if (foundEvent is null)
                return NotFound();

            FileService.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "img", "event", foundEvent.Image);
            foundEvent.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

