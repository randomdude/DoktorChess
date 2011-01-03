using System.Collections.Generic;

namespace doktorChess
{
    public class rookSquare : square
    {
        public rookSquare(squarePos newPos, pieceColour newColour)
            : base(newPos, newColour)
        {
            type = pieceType.rook;
        }

        public override string getPieceNotation()
        {
            return "r";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {
            List<move> toRet = new List<move>(20);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down));

            return toRet;
        }
    }
}