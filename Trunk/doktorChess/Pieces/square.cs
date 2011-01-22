using System;
using System.Collections.Generic;

namespace doktorChess
{
    public class square
    {
        /// <summary>
        /// The amount of times this piece has moved
        /// </summary>
        public int movedCount;

        /// <summary>
        /// The moves made by this piece on the parent board
        /// </summary>
        public Stack<int> moveNumbers = new Stack<int>(50);

        public pieceType type { get; protected set; }
        public pieceColour colour { get; private set; }
        public squarePos position { get; set; }

        public static square makeSquare(pieceType newType, pieceColour newColour, squarePos newPos)
        {
            square toRet ;
            switch (newType)
            {
                case pieceType.none:
                    toRet = new square(newPos);
                    break;
                case pieceType.pawn:
                    toRet = new pawnSquare(newPos, newColour);
                    break;
                case pieceType.rook:
                    toRet = new rookSquare(newPos, newColour);
                    break;
                case pieceType.bishop:
                    toRet = new bishopSquare(newPos, newColour);
                    break;
                case pieceType.knight:
                    toRet = new knightSquare(newPos, newColour);
                    break;
                case pieceType.queen:
                    toRet = new queenSquare(newPos, newColour);
                    break;
                case pieceType.king:
                    toRet = new kingSquare(newPos, newColour);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return toRet;
        }

        public square(squarePos newPos)
        {
            movedCount = 0;
            position = newPos;
            type = pieceType.none;
        }

        public square(squarePos newPos, pieceColour newColour)
        {
            movedCount = 0;
            position = newPos;
            colour = newColour;
        }

        public square(int newx, int newy)
        {
            movedCount = 0;
            position = new squarePos(newx, newy);
        }

        public new string ToString()
        {
            string toRet = getPieceNotation();

            if (colour == pieceColour.black)
                toRet = toRet.ToUpper();

            return toRet;
        }

        public virtual string getPieceNotation()
        {
            return ".";
        }

        virtual public List<move> getPossibleMoves(Board onThis)
        {
            // Empty squares can't move, silly.
            return new List<move>();
        }

        /// <summary>
        /// Find moves in a given direction, including captures
        /// </summary>
        /// <param name="onThis">The board to move on</param>
        /// <param name="dir">The vectorDirection to move in</param>
        /// <returns>A List&lt;move&gt; of moves</returns>
        public List<move> getMovesForVector(Board onThis, vectorDirection dir)
        {
            List<move> toRet = new List<move>(8);

            int startX;
            int finishX;
            int startY;
            int finishY;
            int directionX;
            int directionY;

            switch (dir)
            {
                case vectorDirection.left:
                    startX = position.x - 1;
                    finishX = -1;
                    startY = position.y;
                    finishY = startY + 1;
                    directionX = -1;
                    directionY = 0;
                    break;
                case vectorDirection.right:
                    startX = position.x + 1;
                    finishX = Board.sizeX;
                    startY = position.y;
                    finishY = startY + 1;
                    directionX = +1;
                    directionY = 0;
                    break;
                case vectorDirection.up:
                    startY = position.y + 1;
                    finishY = Board.sizeY;
                    startX = position.x;
                    finishX = position.x + 1;
                    directionX = 0;
                    directionY = +1;
                    break;
                case vectorDirection.down:
                    startY = position.y - 1;
                    finishY = -1;
                    startX = position.x;
                    finishX = position.x+1;
                    directionX = 0;
                    directionY = -1;
                    break;
                case vectorDirection.leftup:
                    startY = position.y + 1;
                    startX = position.x - 1;
                    finishY = Board.sizeY;
                    finishX = -1;
                    directionY = +1;
                    directionX = -1;
                    break;
                case vectorDirection.leftdown:
                    startY = position.y - 1;
                    startX = position.x - 1;
                    finishY = -1;
                    finishX = -1;
                    directionY = -1;
                    directionX = -1;
                    break;
                case vectorDirection.rightup:
                    startY = position.y + 1;
                    startX = position.x + 1;
                    finishY = Board.sizeY;
                    finishX = Board.sizeX;
                    directionY = +1;
                    directionX = +1;
                    break;
                case vectorDirection.rightdown:
                    startY = position.y - 1;
                    startX = position.x + 1;
                    finishY = -1;
                    finishX = Board.sizeX;
                    directionY = -1;
                    directionX = +1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }

            int x = startX;
            int y = startY;
            while ((x != finishX) && (y != finishY))
            {
                squarePos sqPos = new squarePos(x, y);

                // If the square is empty, we can move to it..
                if (onThis[sqPos].type == pieceType.none)
                {
                    toRet.Add(new move(onThis[position], onThis[sqPos]));
                }
                else
                {
                    if (onThis[sqPos].colour == colour )
                    {
                        // the square is occupied by one of our pieces, we cannot move past
                        // it.
                        break;
                    }
                    else
                    {
                        // the square is occupied by an enemy piece, we can move to it, 
                        // but no further.
                        toRet.Add(new move(onThis[position], onThis[sqPos]));
                        break;
                    }
                }

                x += directionX;
                y += directionY;
            }

            return toRet;
        }

        public bool containsPieceNotOfColour(pieceColour ourColour)
        {
            return ((type != pieceType.none) && (colour != ourColour));
        }

        protected List<move> findFreeOrCapturableIfOnBoard(Board onThis, squarePosOffset[] potentialSquareOffsets)
        {
            List<move> toRet = new List<move>(potentialSquareOffsets.Length);

            foreach (squarePosOffset potentialSquareOffset in potentialSquareOffsets)
            {
                if (potentialSquareOffset.x + position.x > Board.sizeX - 1 ||
                    potentialSquareOffset.x + position.x < 0  ||
                    potentialSquareOffset.y + position.y > Board.sizeY - 1||
                    potentialSquareOffset.y + position.y < 0 )
                    continue;

                square destSquare = onThis[potentialSquareOffset.x + position.x, potentialSquareOffset.y + position.y];

                if (destSquare.type == pieceType.none)
                {
                    // Square is free.
                    toRet.Add(new move(this, destSquare));
                }
                else
                {
                    if (destSquare.colour != this.colour)
                    {
                        // We can capture.
                        toRet.Add(new move(this, destSquare));
                    }
                }
            }

            return toRet;
        }
    }

    public class squarePosOffset
    {
        public readonly int x;
        public readonly int y;

        public squarePosOffset(int newX, int newY)
        {
            x = newX;
            y = newY;
        }
    }
}