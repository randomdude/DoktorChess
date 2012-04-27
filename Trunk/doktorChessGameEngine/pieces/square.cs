using System;
using System.Collections.Generic;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    [Serializable]
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

        /// <summary>
        /// If this piece has been promoted from a pawn, this is where its past life as a pawn lives.
        /// </summary>
        public square pastLife;

        public pieceType type { get; internal set; }
        public pieceColour colour { get; private set; }
        public squarePos position { get; set; }

        // This array of bools contains a bool for each square, which can be set to 'true' by a 
        // board implementation if it this square covered by this piece.
        // FIXME: Should really be in the theatMap itself!
        // Additionally, this cannot be serialised - meaning that it cannot pass an appdomain boundary!!
        [NonSerialized]
        public readonly speedySquareList coveredSquares = new speedySquareList();

        public bool excludeFromCastling = false;

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

// ReSharper disable MemberCanBeProtected.Global
        public square(squarePos newPos, pieceColour newColour)
        {
            movedCount = 0;
            position = newPos;
            colour = newColour;
        }
// ReSharper restore MemberCanBeProtected.Global

        public square(int newx, int newy)
        {
            movedCount = 0;
            position = new squarePos(newx, newy);
        }

        public new string ToString()
        {
            string toRet = getPieceNotation();

            if (colour == pieceColour.white)
                toRet = toRet.ToUpper();

            return toRet;
        }

        protected virtual string getPieceNotation()
        {
            return ".";
        }

        virtual public sizableArray<move> getPossibleMoves(baseBoard onThis)
        {
            // Empty squares can't move, silly.
            return new sizableArray<move>(0);
        }

        /// <summary>
        /// Find moves in a given direction, including captures
        /// </summary>
        /// <param name="addTo"></param>
        /// <param name="onThis">The board to move on</param>
        /// <param name="dir">The vectorDirection to move in</param>
        /// <returns>A List&lt;move&gt; of moves</returns>
        public sizableArray<move> getMovesForVector(sizableArray<move> addTo, baseBoard onThis, vectorDirection dir)
        {
            if (addTo == null)
                addTo = new sizableArray<move>(8);

            loopConfig lcfg = new loopConfig(position, dir);

            int x = lcfg.startX;
            int y = lcfg.startY;
            while ((x != lcfg.finishX) && (y != lcfg.finishY))
            {
                squarePos sqPos = new squarePos(x, y);

                // If the square is empty, we can move to it..
                if (onThis[sqPos].type == pieceType.none)
                {
                    addTo.Add(new move(onThis[position], onThis[sqPos]));
                }
                else
                {
                    if (onThis[sqPos].colour != colour )
                    {
                        // the square is occupied by an enemy piece. we can move to it, 
                        // but no further.
                        addTo.Add(new move(onThis[position], onThis[sqPos]));
                    }
                    break;
                }

                x += lcfg.directionX;
                y += lcfg.directionY;
            }

            return addTo;
        }

        protected void getSquaresCoveredForVector(sizableArray<square> addTo, baseBoard onThis, vectorDirection dir)
        {
            if (addTo == null)
                addTo = new sizableArray<square>(8);

            loopConfig lcfg = new loopConfig(position, dir);

            int x = lcfg.startX;
            int y = lcfg.startY;
            while ((x != lcfg.finishX) && (y != lcfg.finishY))
            {
                squarePos sqPos = new squarePos(x, y);

                // If the square is empty, we can move to it..
                if (onThis[sqPos].type == pieceType.none)
                {
                    addTo.Add( onThis[sqPos] );
                }
                else
                {
                    // the square is occupied by some piece. We are covering it, but we cannot go any further.
                    addTo.Add( onThis[sqPos] );
                    break;
                }

                x += lcfg.directionX;
                y += lcfg.directionY;
            }

            return;
        }

        public bool containsPieceNotOfColour(pieceColour ourColour)
        {
            return ((type != pieceType.none) && (colour != ourColour));
        }

        protected sizableArray<move> findFreeOrCapturableIfOnBoard(sizableArray<move> returnArray, baseBoard onThis, squarePosOffset[] potentialSquareOffsets)
        {
            if (returnArray == null)
                returnArray = new sizableArray<move>(potentialSquareOffsets.Length);

            foreach (squarePosOffset potentialSquareOffset in potentialSquareOffsets)
            {
                if (!IsOnBoard(potentialSquareOffset.x, potentialSquareOffset.y))
                    continue;

                square destSquare = onThis[potentialSquareOffset.x + position.x, potentialSquareOffset.y + position.y];

                if (destSquare.type == pieceType.none)
                {
                    // Square is free.
                    returnArray.Add(new move(this, destSquare));
                }
                else
                {
                    if (destSquare.colour != this.colour)
                    {
                        // We can capture.
                        returnArray.Add(new move(this, destSquare));
                    }
                }
            }

            return returnArray;
        }

        protected bool IsOnBoard(int x, int y)
        {
            int offX = x + position.x;
            int offY = y + position.y;

            return (!(offX > baseBoard.sizeX - 1 ||
                       offX < 0 ||
                       offY > baseBoard.sizeY - 1 ||
                       offY < 0));
        }

        public virtual sizableArray<square> getCoveredSquares(baseBoard parentBoard)
        {
            return new sizableArray<square>(0);
        }

    }
}