using NUnit.Framework;
using NExpect;
using static NExpect.Expectations;
using ClamCardTests.Builders;
using System;
using ClamCard.Exceptions;
using ClamCard.Models;
using System.Linq;

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
                Expect(() => CardBuilder.Create().Build())
                    .To.Not.Throw();
            }

            [Test]
            public void GivenNullJourneyHistory_ShouldThrow()
            {
                //arrange
                //act
                //assert
                Expect(() => CardBuilder.Create().WithNullJourneyHistory().Build())
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("journeyHistory"); ;
            }

            [Test]
            public void GivenNullDateTimeProvider_ShouldThrow()
            {
                //arrange
                //act
                //assert
                Expect(() => CardBuilder.Create().WithNullDateTimeProvider().Build())
                    .To.Throw<ArgumentNullException>()
                    .With.Property(err => err.ParamName)
                    .Equal.To("dateTimeProvider"); ;
            }
        }

        [TestFixture]
        public class StartJourney
        {
            [Test]
            public void GivenNull_ShouldThrow()
            {
                //arrange
                var card = CardBuilder.Create()
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
                var station = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
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
                var station = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
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
                var card = CardBuilder.Create()
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
                var station = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                //assert
                Expect(() => card.EndJourney(station)).To.Throw<JourneyException>();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ReturnsJourney()
            {
                //arrange
                var stationTo = StationBuilder.Create()
                    .Build();
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                card.StartJourney(stationFrom);
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(actual).To.Not.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ClearsCurrentJourneyStartFrom()
            {
                //arrange
                var stationTo = StationBuilder.Create()
                    .Build();
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                card.StartJourney(stationFrom);
                card.EndJourney(stationTo);
                //assert
                Expect(card.CurrentJourneyStartFrom).To.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ReturnsJourneyWithStations()
            {
                //arrange
                var stationTo = StationBuilder.Create()
                    .Build();
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                card.StartJourney(stationFrom);
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(actual.From).To.Be(stationFrom);
                Expect(actual.To).To.Be(stationTo);
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_ReturnsJourneyWithDate()
            {
                //arrange
                var today = new DateTime(2018, 8, 12, 13, 14, 15);

                var stationTo = StationBuilder.Create()
                    .Build();
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .WithDateTimeProviderFor(today)
                    .Build();
                //act
                card.StartJourney(stationFrom);
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(actual.Date).To.Equal(today);
            }

            [Test]
            public void GivenSameStationAsJourneyStart_ReturnsNull()
            {
                //arrange
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                card.StartJourney(stationFrom);
                var actual = card.EndJourney(stationFrom);
                //assert
                Expect(actual).To.Be.Null();
            }

            [Test]
            public void GivenStation_WithJourneyUnderway_AddsJourneyToHistory()
            {
                //arrange
                var stationTo = StationBuilder.Create()
                    .Build();
                var stationFrom = StationBuilder.Create()
                    .Build();
                var card = CardBuilder.Create()
                    .Build();
                //act
                card.StartJourney(stationFrom);
                var actual = card.EndJourney(stationTo);
                //assert
                Expect(card.JourneyHistory.AsEnumerable()).To.Contain(actual);
            }

            [TestFixture]
            public class GivenSingleJourney
            {
                [TestFixture]
                public class WithinSameZone
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
                public class FromOneZoneToAnother
                {
                    [Test]
                    public void GivenStartFromCheapZone_ReturnsJourney_WithCostOfSingleJourneyFromEndZone()
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
                    public void GivenStartFromExpensiveZone_ReturnsJourney_WithCostOfSingleJourneyFromStartZone()
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
                    var stationFrom = StationBuilder.Create()
                            .WithZoneWithCostPerSingleJourney(from)
                            .Build();

                    var stationTo = StationBuilder.Create()
                            .WithZoneWithCostPerSingleJourney(to)
                            .Build();

                    var card = CardBuilder.Create()
                        .Build();

                    card.StartJourney(stationFrom);
                    return card.EndJourney(stationTo);
                }
            }

            [TestFixture]
            public class GivenMultipleJourneys
            {
                [TestFixture]
                public class InSameZone
                {
                    [TestFixture]
                    public class OnSameDay
                    {
                        [Test]
                        public void ReturnsJourney_WithCostNotExceedingCostPerDayLimitOfZone()
                        {
                            //arrange
                            var costPerJourney = 2.5m;
                            var costFirstJourney = costPerJourney;
                            var limitPerDay = 4.5m;
                            var expectedCostSecondJourney = 2m;

                            var firstJourney = JourneyBuilder.Create()
                                .WithCost(costFirstJourney)
                                .Build();

                            var card = CardBuilder.Create()
                                .WithJourneyHistory(firstJourney)
                                .Build();

                            var zone = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(costPerJourney)
                                .WithCostPerDayLimit(limitPerDay)
                                .Build();

                            var stationStart = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();

                            var stationEnd = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();
                            //act
                            card.StartJourney(stationStart);
                            var actual = card.EndJourney(stationEnd);
                            //assert
                            Expect(actual.Cost).To.Equal(expectedCostSecondJourney);
                        }
                    }

                    [TestFixture]
                    public class InSameWeek
                    {
                        [Test]
                        public void GivenSecondJourneyNotOnSameDayAsFirst_ShouldReturnJourney_WithFullCostOfSingleJourney()
                        {
                            //arrange
                            var costPerJourney = 2.5m;
                            var limitPerDay = 4.5m;
                            var dateOfFirstJourney = new DateTime(2018, 8, 6);
                            var costFirstJourney = costPerJourney;
                            var dateOfSecondJourney = dateOfFirstJourney.AddDays(1);
                            var expectedCostSecondJourney = costPerJourney;

                            var firstJourney = JourneyBuilder.Create()
                                .WithDate(dateOfFirstJourney)
                                .WithCost(costFirstJourney)
                                .Build();

                            var card = CardBuilder.Create()
                                .WithDateTimeProviderFor(dateOfSecondJourney)
                                .WithJourneyHistory(firstJourney)
                                .Build();

                            var zone = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(costPerJourney)
                                .WithCostPerDayLimit(limitPerDay)
                                .Build();

                            var stationStart = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();

                            var stationEnd = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();
                            //act
                            card.StartJourney(stationStart);
                            var actual = card.EndJourney(stationEnd);
                            //assert
                            Expect(actual.Cost).To.Equal(expectedCostSecondJourney);
                        }

                        [Test]
                        public void ShouldReturnJourney_WithCostNotExceedingCostPerWeekLimitOfZone()
                        {
                            //arrange
                            var costPerJourney = 2.5m;
                            var dateOfFirstJourney = new DateTime(2018, 8, 6);
                            var costFirstJourney = 8m;
                            var limitPerDay = 4.5m;
                            var limitPerWeek = 10m;
                            var dateOfSecondJourney = dateOfFirstJourney.AddDays(1);
                            var expectedCostSecondJourney = 2m;

                            var firstJourney = JourneyBuilder.Create()
                                .WithDate(dateOfFirstJourney)
                                .WithCost(costFirstJourney)
                                .Build();

                            var card = CardBuilder.Create()
                                .WithDateTimeProviderFor(dateOfSecondJourney)
                                .WithJourneyHistory(firstJourney)
                                .Build();

                            var zone = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(costPerJourney)
                                .WithCostPerDayLimit(limitPerDay)
                                .WithCostPerWeekLimit(limitPerWeek)
                                .Build();

                            var stationStart = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();

                            var stationEnd = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();
                            //act
                            card.StartJourney(stationStart);
                            var actual = card.EndJourney(stationEnd);
                            //assert
                            Expect(actual.Cost).To.Equal(expectedCostSecondJourney);
                        }

                        [Test]
                        public void GivenSecondJourneyInSameWeek_ButNotSameYear_ShouldReturnJourney_WithFullCostOfSingleJourney()
                        {
                            //arrange
                            var costPerJourney = 2.5m;
                            var costFirstJourney = 8m;
                            var limitPerDay = 4.5m;
                            var limitPerWeek = 10m;
                            var dateOfFirstJourney = new DateTime(2017, 1, 1);
                            var dateOfSecondJourney = dateOfFirstJourney.AddYears(1);
                            var expectedCostSecondJourney = costPerJourney;

                            var firstJourney = JourneyBuilder.Create()
                                .WithDate(dateOfFirstJourney)
                                .WithCost(costFirstJourney)
                                .Build();

                            var card = CardBuilder.Create()
                                .WithDateTimeProviderFor(dateOfSecondJourney)
                                .WithJourneyHistory(firstJourney)
                                .Build();

                            var zone = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(costPerJourney)
                                .WithCostPerWeekLimit(limitPerWeek)
                                .Build();

                            var stationStart = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();

                            var stationEnd = StationBuilder.Create()
                                .WithZone(zone)
                                .Build();
                            //act
                            card.StartJourney(stationStart);
                            var actual = card.EndJourney(stationEnd);
                            //assert
                            Expect(actual.Cost).To.Equal(expectedCostSecondJourney);
                        }
                    }
                }

                [TestFixture]
                public class FromOneZoneToAnother
                {
                    [TestFixture]
                    public class OnSameDay
                    {
                        [Test]
                        public void ReturnsJourney_WithCostNotExceedingCostPerDayLimitOfMostExpensiveZone()
                        {
                            //arrange
                            var costFirstJourney = 3m;
                            var expectedCostSecondJourney = 2m;

                            var zone1 = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(3)
                                .WithCostPerDayLimit(4)
                                .Build();

                            var zone2 = FakeZoneBuilder
                                .Create()
                                .WithCostPerSingleJourney(4)
                                .WithCostPerDayLimit(5)
                                .Build();

                            var firstJourney = JourneyBuilder.Create()
                                .WithCost(costFirstJourney)
                                .Build();

                            var card = CardBuilder.Create()
                                .WithJourneyHistory(firstJourney)
                                .Build();
                            
                            var stationStart = StationBuilder.Create()
                                .WithZone(zone1)
                                .Build();

                            var stationEnd = StationBuilder.Create()
                                .WithZone(zone2)
                                .Build();
                            //act
                            card.StartJourney(stationStart);
                            var actual = card.EndJourney(stationEnd);
                            //assert
                            Expect(actual.Cost).To.Equal(expectedCostSecondJourney);
                        }
                    }
                }
            }
        }
    }
}
