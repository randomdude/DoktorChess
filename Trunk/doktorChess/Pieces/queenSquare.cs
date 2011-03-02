namespace doktorChess
{
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

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(35);

            getMovesForVector(toRet, onThis, vectorDirection.left, false);
            getMovesForVector(toRet, onThis, vectorDirection.down, false);
            getMovesForVector(toRet, onThis, vectorDirection.right, false);
            getMovesForVector(toRet, onThis, vectorDirection.up, false);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown, false);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown, false);
            getMovesForVector(toRet, onThis, vectorDirection.leftup, false);
            getMovesForVector(toRet, onThis, vectorDirection.rightup, false);

            return toRet;
        }

        public override sizableArray<move> getCoveredSquares(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(35);

            getMovesForVector(toRet, onThis, vectorDirection.left, true);
            getMovesForVector(toRet, onThis, vectorDirection.down, true);
            getMovesForVector(toRet, onThis, vectorDirection.right, true);
            getMovesForVector(toRet, onThis, vectorDirection.up, true);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown, true);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown, true);
            getMovesForVector(toRet, onThis, vectorDirection.leftup, true);
            getMovesForVector(toRet, onThis, vectorDirection.rightup, true);

            return toRet;
        }

    }
}