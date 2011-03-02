namespace doktorChess
{
    public class kingSquare : square
    {
        // We can move to these squares if they are free.
        private readonly squarePosOffset[] potentialSquares = new[] {
                    new squarePosOffset(-1, +1), new squarePosOffset( 0, +1), new squarePosOffset(+1, +1),
                    new squarePosOffset(-1,  0), new squarePosOffset(+1,  0),
                    new squarePosOffset(-1, -1), new squarePosOffset( 0, -1), new squarePosOffset(+1, -1)};


        public kingSquare(squarePos newPos, pieceColour newColour)
            : base(newPos, newColour)
        {
            type = pieceType.king;
        }

        protected override string getPieceNotation()
        {
            return "k";
        }

        public override sizableArray<move> getPossibleMoves(Board onThis)
        {
            sizableArray<move> possibleMoves = new sizableArray<move>(potentialSquares.Length + 2);

            findFreeOrCapturableIfOnBoard(possibleMoves, onThis, potentialSquares);

            if (canCastle(onThis, true))
            {
                // Castling kingside is possible. It is represented as a two-space move by the king.
                possibleMoves.Add(new move(this, onThis[position.right(2)]));
            }
            if (canCastle(onThis, false))
            {
                // Likewise, queenside.
                possibleMoves.Add(new move(this, onThis[position.left(2)]));
            }

            return possibleMoves;
        }

        public override sizableArray<square> getCoveredSquares(Board parentBoard)
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

        private bool canCastle(Board theBoard, bool kingSide)
        {
            // We can castle under specific circumstances:
            // * Our king has not moved
            // * A rook is on the same rank as the king and has not moved
            // * The squares between the king and the rook are empty
            // * The first two spaces between the king and the rook are not threatened by anything (the rook can move through 'check' but the king cannot)

            // If we are castling kingside, examine row 7 - otherwise, row 0.
            square potentialRookSquare = theBoard[ kingSide ? 7 : 0 , position.y];

            if (movedCount == 0                                 // King has not moved
                && potentialRookSquare.type == pieceType.rook   // Rook is present
                && potentialRookSquare.movedCount == 0)         // Rook has not moved
            {
                // Verify that king spaces are free
                int startx;
                int limitx;
                if (kingSide)
                {
                    startx = position.x + 1;
                    limitx = potentialRookSquare.position.x - 1;
                }
                else
                {
                    limitx = position.x - 1;
                    startx = potentialRookSquare.position.x + 1;
                }

                for (int n = startx ; n < limitx + 1; n++)
                {
                    square perhapsEmpty = theBoard[ n, position.y ];
                    if (perhapsEmpty.type != pieceType.none)
                    {
                        // Intermediate squares are occupied - castling is impossible.
                        return false;
                    }
                }

                // Verify that two spaces next to the king are not threatened by the opposing side
                for (int n = 1; n < 3; n++)
                {
                    square perhapsThreatened = theBoard[ kingSide ? position.right(n) : position.left(n)];
                    if ( theBoard.isThreatened(perhapsThreatened, colour)  )
                    {
                        // Intermediate squares are threatened - castling is impossible.
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}