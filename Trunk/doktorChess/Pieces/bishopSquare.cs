using System;
using System.Collections.Generic;

namespace doktorChess
{
    public class bishopSquare : square
    {
        public bishopSquare(squarePos newPos, pieceColour newColour) : base(newPos, newColour)
        {
            type = pieceType.bishop;
        }

        public override string getPieceNotation()
        {
            return "b";
        }

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.leftup, false);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown, false);
            getMovesForVector(toRet, onThis, vectorDirection.rightup, false);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown, false);

            return toRet;
        }

        public override sizableArray<move> getCoveredSquares(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.leftup, true);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown, true);
            getMovesForVector(toRet, onThis, vectorDirection.rightup, true);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown, true);

            return toRet;
        }
    }
}