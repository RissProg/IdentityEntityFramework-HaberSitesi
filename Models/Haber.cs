using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityOrnek.Models
{
    public class Haber
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public string Kategori { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        public string Icerik { get; set; } = string.Empty;

        public string? GorselUrl { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        public string? YazarId { get; set; }

        [ForeignKey("YazarId")]
        public AppUser? Yazar { get; set; }
    }
}