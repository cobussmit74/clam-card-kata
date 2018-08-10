using ClamCard;
using ClamCard.Implementations;
using ClamCard.Models;
using NSubstitute;

namespace ClamCardTests.Builders
{
    public class JourneyBuilder
    {
        public JourneyBuilder()
        {
            _fromStation = Substitute.For<IStation>();
            _toStation = Substitute.For<IStation>();
        }

        private IStation _fromStation;
        private IStation _toStation;

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

        public Journey Build()
        {
            return new Journey
            {
                From = _fromStation,
                To = _toStation
            };
        }
    }
}
