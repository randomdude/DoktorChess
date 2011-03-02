namespace doktorChess
{
    public class bishopSquare : square
    {
        public bishopSquare(squarePos newPos, pieceColour newColour) : base(newPos, newColour)
        {
            type = pieceType.bishop;
        }

        protected override string getPieceNotation()
        {
            return "b";
        }

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(20);

            getMovesForVector(toRet, onThis, vectorDirection.leftup);
            getMovesForVector(toRet, onThis, vectorDirection.leftdown);
            getMovesForVector(toRet, onThis, vectorDirection.rightup);
            getMovesForVector(toRet, onThis, vectorDirection.rightdown);

            return toRet;
        }

        public override sizableArray<square> getCoveredSquares(Board onThis)
        {
            sizableArray<square> toRet = new sizableArray<square>(20);

            getSquaresCoveredForVector(toRet, onThis, vectorDirection.leftup);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.leftdown);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.rightup);
            getSquaresCoveredForVector(toRet, onThis, vectorDirection.rightdown);

            return toRet;
        }
    }
}