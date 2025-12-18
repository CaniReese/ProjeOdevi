using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjeOdevi.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string MemberUserId { get; set; } = string.Empty;

        [Required]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public int DurationSnapshot { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceSnapshot { get; set; }
    }
}
