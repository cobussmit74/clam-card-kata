using ClamCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClamCard.Extensions
{
    public static class JourneyListExtensions
    {
        private static decimal SumCost(this IEnumerable<Journey> journeys)
        {
            return journeys.Sum(j => j.Cost);
        }

        public static decimal SumCostOfJourneysTakenOnDay(this IEnumerable<Journey> journeys, DateTime date)
        {
            return journeys
                .Where(j => j.Date.Date == date.Date)
                .SumCost();
        }

        public static decimal SumCostOfJourneysTakenInWeek(this IEnumerable<Journey> journeys, int year, int weekNumber)
        {
            return journeys
                .Where(j => j.Date.Year == year && j.Date.WeekOfYear() == weekNumber)
                .SumCost();
        }

        public static decimal SumCostOfJourneysTakenInMonth(this IEnumerable<Journey> journeys, int year, int month)
        {
            return journeys
                .Where(j => j.Date.Year == year && j.Date.Month == month)
                .SumCost();
        }

        public static (decimal day, decimal week, decimal month) SumCostOfPreviousJourneys(this IEnumerable<Journey> journeys, DateTime currentDate)
        {
            var amountAlreadyChargedToday = journeys
                .SumCostOfJourneysTakenOnDay(currentDate);

            var amountAlreadyChargedThisWeek = journeys
                .SumCostOfJourneysTakenInWeek(currentDate.Year, currentDate.WeekOfYear());

            var amountAlreadyChargedThisMonth = journeys
                .SumCostOfJourneysTakenInMonth(currentDate.Year, currentDate.Month);

            return (amountAlreadyChargedToday, amountAlreadyChargedThisWeek, amountAlreadyChargedThisMonth);
        }
    }
}
