namespace ClamCard.Models
{
    public class Journey
    {
        public IStation From { get; set; }
        public IStation To { get; set; }
        public decimal Cost { get; set; }
    }
}