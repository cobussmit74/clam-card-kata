using ClamCard.Exceptions;
using ClamCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                From = CurrentJourneyStartFrom,
                To = station,
                Cost = CalculateCostForCurrentJourney(station)
            };

            ClearCurrentJourney(journey);

            return journey;
        }

        private decimal CalculateCostForCurrentJourney(IStation endStation)
        {
            var costForSingle = CostPerSingleJourney(CurrentJourneyStartFrom, endStation);
            var costPerDayLimit = CostPerDayLimit(CurrentJourneyStartFrom, endStation);
            var totalCostToday = _journeyHistory.Sum(j => j.Cost);

            if (totalCostToday + costForSingle > costPerDayLimit)
                return costPerDayLimit - totalCostToday;
            else
                return costForSingle;
        }

        private void ClearCurrentJourney(Journey theCompletedJourney)
        {
            CurrentJourneyStartFrom = null;
            _journeyHistory.Add(theCompletedJourney);
        }

        private static decimal CostPerSingleJourney(IStation startStation, IStation endStation)
        {
            var costAtStartZone = startStation.Zone.CostPerSingleJourney;
            var costAtEndZone = endStation.Zone.CostPerSingleJourney;
            return Math.Max(costAtStartZone, costAtEndZone);
        }

        private static decimal CostPerDayLimit(IStation startStation, IStation endStation)
        {
            var costAtStartZone = startStation.Zone.CostPerDayLimit;
            var costAtEndZone = endStation.Zone.CostPerDayLimit;
            return Math.Max(costAtStartZone, costAtEndZone);
        }
    }
}
