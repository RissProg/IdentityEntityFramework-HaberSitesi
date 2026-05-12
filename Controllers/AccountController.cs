using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOrnek.Models;
using IdentityOrnek.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityOrnek.Controllers
{

    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ILogger<AccountController> logger,
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Ad = model.Ad,
                Soyad = model.Soyad,
                Telefon = model.Telefon,
                Adres = model.Adres
            };
            var sonuc = await _userManager.CreateAsync(user, model.Password);
            if (sonuc.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("TamAd", user.Ad + "", user.Soyad));
                await _userManager.AddToRoleAsync(user, "Member");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var hata in sonuc.Errors)
            {
                ModelState.AddModelError("", hata.Description);
            }
            return View(model);
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var sonuc = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
                );
            if (sonuc.Succeeded) return RedirectToAction("Index", "Haberler");

            if (sonuc.IsLockedOut)
                ModelState.AddModelError("", "Hesabınız Kitlendi");
            else
                ModelState.AddModelError("", "Kullanıcı Adı veya şifre hatalı");

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Haberler");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}