using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityOrnek.Models.ViewModels
{
    public class LoginViewModel
    {
    [Required(ErrorMessage = "E-posta adresi gereklidir.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre gereklidir.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Beni Hatırlat")]
    public bool RememberMe { get; set; }
    }
}