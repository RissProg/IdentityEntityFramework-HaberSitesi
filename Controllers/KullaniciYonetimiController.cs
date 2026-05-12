using IdentityOrnek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityOrnek.Data;

namespace IdentityOrnek.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KullaniciYonetimiController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppdbContext _context;

        public KullaniciYonetimiController(UserManager<AppUser> userManager, AppdbContext context)
        {
            _userManager = userManager;

            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(user.Id, roles);
            }

            ViewBag.Roles = userRoles;
            return View(users);
        }

        public async Task<IActionResult> Sil(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> YazarYap(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, roles);

                await _userManager.AddToRoleAsync(user, "Yazar");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AdminYap(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);

                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OnayBekleyenYorumlar()
        {
            var yorumlar = await _context.Yorumlar
                .Include(y => y.AppUser)
                .Include(y => y.Haber)
                .Where(y => y.Onay == false)
                .OrderByDescending(y => y.Tarih)
                .ToListAsync();

            return View(yorumlar);
        }

        [HttpPost]
        public async Task<IActionResult> YorumOnayla(int id)
        {
            var yorum = await _context.Yorumlar.FindAsync(id);
            if (yorum != null)
            {
                yorum.Onay = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(OnayBekleyenYorumlar));
        }

        [HttpPost]
        public async Task<IActionResult> YorumReddet(int id)
        {
            var yorum = await _context.Yorumlar.FindAsync(id);
            if (yorum != null)
            {
                _context.Yorumlar.Remove(yorum);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(OnayBekleyenYorumlar));
        }
    }
}





