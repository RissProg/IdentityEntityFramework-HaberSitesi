using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityOrnek.Models
{
    public class Yorum
    {
        public int Id { get; set; }
        public string Icerik { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool Onay { get; set; } = false;

        public int HaberId { get; set; }

        [ForeignKey("HaberId")]
        public Haber? Haber { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}