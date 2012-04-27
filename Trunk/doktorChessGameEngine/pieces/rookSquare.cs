using System;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    [Serializable]
    public class rookSquare : square
    {
        public rookSquare(squarePos newPos, pieceColour newColour)
            : base(newPos, newColour)
        {
            type = pieceType.rook;
        }

        protected override string getPieceNotation()
        {
            return "r";
        }

        public override sizableArray<move> getPossibleMoves(baseBoard onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.left);
            getMovesForVector(toRet, onThis, vectorDirection.down);
            getMovesForVector(toRet, onThis, vectorDirection.right);
            getMovesForVector(toRet, onThis, vectorDirection.up);

            return toRet;
        }

        public override sizableArray<square> getCoveredSquares(baseBoard onThis)
        {
            sizableArray<square> toRet = new sizableArray<square>(20);

            getSquaresCoveredForVector(toRet, onThis, vectorDirection.left);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.down);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.right);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.up);

            return toRet;
        }    
    }
}