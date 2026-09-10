using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;

namespace SereGlowy.Controllers
{
    [Authorize(Roles = "User")]
    public class RoutineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RoutineController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var profile = await _context.SkinProfiles
                .FirstOrDefaultAsync(
                    p => p.UserId == userId);

            if (profile == null)
            {
                TempData["ErrorMessage"] =
                    "Please create your skin profile first.";

                return RedirectToAction(
                    "Index",
                    "Onboarding");
            }

            var routines = await _context.RoutineRecommendations
                .Where(r =>
                    r.SkinType == profile.SkinType &&
                    r.SkinGoal == profile.SkinGoal)
                .OrderBy(r => r.RoutineTime)
                .ThenBy(r => r.StepOrder)
                .ToListAsync();

            ViewBag.SkinType = profile.SkinType;
            ViewBag.SkinGoal = profile.SkinGoal;

            return View(routines);
        }
    }
}