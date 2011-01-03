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

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftup));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.leftdown));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightup));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.rightdown));

            return toRet;
        }

    }
}