using System;
using System.Collections.Generic;
using doktorChessGameEngine;

namespace tournament
{
    [Serializable]
    public class playedGame
    {
        public int opponentID;
        public bool isErrored;
        public bool isDraw;
        public List<move> moveList;
        public string errorMessage;
        public string opponentTypeName;
        public bool didWin;
        public Exception exception;
        public pieceColour col;
        public baseBoard board;
        public pieceColour erroredSide;
    }
}