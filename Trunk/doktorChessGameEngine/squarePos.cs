﻿using System;
using doktorChessGameEngine;

namespace doktorChessGameEngine
{
    public class squarePos
    {
        public readonly int x;
        public readonly int y;

        // Used by deserialisation code.
        public squarePos() { }

        public squarePos(int newX, int newY)
        {
            if (newX + 1 > baseBoard.sizeX || newY + 1 > baseBoard.sizeY)
                throw new ArgumentOutOfRangeException();

            x = newX;
            y = newY;
        }

        public squarePos upOne()
        {
            return up(1);
        }

        public squarePos leftOne()
        {
            return left(1);
        }

        public squarePos downOne()
        {
            return down(1);
        }

        public squarePos rightOne()
        {
            return right(1);
        }

        public squarePos up(int distance)
        {
            return new squarePos(x, y + distance);
        }

        public squarePos down(int distance)
        {
            return new squarePos(x, y - distance);
        }

        public squarePos left(int offset)
        {
            return new squarePos(x - offset, y);
        }

        public squarePos right(int offset)
        {
            return new squarePos(x + offset, y);
        }

        public bool isSameSquareAs(squarePos compareTo)
        {
            return isSameSquareAs(compareTo.x, compareTo.y);
        }

        public bool isSameSquareAs(int compareX, int compareY)
        {
            return (x == compareX && y == compareY);
        }

        public string ToString(moveStringStyle chessNotation)
        {
            switch (chessNotation)
            {
                case moveStringStyle.coord:
                    return string.Format("[ {0} , {1} ]", x, y);
                case moveStringStyle.chessNotation:
                    // Don't forget chess-style co-ords start at 1, not 0.
                    return string.Format("{0}{1}", getBoardLetter(x), y + 1);
                default:
                    throw new ArgumentOutOfRangeException("chessNotation");
            }
        }

        /// <summary>
        /// Return a letter indicating the chess notation for the indexed column
        /// </summary>
        /// <param name="x">Column to index</param>
        /// <returns>A string containing a letter</returns>
        private static string getBoardLetter(int x)
        {
            switch (x)
            {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                case 3:
                    return "D";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "G";
                case 7:
                    return "H";
                default:
                    throw new ArgumentException();
            }
        }

        public int flatten()
        {
            return flatten(x, y);
        }

        public static int flatten(int x, int y)
        {
            return x + (y * baseBoard.sizeX);
        }

        public static squarePos unflatten(int flattened)
        {
            int y = flattened / baseBoard.sizeX;
            int x = flattened % baseBoard.sizeX;

            return new squarePos(x, y);
        }

        public override string ToString()
        {
            return x + ", " + y;
        }
    }

}