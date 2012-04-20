using System;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    public class loopConfig
    {
        public int startX;
        public int startY;
        public int finishX;
        public int finishY;
        public int directionX;
        public int directionY;

        public loopConfig(squarePos position, vectorDirection dir)
        {
            switch (dir)
            {
                case vectorDirection.left:
                    startX = position.x - 1;
                    finishX = -1;
                    startY = position.y;
                    finishY = startY + 1;
                    directionX = -1;
                    directionY = 0;
                    break;
                case vectorDirection.right:
                    startX = position.x + 1;
                    finishX = baseBoard.sizeX;
                    startY = position.y;
                    finishY = startY + 1;
                    directionX = +1;
                    directionY = 0;
                    break;
                case vectorDirection.up:
                    startY = position.y + 1;
                    finishY = baseBoard.sizeY;
                    startX = position.x;
                    finishX = position.x + 1;
                    directionX = 0;
                    directionY = +1;
                    break;
                case vectorDirection.down:
                    startY = position.y - 1;
                    finishY = -1;
                    startX = position.x;
                    finishX = position.x + 1;
                    directionX = 0;
                    directionY = -1;
                    break;
                case vectorDirection.leftup:
                    startY = position.y + 1;
                    startX = position.x - 1;
                    finishY = baseBoard.sizeY;
                    finishX = -1;
                    directionY = +1;
                    directionX = -1;
                    break;
                case vectorDirection.leftdown:
                    startY = position.y - 1;
                    startX = position.x - 1;
                    finishY = -1;
                    finishX = -1;
                    directionY = -1;
                    directionX = -1;
                    break;
                case vectorDirection.rightup:
                    startY = position.y + 1;
                    startX = position.x + 1;
                    finishY = baseBoard.sizeY;
                    finishX = baseBoard.sizeX;
                    directionY = +1;
                    directionX = +1;
                    break;
                case vectorDirection.rightdown:
                    startY = position.y - 1;
                    startX = position.x + 1;
                    finishY = -1;
                    finishX = baseBoard.sizeX;
                    directionY = -1;
                    directionX = +1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
            
        }
    }
}