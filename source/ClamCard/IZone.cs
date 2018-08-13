namespace ClamCard
{
    public interface IZone
    {
        decimal CostPerSingleJourney { get; }
        decimal CostPerDayLimit { get; }
    }
}
