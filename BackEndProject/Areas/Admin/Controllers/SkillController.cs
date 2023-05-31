using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Data;

namespace BackEndProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SkillController : Controller
    {
        private readonly AppDbContext _context;

        public SkillController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            var skills = await _context.Skills.OrderByDescending(s => s.ModifiedAt).ToListAsync();

            return View(skills);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(SkillViewModel skillViewModel)
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (skillViewModel.TeacherId == 0)
            {
                ModelState.AddModelError("TeacherId", "The Teachers field is required");
                return View();
            }
            if (!_context.Teachers.Any(t => t.Id == skillViewModel.TeacherId))
                return BadRequest();

            Skill skill = new()
            {
                Name = skillViewModel.Name,
                Rate = skillViewModel.Rate,
                TeacherId = skillViewModel.TeacherId,
                IsDeleted = false
            };

            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var skill = await _context.Skills.Include(s => s.Teacher).FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                return NotFound();

            return View(skill);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            var skill = await _context.Skills.Include(s => s.Teacher).FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                return NotFound();

            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            SkillViewModel skillViewModel = new()
            {
                Id = skill.Id,
                Name = skill.Name,
                Rate = skill.Rate,
                TeacherId = skill.TeacherId,
            };

            return View(skillViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id, SkillViewModel skillViewModel)
        {
            ViewBag.Teachers = _context.Teachers.AsEnumerable();

            if (!ModelState.IsValid)
                return View();

            if (skillViewModel.TeacherId == 0)
            {
                ModelState.AddModelError("TeacherId", "The Teachers field is required");
                return View();
            }
            if (!_context.Teachers.Any(t => t.Id == skillViewModel.TeacherId))
                return BadRequest();

            var skill = await _context.Skills.Include(s => s.Teacher).FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                return NotFound();

            skill.Name = skillViewModel.Name;
            skill.Rate = skillViewModel.Rate;
            skill.TeacherId = skillViewModel.TeacherId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.Include(s => s.Teacher).FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                return NotFound();

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var skill = await _context.Skills.Include(s => s.Teacher).FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                return NotFound();

            skill.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
