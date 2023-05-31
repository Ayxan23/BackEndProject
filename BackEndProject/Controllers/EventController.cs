namespace BackEndProject.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var foundEvents = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).OrderBy(e => e.StartTime).ToListAsync();

            return View(foundEvents);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var foundEvent = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(e => e.Id == id);

            if (foundEvent is null)
                return NotFound();

            return View(foundEvent);
        }
    }
}
