using NUnit.Framework;
using NExpect;
using static NExpect.Expectations;
using ClamCardTests.Builders;
using System;
using ClamCard.Exceptions;
using ClamCard.Models;

namespace ClamCardTests
{
    [TestFixture]
    public class CardTests
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
                Expect(() => new CardBuilder().Build())
                    .To.Not.Throw();
            }
        }

        [TestFixture]
        public class StartJourney
        {
            [Test]
            public void GivenNull_ShouldThrow()
            {
                //arrange
                var card = new CardBuilder()
                    .Build();
                //act
                //assert
                Expect(() => card.StartJourney(null))
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("station");
            }

            [Test]
            public void GivenStation_SetsCurrentJourneyStartFrom()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .Build();
                //act
                card.StartJourney(station);
                //assert
                Expect(card.CurrentJourneyStartFrom).To.Not.Be.Null();
                Expect(card.CurrentJourneyStartFrom).To.Equal(station);
            }

            [Test]
            public void GivenStation_WithJourneyAlreadyUnderway_ThrowsJourneyException()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .Build();
                //act
                card.StartJourney(station);
                //assert
                Expect(() => card.StartJourney(station)).To.Throw<JourneyException>();
            }
        }

        [TestFixture]
        public class EndJourney
        {
            [Test]
            public void GivenNull_ShouldThrow()
            {
                //arrange
                var card = new CardBuilder()
                    .Build();
                //act
                //assert
                Expect(() => card.EndJourney(null))
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("station");
            }

            [Test]
            public void GivenStation_WithJourneyNoUnderway_ThrowsJourneyException()
            {
                //arrange
                var station = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .Build();
                //act
                //assert
                Expect(() => card.EndJourney(station)).To.Throw<JourneyException>();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ReturnsJourney()
            {
                //arrange
                var stationTo = new StationBuilder()
                    .Build();
                var stationFrom = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(actual).To.Not.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ClearsCurrentJourneyStartFrom()
            {
                //arrange
                var stationTo = new StationBuilder()
                    .Build();
                var stationFrom = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                card.EndJourney(stationTo);
                //assert
                Expect(card.CurrentJourneyStartFrom).To.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ReturnsJourneyWithDetails()
            {
                //arrange
                var stationTo = new StationBuilder()
                    .Build();
                var stationFrom = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(actual.From).To.Be(stationFrom);
                Expect(actual.To).To.Be(stationTo);
            }

            [Test]
            public void GivenSameStationAsJounreyStart_ReturnsNull()
            {
                //arrange
                var stationFrom = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                var actual = card.EndJourney(stationFrom);
                //assert
                Expect(actual).To.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_AddsJourneyToHistory()
            {
                //arrange
                var stationTo = new StationBuilder()
                    .Build();
                var stationFrom = new StationBuilder()
                    .Build();
                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();
                //act
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(card.JourneyHistory).To.Contain(actual);
            }

            [TestFixture]
            public class GivenSingleJourney_WithinSameZone
            {
                [Test]
                public void ReturnsJourney_WithCostOfSingleJourneyFromZone()
                {
                    //arrange
                    var cost = 2.5m;
                    //act
                    var actual = CreateSingleJourneyWithZoneCost(cost, cost);
                    //assert
                    Expect(actual.Cost).To.Equal(cost);
                }
            }

            [TestFixture]
            public class GivenSingleJourney_FromOneZoneToAnother
            {
                [Test]
                public void ReturnsJourney_WithCostOfSingleJourneyFromMostExpensiveZone_LowestFirst()
                {
                    //arrange
                    var costStart = 2.5m;
                    var costEnd = 3m;
                    //act
                    var actual = CreateSingleJourneyWithZoneCost(costStart, costEnd);
                    //assert
                    Expect(actual.Cost).To.Equal(costEnd);
                }

                [Test]
                public void ReturnsJourney_WithCostOfSingleJourneyFromMostExpensiveZone_HighestFirst()
                {
                    //arrange
                    var costStart = 3m; 
                    var costEnd = 2.5m;
                    //act
                    var actual = CreateSingleJourneyWithZoneCost(costStart, costEnd);
                    //assert
                    Expect(actual.Cost).To.Equal(costStart);
                }
            }

            private static Journey CreateSingleJourneyWithZoneCost(decimal from, decimal to)
            {
                var stationFrom = new StationBuilder()
                        .WithZoneWithCostPerSingleJourney(from)
                        .Build();

                var stationTo = new StationBuilder()
                        .WithZoneWithCostPerSingleJourney(to)
                        .Build();

                var card = new CardBuilder()
                    .WithJourneyStartedFrom(stationFrom)
                    .Build();

                return card.EndJourney(stationTo);
            }
        }
    }
}
