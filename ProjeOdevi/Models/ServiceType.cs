using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeOdevi.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = string.Empty;

        [Range(15, 300)]
        [Display(Name = "Süre (dk)")]
        public int DurationMinutes { get; set; }

        [Range(0, 100000)]
        [Display(Name = "Ücret (₺)")]

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
