using ClamCard.Models;
using System;

namespace ClamCard.Implementations
{
    public class Station : IStation
    {
        public Station(IZone zone)
        {
            Zone = zone ?? throw new ArgumentNullException(nameof(zone));
        }

        public IZone Zone { get; private set; }

        public void SwipeIn(ICard card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));

            card.StartJourney(this);
        }

        public Journey SwipeOut(ICard card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));

            return card.EndJourney(this);
        }
    }
}
