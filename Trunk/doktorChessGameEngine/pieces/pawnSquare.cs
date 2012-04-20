using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    public class pawnSquare : square
    {
        public pawnSquare(squarePos newPos, pieceColour newColour)
            :base(newPos, newColour)
        {
            type = pieceType.pawn;
        }

        protected override string getPieceNotation()
        {
            return "p";
        }

        public override sizableArray<move> getPossibleMoves(baseBoard onThis)
        {
            sizableArray<move> toRet = new sizableArray<move>(40);

            int direction;
            if (colour == pieceColour.white)
                direction = +1;
            else
                direction = -1;
            
            // We can capture upward diagonally, if immediate diagonal upward squares 
            // contain an enemy piece.
            if ((position.y + direction < baseBoard.sizeY) && 
                (position.y + direction > -1) )
            {
                // Check for en passant. En passant can never cause a promotion.
                if (position.x > 0)
                {
                    square adjacentLeft = onThis[position.leftOne()];
                    if (canEnPassantTo(adjacentLeft, onThis))
                        toRet.Add(new move(onThis[position], onThis[position.up(direction).leftOne()], adjacentLeft));
                }
                if (position.x < baseBoard.sizeX - 1)
                {
                    square adjacentRight = onThis[position.rightOne()];
                    if (canEnPassantTo(adjacentRight, onThis))
                        toRet.Add(new move(onThis[position], onThis[position.up(direction).rightOne()], adjacentRight));
                }

                // And we can move forward two if we haven't moved this piece yet, and both
                // squares are empty. It is assumed that this can't cause a promotion, so explicitly
                // prevent this move from moving in to the back row.
                if (movedCount == 0)
                {
                    if (position.y + (direction * 2) < baseBoard.sizeY &&
                        position.y + (direction * 2) > -1)
                    {
                        // check back row
                        if (position.y != (colour == pieceColour.white ? 7 : 0))
                        {
                            if (onThis[position.up(direction * 2)].type == pieceType.none &&
                                onThis[position.up(direction * 1)].type == pieceType.none)
                                toRet.Add(new move(onThis[position], onThis[position.up(direction*2)]));
                        }
                    }
                }

                // All of the other moves could cause a promotion, so call addPawnMovesToSquare so that
                // promoting moves are added.

                // Check the two diagonals
                if (position.x > 0)
                {
                    if (onThis[position.up(direction).leftOne()].containsPieceNotOfColour(colour))
                        addPawnMovesToSquare(toRet, onThis[position], onThis[position.up(direction).leftOne()]);
                }
                if (position.x < baseBoard.sizeX - 1)
                {
                    if (onThis[position.up(direction).rightOne()].containsPieceNotOfColour(colour))
                        addPawnMovesToSquare(toRet, onThis[position], onThis[position.up(direction).rightOne()]);
                }

                // We can move forward one if that square is empty.
                if (onThis[position.up(direction)].type == pieceType.none)
                {
                    if (onThis[position.up(direction)].type == pieceType.none)
                        addPawnMovesToSquare(toRet, onThis[position], onThis[position.up(direction)]);
                }
            }

            return toRet;
        }

        private void addPawnMovesToSquare(sizableArray<move> moveList, square src, square dst)
        {
            // If we are moving in to the end row, we should promote. Handle this.
            if (dst.position.y == (colour == pieceColour.white ? 7 : 0) )
            {
                // OK. Promotions it is.
                pieceType[] promotionOptions = new[] {
                                                      pieceType.queen,
                                                      pieceType.rook,
                                                      pieceType.knight,
                                                      pieceType.bishop
                                                  };

                foreach (pieceType promotionOption in promotionOptions)
                {
                    moveList.Add(new move(src, dst, promotionOption));
                }
            }
            else
            {
                moveList.Add(new move(src, dst));                
            }
        }

        private bool canEnPassantTo(square adjacent, baseBoard theBoard)
        {
            // We can capture via en passant if:
            // * An enemy pawn is on our right/left
            // * The enemy pawn has moved only once
            // * The enemy pawn moved last move
            // * The enemy pawn (and us) are on the 4th/5th rank (according to player colour)
            // * The space behind the enemy pawn is empty.
            if (        adjacent.containsPieceNotOfColour(colour)          // Is an enemy piece
                    && adjacent.type == pieceType.pawn                 // Is an enemy pawn
                    && adjacent.movedCount == 1                        // Has moved only once
                    && adjacent.moveNumbers.Peek() == theBoard.moveCount-1 // Moved last move
                    && adjacent.position.y == (colour == pieceColour.white ? 4 : 3) // is on start row
                    && theBoard[ adjacent.position.up(colour == pieceColour.white ? 1 : -1) ].type == pieceType.none )
                return true;

            return false;
        }

        public override sizableArray<square> getCoveredSquares(baseBoard parentBoard)
        {
            sizableArray<square> toRet = new sizableArray<square>(2);
            int direction = (colour == pieceColour.white) ? 1 : -1;

            if ((position.y + direction < baseBoard.sizeY) &&
                (position.y + direction > -1))
            {
                // Check the two diagonals
                if (position.x > 0)
                {
                    toRet.Add( parentBoard[position.up(direction).leftOne()] );
                }
                if (position.x < baseBoard.sizeX - 1)
                {
                    toRet.Add( parentBoard[position.up(direction).rightOne()] );
                }
            }
            return toRet;
        }
    }
}