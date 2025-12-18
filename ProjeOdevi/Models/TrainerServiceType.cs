namespace ProjeOdevi.Models
{
    public class TrainerServiceType
    {
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }
    }
}
