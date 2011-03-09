using System;

namespace doktorChess
{
    public class ChessPlayer
    {
        public Board board;
        public Single score;

        public ChessPlayer(Board newBoard)
        {
            board = newBoard;
        }
    }
}