using ClamCard.Exceptions;
using ClamCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ClamCard.Extensions;

namespace ClamCard.Implementations
{
    public class Card : ICard
    {
        public Card(
            IDateTimeProvider dateTimeProvider,
            IEnumerable<Journey> journeyHistory)
        {
            if (journeyHistory == null) throw new ArgumentNullException(nameof(journeyHistory));
            _journeyHistory = new List<Journey>(journeyHistory);
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        private IDateTimeProvider _dateTimeProvider;
        private List<Journey> _journeyHistory = new List<Journey>();

        public IStation CurrentJourneyStartFrom { get; private set; }
        public IReadOnlyList<Journey> JourneyHistory => _journeyHistory;

        public void StartJourney(IStation station)
        {
            if (CurrentJourneyStartFrom != null) throw new JourneyException("A Journey is already underway");

            CurrentJourneyStartFrom = station ?? throw new ArgumentNullException(nameof(station));
        }

        public Journey EndJourney(IStation station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));
            if (CurrentJourneyStartFrom == null) throw new JourneyException("No Journey currently underway");

            if (CurrentJourneyStartFrom == station) return null;

            return ProcessJourneyEnd(station);
        }

        private Journey ProcessJourneyEnd(IStation station)
        {
            var journey = new Journey
            {
                Date = _dateTimeProvider.Now,
                From = CurrentJourneyStartFrom,
                To = station,
                Cost = CalculateCostForCurrentJourney(station)
            };

            ClearCurrentJourney(journey);

            return journey;
        }

        private decimal CalculateCostForCurrentJourney(IStation endStation)
        {
            var startZone = CurrentJourneyStartFrom.Zone;
            var endZone = endStation.Zone;

            var journeyCost = CostPerSingleJourney(startZone, endZone);
            var (costPerDayLimit, costPerWeekLimit, costPerMonthLimit) = CostLimits(startZone, endZone);
            
            var(amountAlreadyChargedToday, amountAlreadyChargedThisWeek, amountAlreadyChargedThisMonth) = 
                _journeyHistory
                .SumCostOfPreviousJourneys(_dateTimeProvider.Now.Date);

            journeyCost = LimitCostToMaxAmount(journeyCost, costPerDayLimit, amountAlreadyChargedToday);
            journeyCost = LimitCostToMaxAmount(journeyCost, costPerWeekLimit, amountAlreadyChargedThisWeek);
            journeyCost = LimitCostToMaxAmount(journeyCost, costPerMonthLimit, amountAlreadyChargedThisMonth);

            return journeyCost;
        }

        private void ClearCurrentJourney(Journey theCompletedJourney)
        {
            CurrentJourneyStartFrom = null;
            _journeyHistory.Add(theCompletedJourney);
        }

        private static decimal CostPerSingleJourney(IZone startZone, IZone endZone)
        {
            return Math.Max(
                startZone.CostPerSingleJourney,
                endZone.CostPerSingleJourney);
        }

        private static decimal CostPerDayLimit(IZone startZone, IZone endZone)
        {
            return Math.Max(
                startZone.CostPerDayLimit,
                endZone.CostPerDayLimit);
        }

        private static decimal CostPerWeekLimit(IZone startZone, IZone endZone)
        {
            return Math.Max(
                startZone.CostPerWeekLimit,
                endZone.CostPerWeekLimit);
        }

        private static decimal CostPerMonthLimit(IZone startZone, IZone endZone)
        {
            return Math.Max(
                startZone.CostPerMonthLimit,
                endZone.CostPerMonthLimit);
        }

        private static (decimal day, decimal week, decimal month) CostLimits(IZone startZone, IZone endZone)
        {
            var costPerDayLimit = CostPerDayLimit(startZone, endZone);
            var costPerWeekLimit = CostPerWeekLimit(startZone, endZone);
            var costPerMonthLimit = CostPerMonthLimit(startZone, endZone);

            return (costPerDayLimit, costPerWeekLimit, costPerMonthLimit);
        }

        private static decimal LimitCostToMaxAmount(decimal cost, decimal costUpperLimit, decimal amountAlreadyCharged)
        {
            return (amountAlreadyCharged + cost > costUpperLimit)
                ? costUpperLimit - amountAlreadyCharged
                : cost;
        }
    }
}
