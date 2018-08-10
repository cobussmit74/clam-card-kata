using ClamCard;
using ClamCard.Implementations;
using NSubstitute;

namespace ClamCardTests.Builders
{
    class StationBuilder
    {
        public StationBuilder()
        {
            _zone = Substitute.For<IZone>();
        }

        private IZone _zone;

        public StationBuilder WithZone(IZone value)
        {
            _zone = value;
            return this;
        }

        public StationBuilder WithZoneWithCostPerSingleJourney(decimal value)
        {
            _zone = Substitute.For<IZone>();
            _zone.CostPerSingleJourney.Returns(value);
            return this;
        }

        public Station Build()
        {
            return new Station(_zone);
        }
    }
}
