using System.Collections.Generic;

namespace doktorChess
{
    public class queenSquare : square
    {
        public queenSquare(squarePos newPos, pieceColour newColour)
            :base(newPos, newColour)
        {
            type = pieceType.queen;
        }

        public override string getPieceNotation()
        {
            return "q";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {
            List<move> toRet = new List<move>(35);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown, false));

            return toRet;
        }

        public override List<move> getCoveredSquares(Board onThis)
        {
            List<move> toRet = new List<move>(35);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown, true));

            return toRet;
        }

    }
}