using ClamCard;
using ClamCard.Implementations;
using NSubstitute;

namespace ClamCardTests.Builders
{
    class StationBuilder
    {
        private StationBuilder()
        {
            _zone = Substitute.For<IZone>();
        }

        public static StationBuilder Create()
        {
            return new StationBuilder();
        }

        private IZone _zone;

        public StationBuilder WithZone(IZone value)
        {
            _zone = value;
            return this;
        }

        public StationBuilder WithZoneWithCostPerSingleJourney(decimal value)
        {
            _zone = FakeZoneBuilder
                .Create()
                .WithCostPerSingleJourney(value)
                .Build();
            return this;
        }

        public Station Build()
        {
            return new Station(_zone);
        }
    }
}
