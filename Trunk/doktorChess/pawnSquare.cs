using System.Collections.Generic;

namespace doktorChess
{
    public class pawnSquare : square
    {
        public pawnSquare(squarePos newPos, pieceColour newColour)
            :base(newPos, newColour)
        {
            type = pieceType.pawn;
        }

        public override List<move> getPossibleMoves(Board onThis )
        {
            List<move> toRet = new List<move>(2);

            // We can capture upward diagonally, if immediate diagonal upward squares 
            // contain an enemy piece.
            if (position.y < Board.sizeY - 1)
            {
                if (position.x > 0)
                {
                    if (onThis[position.upOne().leftOne()].containsPieceNotOfColour(colour))
                        toRet.Add(new move(onThis[position], onThis[position.upOne().leftOne()]));
                }

                if (position.x < Board.sizeX - 1)
                {
                    if (onThis[position.upOne().rightOne()].containsPieceNotOfColour(colour))
                        toRet.Add(new move(onThis[position], onThis[position.upOne().rightOne()]));
                }

                // We can move forward one if that square is empty.
                if (onThis[position.upOne()].type == pieceType.none)
                    toRet.Add(new move(onThis[position], onThis[position.upOne()]));
                else
                    return toRet;

                // And we can move forward two if we haven't moved this piece yet, and both
                // squares are empty.
                if (movedCount == 0)
                {
                    if (position.y < Board.sizeX - 2)
                    {
                        if (onThis[position.up(2)].type == pieceType.none)
                            toRet.Add(new move(onThis[position], onThis[position.up(2)]));
                    }
                }
            }
            return toRet;
        }
    }
}