using System;
using doktorChessGameEngine;

namespace tournament
{
    public class playerException : Exception
    {
        public readonly contender culprit;
        public readonly pieceColour culpritCol;

        public playerException(contender newCulprit, pieceColour responsible)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }

        public playerException(contender newCulprit, pieceColour responsible, string why)
            : base(why)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }

        public playerException(contender newCulprit, pieceColour responsible, string why, Exception e)
            : base(why, e)
        {
            culprit = newCulprit;
            culpritCol = responsible;
        }
    }
}