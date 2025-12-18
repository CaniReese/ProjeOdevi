using System.ComponentModel.DataAnnotations;

namespace ProjeOdevi.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
        public ICollection<TrainerServiceType> TrainerServiceTypes { get; set; } = new List<TrainerServiceType>();
    }
}
