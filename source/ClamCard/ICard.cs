using ClamCard.Models;
using System.Collections.Generic;

namespace ClamCard
{
    public interface ICard
    {
        IStation CurrentJourneyStartFrom { get; }
        void StartJourney(IStation station);
        Journey EndJourney(IStation station);
        IReadOnlyList<Journey> JourneyHistory { get; }
    }
}