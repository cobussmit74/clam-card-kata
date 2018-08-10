using ClamCard.Models;

namespace ClamCard
{
    public interface IStation
    {
        IZone Zone { get; }

        void SwipeIn(ICard card);
        Journey SwipeOut(ICard card);
    }
}
