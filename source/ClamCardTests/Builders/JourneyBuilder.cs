using ClamCard;
using ClamCard.Implementations;
using ClamCard.Models;
using NSubstitute;
using System;

namespace ClamCardTests.Builders
{
    public class JourneyBuilder
    {
        private JourneyBuilder()
        {
            _fromStation = Substitute.For<IStation>();
            _toStation = Substitute.For<IStation>();
        }

        public static JourneyBuilder Create()
        {
            return new JourneyBuilder();
        }
        public static Journey BuildDefault()
        {
            return Create().Build();
        }

        private IStation _fromStation;
        private IStation _toStation;
        private DateTime _date;
        private decimal _cost;

        public JourneyBuilder WithFromStation(IStation station)
        {
            _fromStation = station;
            return this;
        }

        public JourneyBuilder WithToStation(IStation station)
        {
            _toStation = station;
            return this;
        }

        public JourneyBuilder WithDate(DateTime value)
        {
            _date = value;
            return this;
        }

        public JourneyBuilder WithCost(decimal value)
        {
            _cost = value;
            return this;
        }

        public Journey Build()
        {
            return new Journey
            {
                Date = _date,
                From = _fromStation,
                To = _toStation,
                Cost = _cost
            };
        }
    }
}
