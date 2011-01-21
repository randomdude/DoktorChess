using System;
using System.Collections.Generic;

namespace doktorChess
{
    public class kingSquare : square
    {
        public bool inhibitCastling;

        public kingSquare(squarePos newPos, pieceColour newColour)
            : base(newPos, newColour)
        {
            type = pieceType.king;
        }

        public override string getPieceNotation()
        {
            return "k";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {
            // We can move to these squares if they are free.
            squarePosOffset[] potentialSquares = new squarePosOffset[] {    new squarePosOffset(-1, +1), new squarePosOffset( 0, +1), new squarePosOffset(+1, +1),
                                                                            new squarePosOffset(-1,  0), new squarePosOffset(+1,  0),
                                                                            new squarePosOffset(-1, -1), new squarePosOffset( 0, -1), new squarePosOffset(+1, -1)};
            List<move> possibleMoves = findFreeOrCapturableIfOnBoard(onThis, potentialSquares);

            if (canCastle(onThis, true))
            {
                // Castling kingside is possible. It is represented as a two-space move by the king.
                possibleMoves.Add(new move(this, onThis[ position.right(2) ] ));
            }
            if (canCastle(onThis, false))
            {
                // Likewise, queenside.
                possibleMoves.Add(new move(this, onThis[position.left(2)]));
            }

            return possibleMoves;
        }

        private bool canCastle(Board theBoard, bool kingSide)
        {
            // We can castle under specific circumstances:
            // * Our king has not moved
            // * A rook is on the same rank as the king and has not moved
            // * The squares between the king and the rook are empty
            // * The first two spaces between the king and the rook are not threatened by anything (the rook can move through 'check' but the king cannot)
            
            if (inhibitCastling)
                // This is set by other logic so we don't return a castling move even if one is possible.
                return false;

            // If we are castling kingside, examine row 7 - otherwise, row 0.
            square potentialRookSquare = theBoard[ kingSide ? 7 : 0 , position.y];

            if (movedCount == 0                                 // King has not moved
                && potentialRookSquare.type == pieceType.rook   // Rook is present
                && potentialRookSquare.movedCount == 0)         // Rook has not moved
            {
                // Verify that king spaces are free
                int startx = 0;
                int limitx = 0;
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
                    if ( theBoard.getCoverLevel(perhapsThreatened, Board.getOtherSide(colour) ) > 0 )
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