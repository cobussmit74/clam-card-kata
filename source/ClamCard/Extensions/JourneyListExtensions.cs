using ClamCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClamCard.Extensions
{
    public static class JourneyListExtensions
    {
        public static decimal SumCost(this IEnumerable<Journey> journeys)
        {
            return journeys.Sum(j => j.Cost);
        }

        public static decimal SumCostOfJourneysTakenOnDay(this IEnumerable<Journey> journeys, DateTime date)
        {
            return journeys
                .Where(j => j.Date.Date == date.Date)
                .SumCost();
        }

        public static decimal SumCostOfJourneysTakenInWeek(this IEnumerable<Journey> journeys, int weekNumber)
        {
            return journeys
                .Where(j => j.Date.WeekOfYear() == weekNumber)
                .SumCost();
        }
    }
}
