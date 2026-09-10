using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SereGlowy.Data;
using SereGlowy.Models;

namespace SereGlowy.Controllers
{
    [Authorize(Roles = "User")]
    public class SkinProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SkinProfileController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // View Skin Profile
        // =========================
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
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }


        // =========================
        // Create Profile - GET
        // =========================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var existingProfile = await _context.SkinProfiles
                .FirstOrDefaultAsync(
                    p => p.UserId == userId);

            if (existingProfile != null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        // =========================
        // Create Profile - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SkinProfile profile)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var existingProfile = await _context.SkinProfiles
                    .FirstOrDefaultAsync(
                        p => p.UserId == userId);

                if (existingProfile != null)
                {
                    return RedirectToAction(nameof(Index));
                }

                profile.UserId = userId;
                profile.CreatedAt = DateTime.Now;
                profile.UpdatedAt = DateTime.Now;

                _context.SkinProfiles.Add(profile);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Skin profile created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }


        // =========================
        // Edit Profile - GET
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit()
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
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }


        // =========================
        // Edit Profile - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            SkinProfile profile)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var existingProfile = await _context.SkinProfiles
                    .FirstOrDefaultAsync(
                        p => p.UserId == userId);

                if (existingProfile == null)
                {
                    return NotFound();
                }

                existingProfile.SkinType =
                    profile.SkinType;

                existingProfile.IsSensitive =
                    profile.IsSensitive;

                existingProfile.SkinGoal =
                    profile.SkinGoal;

                existingProfile.RoutineExperience =
                    profile.RoutineExperience;

                existingProfile.MakeupUsage =
                    profile.MakeupUsage;

                existingProfile.UpdatedAt =
                    DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Skin profile updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }
    }
}