using doktorChessGameEngine;

namespace doktorChessGameEngine
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

        public override sizableArray<move> getPossibleMoves(baseBoard onThis)
        {
            return findFreeOrCapturableIfOnBoard(null, onThis, potentialSquares);
        }

        public override sizableArray<square> getCoveredSquares(baseBoard parentBoard)
        {
            sizableArray<square> toRet = new sizableArray<square>(8);

            foreach (squarePosOffset potentialSquare in potentialSquares)
            {
                int posX = position.x + potentialSquare.x;
                int posY = position.y + potentialSquare.y;

                if (IsOnBoard(potentialSquare.x, potentialSquare.y))
                    toRet.Add( parentBoard[posX, posY] );
            }

            return toRet;
        }
    }
}