namespace doktorChess
{
    public class knightSquare : square
    {
        // We can move to these squares if they are free.
        private readonly squarePosOffset[] potentialSquares = new[]
                                               {
                                                   new squarePosOffset(1,2), new squarePosOffset(2,1),
                                                   new squarePosOffset(-1,2), new squarePosOffset(-2,1),
                                                   new squarePosOffset(1,-2), new squarePosOffset(2,-1),
                                                   new squarePosOffset(-1,-2), new squarePosOffset(-2,-1)
                                               };

        public knightSquare(squarePos newPos, pieceColour newColour) : base(newPos, newColour)
        {
            type = pieceType.knight;
        }

        protected override string getPieceNotation()
        {
            return "n";
        }

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            return findFreeOrCapturableIfOnBoard(null, onThis, potentialSquares);
        }

        public override sizableArray<move> getCoveredSquares(Board parentBoard)
        {
            sizableArray<move> toRet = new sizableArray<move>(8);

            foreach (squarePosOffset potentialSquare in potentialSquares)
            {
                int posX = position.x + potentialSquare.x;
                int posY = position.y + potentialSquare.y;

                if (IsOnBoard(potentialSquare.x, potentialSquare.y))
                    toRet.Add(new move( this, parentBoard[posX, posY]));
            }

            return toRet;
        }
    }
}