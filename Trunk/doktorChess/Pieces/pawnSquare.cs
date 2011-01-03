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

        public override string getPieceNotation()
        {
            return "p";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {
            List<move> toRet = new List<move>(4);

            int direction;
            if (colour == pieceColour.white)
                direction = +1;
            else
                direction = -1;
            
            // We can capture upward diagonally, if immediate diagonal upward squares 
            // contain an enemy piece.
            if ((position.y + direction < Board.sizeY ) && 
                (position.y + direction > -1) )
            {
                if (position.x > 0)
                {
                    if (onThis[position.up(direction).leftOne()].containsPieceNotOfColour(colour))
                        toRet.Add(new move(onThis[position], onThis[position.up(direction).leftOne()]));
                }

                if (position.x < Board.sizeX -1 )
                {
                    if (onThis[position.up(direction).rightOne()].containsPieceNotOfColour(colour))
                        toRet.Add(new move(onThis[position], onThis[position.up(direction).rightOne()]));
                }

                // We can move forward one if that square is empty.
                if (onThis[position.up(direction)].type == pieceType.none)
                    toRet.Add(new move(onThis[position], onThis[position.up(direction)]));
                else
                    return toRet;

                // And we can move forward two if we haven't moved this piece yet, and both
                // squares are empty.
                if (movedCount == 0)
                {
                    if (position.y + (direction * 2) < Board.sizeY && 
                        position.y + (direction * 2) > -1)
                    {
                        if (onThis[position.up(direction * 2)].type == pieceType.none)
                            toRet.Add(new move(onThis[position], onThis[position.up(direction * 2)]));
                    }
                }
            }
            return toRet;
        }
    }
}