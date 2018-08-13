using System;
using System.Collections.Generic;
using System.Linq;
using ClamCard;
using ClamCard.Implementations;
using ClamCard.Models;

namespace ClamCardTests.Builders
{
    class CardBuilder
    {
        private CardBuilder()
        {
            _currentJourneyStartFrom = null;
            _journeyHistory = new List<Journey>();
            _dateTimeProvider = FakeDateTimeProviderBuilder.BuildDefault();
        }

        public static CardBuilder Create()
        {
            return new CardBuilder();
        }

        private List<Journey> _journeyHistory;
        private IDateTimeProvider _dateTimeProvider;
        private IStation _currentJourneyStartFrom;

        public CardBuilder WithJourneyStartedFrom(Station from)
        {
            _currentJourneyStartFrom = from;
            return this;
        }

        public CardBuilder WithNullJourneyHistory()
        {
            _journeyHistory = null;
            return this;
        }

        public CardBuilder WithJourneyHistory(params Journey[] journeys)
        {
            _journeyHistory = journeys.ToList();
            return this;
        }

        public CardBuilder WithNullDateTimeProvider()
        {
            _dateTimeProvider = null;
            return this;
        }

        public CardBuilder WithDateTimeProvider(IDateTimeProvider value)
        {
            _dateTimeProvider = value;
            return this;
        }

        public Card Build()
        {
            var card = new Card(_dateTimeProvider, _journeyHistory);

            if (_currentJourneyStartFrom != null) card.StartJourney(_currentJourneyStartFrom);

            return card;
        }
    }
}
