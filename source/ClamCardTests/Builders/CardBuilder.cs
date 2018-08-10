using System;
using ClamCard;
using ClamCard.Implementations;

namespace ClamCardTests.Builders
{
    class CardBuilder
    {
        private IStation _currentJourneyStartFrom = null;

        public Card Build()
        {
            var card = new Card();

            if (_currentJourneyStartFrom != null) card.StartJourney(_currentJourneyStartFrom);

            return card;
        }

        public CardBuilder WithJourneyStartedFrom(Station from)
        {
            _currentJourneyStartFrom = from;
            return this;
        }
    }
}
