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

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up, false));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down, false));

            return toRet;
        }

        public override List<move> getCoveredSquares(Board onThis)
        {
            List<move> toRet = new List<move>(20);

            toRet.AddRange(getMovesForVector(onThis, vectorDirection.left, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.up, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.right, true));
            toRet.AddRange(getMovesForVector(onThis, vectorDirection.down, true));

            return toRet;
        }    
    }
}