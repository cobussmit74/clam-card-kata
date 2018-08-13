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
            _journeyHistory = new List<Journey>();
            _dateTimeProvider = FakeDateTimeProviderBuilder.BuildDefault();
        }

        public static CardBuilder Create()
        {
            return new CardBuilder();
        }

        private List<Journey> _journeyHistory;
        private IDateTimeProvider _dateTimeProvider;
        
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

        public CardBuilder WithDateTimeProviderFor(DateTime value)
        {
            _dateTimeProvider = FakeDateTimeProviderBuilder.BuildFor(value);
            return this;
        }

        public Card Build()
        {
            var card = new Card(_dateTimeProvider, _journeyHistory);

            return card;
        }
    }
}
