using System.Collections.Generic;

namespace doktorChess
{
    public class knightSquare : square
    {
        public knightSquare(squarePos newPos, pieceColour newColour) : base(newPos, newColour)
        {
            type = pieceType.knight;
        }

        public override string getPieceNotation()
        {
            return "n";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {
            // We can move to these squares if they are free.
            squarePosOffset[] potentialSquares = new squarePosOffset[]
                                               {
                                                   new squarePosOffset(1,2), new squarePosOffset(2,1),
                                                   new squarePosOffset(-1,2), new squarePosOffset(-2,1),
                                                   new squarePosOffset(1,-2), new squarePosOffset(2,-1),
                                                   new squarePosOffset(-1,-2), new squarePosOffset(-2,-1)
                                               };

            return findFreeOrCapturableIfOnBoard(onThis, potentialSquares);
        }
    }
}