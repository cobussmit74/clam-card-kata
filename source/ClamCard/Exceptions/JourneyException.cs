using System;

namespace ClamCard.Exceptions
{
    public class JourneyException : Exception
    {
        public JourneyException(string message) : base(message)
        {
        }
    }
}
