using System;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    [Serializable]
    public class queenSquare : square
    {
        public queenSquare(squarePos newPos, pieceColour newColour)
            :base(newPos, newColour)
        {
            type = pieceType.queen;
        }

        protected override string getPieceNotation()
        {
            return "q";
        }

        public override sizableArray<move> getPossibleMoves(baseBoard onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(35);

            getMovesForVector(toRet, onThis, vectorDirection.left);
            getMovesForVector(toRet, onThis, vectorDirection.down);
            getMovesForVector(toRet, onThis, vectorDirection.right);
            getMovesForVector(toRet, onThis, vectorDirection.up);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown);
            getMovesForVector(toRet, onThis, vectorDirection.leftup);
            getMovesForVector(toRet, onThis, vectorDirection.rightup);

            return toRet;
        }

        public override sizableArray<square> getCoveredSquares(baseBoard onThis)
        {
            sizableArray<square> toRet = new sizableArray<square>(35);

            getSquaresCoveredForVector(toRet, onThis, vectorDirection.left);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.down);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.right);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.up);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.leftdown);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.rightdown);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.leftup);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.rightup);

            return toRet;
        }

    }
}