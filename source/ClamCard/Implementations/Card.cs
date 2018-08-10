using ClamCard.Exceptions;
using ClamCard.Models;
using System;
using System.Collections.Generic;

namespace ClamCard.Implementations
{
    public class Card : ICard
    {
        public Card()
        {
            JourneyHistory = new List<Journey>();
        }

        public IStation CurrentJourneyStartFrom { get; private set; }
        public IList<Journey> JourneyHistory { get; private set; }

        public void StartJourney(IStation station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));
            if (CurrentJourneyStartFrom != null) throw new JourneyException("A Journey is already underway");

            CurrentJourneyStartFrom = station;
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

            EndCurrentJourney(journey);

            return journey;
        }

        private decimal CalculateCostForCurrentJourney(IStation endStation)
        {
            var costAtStartZone = CurrentJourneyStartFrom.Zone.CostPerSingleJourney;
            var costAtEndZone = endStation.Zone.CostPerSingleJourney;
            return Math.Max(costAtStartZone, costAtEndZone);
        }

        private void EndCurrentJourney(Journey theCompletedJourney)
        {
            CurrentJourneyStartFrom = null;
            JourneyHistory.Add(theCompletedJourney);
        }
    }
}
