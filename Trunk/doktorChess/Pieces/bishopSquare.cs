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

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown));

            return toRet;
        }
    }
}