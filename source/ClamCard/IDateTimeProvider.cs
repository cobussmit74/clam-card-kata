using System;

namespace ClamCard
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
