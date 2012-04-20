using System;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    public class ChessPlayer
    {
        public readonly baseBoard board;
        public Single score;

        public ChessPlayer(baseBoard newBoard)
        {
            board = newBoard;
        }
    }
}