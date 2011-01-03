using System;
using System.Collections.Generic;

namespace doktorChess
{
    public class kingSquare : square
    {
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
            return findFreeOrCapturableIfOnBoard(onThis, potentialSquares);
        }
    }
}