﻿using ClamCard;
using NSubstitute;

namespace ClamCardTests.Builders
{
    public class FakeZoneBuilder
    {
        private FakeZoneBuilder()
        {
            _costPerSingleJourney = 1m;
            _costPerDayLimit = 1000000m;
            _costPerWeekLimit = 1000000m;
            _costPerMonthLimit = 1000000m;
        }

        public static FakeZoneBuilder Create()
        {
            return new FakeZoneBuilder();
        }

        private decimal _costPerSingleJourney;
        private decimal _costPerDayLimit;
        private decimal _costPerWeekLimit;
        private decimal _costPerMonthLimit;

        public FakeZoneBuilder WithCostPerSingleJourney(decimal value)
        {
            _costPerSingleJourney = value;
            return this;
        }

        public FakeZoneBuilder WithCostPerDayLimit(decimal value)
        {
            _costPerDayLimit = value;
            return this;
        }

        public FakeZoneBuilder WithCostPerWeekLimit(decimal value)
        {
            _costPerWeekLimit = value;
            return this;
        }

        public FakeZoneBuilder WithCostPerMonthLimit(decimal value)
        {
            _costPerMonthLimit = value;
            return this;
        }

        public IZone Build()
        {
            var zone = Substitute.For<IZone>(); ;

            zone.CostPerSingleJourney.Returns(_costPerSingleJourney);
            zone.CostPerDayLimit.Returns(_costPerDayLimit);
            zone.CostPerWeekLimit.Returns(_costPerWeekLimit);
            zone.CostPerMonthLimit.Returns(_costPerMonthLimit);

            return zone;
        }
    }
}
