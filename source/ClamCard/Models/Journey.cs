using System;

namespace ClamCard.Models
{
    public class Journey
    {
        public DateTime Date { get; set; }
        public IStation From { get; set; }
        public IStation To { get; set; }
        public decimal Cost { get; set; }
    }
}