using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOrnek.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Claims;

namespace IdentityOrnek.Data
{
    public static class DbSeeder
    {
        public static async Task RoleEkle(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            string[] roller = { "Admin", "Member","Yazar" };
            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                { await roleManager.CreateAsync(new IdentityRole(rol)); }
            }
            
            //Varsayılan Kullanıcı Oluştur
            var adminMail = "admin@proje.com";
            var adminUser = await userManager.FindByEmailAsync(adminMail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminMail,
                    Email = adminMail,
                    Ad = "Mamad",
                    Soyad = "Kizoglu",
                    Adres = "Izmir",
                    Telefon = "555 444 33 22",
                    EmailConfirmed = true
                };
                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddClaimAsync(newAdmin, new Claim("TamAd", newAdmin.Ad + " " + newAdmin.Soyad));
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

        }
    }
}