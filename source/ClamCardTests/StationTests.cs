using NUnit.Framework;
using NExpect;
using static NExpect.Expectations;
using ClamCardTests.Builders;
using System;
using System.Linq;
using NSubstitute;
using ClamCard;

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
                var card = Substitute.For<ICard>();
                //act
                station.SwipeIn(card);
                //assert
                card.Received(1).StartJourney(station);
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
            public void GivenCard_ShouldEndJouneyAtThatStation()
            {
                //arrange
                var journey = new JourneyBuilder().Build();
                var station = new StationBuilder()
                    .Build();
                var card = Substitute.For<ICard>();
                card.EndJourney(station).Returns(journey);
                //act
                var actual = station.SwipeOut(card);
                //assert
                Expect(actual).To.Be(journey);
            }
        }
    }
}
