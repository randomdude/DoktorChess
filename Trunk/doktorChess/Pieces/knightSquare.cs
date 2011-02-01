using System.Collections.Generic;

namespace doktorChess
{
    public class knightSquare : square
    {
        // We can move to these squares if they are free.
        private squarePosOffset[] potentialSquares = new squarePosOffset[]
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

        public override string getPieceNotation()
        {
            return "n";
        }

        public override List<move> getPossibleMoves(Board onThis)
        {

            return findFreeOrCapturableIfOnBoard(onThis, potentialSquares);
        }

        public override List<move> getCoveredSquares(Board parentBoard)
        {
            List<move> toRet = new List<move>(8);

            foreach (squarePosOffset potentialSquare in potentialSquares)
            {
                int posX = position.x + potentialSquare.x;
                int posY = position.y + potentialSquare.y;

                if (IsOnBoard(potentialSquare.x, potentialSquare.y))
                    toRet.Add(new move(this, parentBoard[posX, posY]));
            }

            return toRet;
        }
    }
}