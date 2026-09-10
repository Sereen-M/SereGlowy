using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

namespace SereGlowy.Controllers
{
    [Authorize(Roles = "User")]
    public class OnboardingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OnboardingController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // Onboarding - GET
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var existingProfile = await _context.SkinProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile != null)
            {
                return RedirectToAction(
                    "Index",
                    "SkinProfile");
            }

            return View();
        }


        // =========================
        // Onboarding - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(
            string skinType,
            bool? isSensitive,
            string skinGoal,
            string routineExperience,
            string makeupUsage)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var existingProfile = await _context.SkinProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile != null)
            {
                return RedirectToAction(
                    "Index",
                    "SkinProfile");
            }

            if (string.IsNullOrWhiteSpace(skinType) ||
                string.IsNullOrWhiteSpace(skinGoal) ||
                string.IsNullOrWhiteSpace(routineExperience) ||
                string.IsNullOrWhiteSpace(makeupUsage))
            {
                TempData["ErrorMessage"] =
                    "Please complete all questions.";

                return RedirectToAction(nameof(Index));
            }

            var profile = new SkinProfile
            {
                UserId = userId,
                SkinType = skinType,
                IsSensitive = isSensitive,
                SkinGoal = skinGoal,
                RoutineExperience = routineExperience,
                MakeupUsage = makeupUsage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.SkinProfiles.Add(profile);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your SereGlowy profile is ready!";

            return RedirectToAction(
                "Index",
                "SkinProfile");
        }
    }
}