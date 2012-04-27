using System;

namespace doktorChessGameEngine
{
    [Serializable]
    public class squarePosOffset
    {
        public readonly int x;
        public readonly int y;

        public squarePosOffset(int newX, int newY)
        {
            x = newX;
            y = newY;
        }
    }
}