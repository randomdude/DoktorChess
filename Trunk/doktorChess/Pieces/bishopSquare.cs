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

        public override List<move> getPossibleMoves(Board onThis)
        {
            List<move> toRet = new List<move>(20);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown, false));

            return toRet;
        }

        public override List<move> getCoveredSquares(Board onThis)
        {
            List<move> toRet = new List<move>(20);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown, true));

            return toRet;
        }
    }
}