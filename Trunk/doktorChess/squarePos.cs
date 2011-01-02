using System;

namespace doktorChess
{
    public class squarePos
    {
        public int x;
        public int y;

        public squarePos(int newX, int newY)
        {
            if (newX+1 > Board.sizeX || newY+1 > Board.sizeY)
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
            return (x == compareTo.x && y == compareTo.y);
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
    }
}