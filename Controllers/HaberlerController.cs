using IdentityOrnek.Data;
using IdentityOrnek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IdentityOrnek.Controllers
{
    // DİKKAT: Buradaki sınıf seviyesindeki [Authorize] özniteliğini TAMAMEN SİLDİK!
    public class HaberlerController : Controller
    {
        private readonly AppdbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HaberlerController(AppdbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 1. HERKESE AÇIK BÖLÜMLER (Ziyaretçiler için)
        // ==========================================

        public async Task<IActionResult> Index(string? kategori)
        {
            var query = _context.Haberler.Include(h => h.Yazar).AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
            {
                query = query.Where(h => h.Kategori == kategori);
            }

            var haberler = await query.OrderByDescending(h => h.OlusturulmaTarihi).ToListAsync();
            return View(haberler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var haber = await _context.Haberler.Include(h => h.Yazar).FirstOrDefaultAsync(h => h.Id == id);
            if (haber == null) return NotFound();

            ViewBag.Yorumlar = await _context.Yorumlar
                .Include(y => y.AppUser)
                .Where(y => y.HaberId == id && y.Onay == true)
                .OrderByDescending(y => y.Tarih)
                .ToListAsync();

            ViewBag.BenzerHaberler = await _context.Haberler
                .Where(h => h.Kategori == haber.Kategori && h.Id != id)
                .OrderByDescending(h => h.OlusturulmaTarihi)
                .Take(3) // 3 Adet
                .ToListAsync();

            return View(haber);
        }

        // ==========================================
        // 2. SADECE ÜYELERE AÇIK BÖLÜMLER (Member, Yazar, Admin)
        // ==========================================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> YorumEkle(int HaberId, string Icerik)
        {
            if (string.IsNullOrWhiteSpace(Icerik)) return RedirectToAction("Detay", new { id = HaberId });

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var yeniYorum = new Yorum
                {
                    HaberId = HaberId,
                    Icerik = Icerik,
                    AppUserId = user.Id,
                    Tarih = DateTime.Now,
                    Onay = false
                };
                _context.Yorumlar.Add(yeniYorum);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Detay", new { id = HaberId });
        }

        // ==========================================
        // 3. YAZAR VE ADMİNE AÇIK BÖLÜMLER (Yönetim Paneli İşlemleri)
        // ==========================================

        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Liste()
        {
            var haberler = await _context.Haberler
                .Include(h => h.Yazar)
                .OrderByDescending(h => h.OlusturulmaTarihi)
                .ToListAsync();

            return View(haberler);
        }

        [Authorize(Roles = "Admin,Yazar")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Ekle(Haber haber, IFormFile? Gorsel)
        {
            ModelState.Remove("Yazar");
            ModelState.Remove("YazarId");

            if (ModelState.IsValid)
            {
                // GÖRSEL KAYDETME MANTIĞI BURAYA EKLENDİ
                if (Gorsel != null)
                {
                    string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(Gorsel.FileName);
                    string yol = Path.Combine(klasorYolu, dosyaAdi);

                    using (var stream = new FileStream(yol, FileMode.Create))
                    {
                        await Gorsel.CopyToAsync(stream);
                    }
                    haber.GorselUrl = "/img/" + dosyaAdi; // Veritabanına yolunu yazıyoruz
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    haber.YazarId = user.Id;
                }

                _context.Haberler.Add(haber);
                await _context.SaveChangesAsync();
                return RedirectToAction("Liste"); // Başarılıysa Liste'ye gider
            }
            return View(haber);
        }

        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var haber = await _context.Haberler.FindAsync(id);
            if (haber == null) return NotFound();
            return View(haber);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Duzenle(Haber haber, IFormFile? Gorsel)
        {
            // Model doğrulaması için gereksiz alanları temizle
            ModelState.Remove("Yazar");
            ModelState.Remove("YazarId");

            if (ModelState.IsValid)
            {
                // Eğer yeni bir görsel yüklenmişse
                if (Gorsel != null)
                {
                    string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(Gorsel.FileName);
                    string yol = Path.Combine(klasorYolu, dosyaAdi);

                    using (var stream = new FileStream(yol, FileMode.Create))
                    {
                        await Gorsel.CopyToAsync(stream);
                    }
                    haber.GorselUrl = "/img/" + dosyaAdi;
                }

                _context.Update(haber);
                await _context.SaveChangesAsync();
                return RedirectToAction("Liste");
            }
            return View(haber);
        }

        // ==========================================
        // 4. SADECE ADMİNE AÇIK BÖLÜMLER
        // ==========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var haber = await _context.Haberler.FindAsync(id);
            if (haber != null)
            {
                _context.Haberler.Remove(haber);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> YorumSil(int id, int haberId)
        {
            var yorum = await _context.Yorumlar.FindAsync(id);
            if (yorum != null)
            {
                _context.Yorumlar.Remove(yorum);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Detay", new { id = haberId });
        }

        // ==========================================
        // TİNYMCE İÇERİK RESİM/VİDEO YÜKLEME METODU
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> ResimYukle(IFormFile file)
        {
            // TinyMCE varsayılan olarak dosyayı "file" parametresiyle gönderir
            if (file == null || file.Length == 0)
            {
                return BadRequest("Dosya yüklenemedi.");
            }

            // wwwroot/img klasörünün yolunu belirliyoruz
            string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

            // Dosya adının çakışmaması için benzersiz bir isim (Guid) oluşturuyoruz
            string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string yol = Path.Combine(klasorYolu, dosyaAdi);

            using (var stream = new FileStream(yol, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // DİKKAT: TinyMCE bizden tam olarak bu JSON formatını bekler: { "location": "/dosyayolu.jpg" }
            return Json(new { location = "/img/" + dosyaAdi });
        }
    }
}