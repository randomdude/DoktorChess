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

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.left, false);
            getMovesForVector(toRet, onThis, vectorDirection.down, false);
            getMovesForVector(toRet, onThis, vectorDirection.right, false);
            getMovesForVector(toRet, onThis, vectorDirection.up, false);

            return toRet;
        }

        public override sizableArray<move> getCoveredSquares(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.left, true);
            getMovesForVector(toRet, onThis, vectorDirection.down, true);
            getMovesForVector(toRet, onThis, vectorDirection.right, true);
            getMovesForVector(toRet, onThis, vectorDirection.up, true);

            return toRet;
        }    
    }
}