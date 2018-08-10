using NUnit.Framework;
using NExpect;
using static NExpect.Expectations;
using ClamCardTests.Builders;
using System;
using System.Linq;

namespace ClamCardTests
{
    [TestFixture]
    public class StationTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void ShouldNotThrow()
            {
                //arrange
                //act
                //assert
                Expect(() => new StationBuilder().Build())
                    .To.Not.Throw();
            }

            [Test]
            public void GivenNullZone_ShouldThrow()
            {
                //arrange
                //act
                //assert
                Expect(() => new StationBuilder().WithZone(null).Build())
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("zone");
            }
        }

        [TestFixture]
        public class SwipeIn
        {
            [Test]
            public void GivenNull_ShouldThrow()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                //act
                //assert
                Expect(() => station.SwipeIn(null))
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("card");
            }

            [Test]
            public void GivenCard_ShouldStartJouneyFromThatStation()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .Build();
                //act
                station.SwipeIn(card);
                //assert
                Expect(card.CurrentJourneyStartFrom).To.Be(station);
            }
        }

        [TestFixture]
        public class SwipeOut
        {
            [Test]
            public void GivenNull_ShouldThrow()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                //act
                //assert
                Expect(() => station.SwipeOut(null))
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("card");
            }

            [Test]
            public void GivenCard_WithJourneyUnderway_ShouldEndJouneyAtThatStation()
            {
                //arrange
                var stationFrom = new StationBuilder()
                    .Build();
                var stationTo = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                var actual = stationTo.SwipeOut(card);
                //assert
                Expect(actual.To).To.Be(stationTo);
            }
        }
    }
}
