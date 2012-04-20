using System.Collections.Generic;

namespace doktorChessGameEngine
{
    public class speedySquareListEnumerator : IEnumerator<int>
    {
        private readonly bool[] _squares;

        private int i = -1;

        public speedySquareListEnumerator(bool[] newSquares)
        {
            _squares = newSquares;
        }

        public bool MoveNext()
        {
            // Find the next set element
            do
            {
                i++;
                if (i == _squares.Length)
                    return false;
            } while ( !_squares[i] );

            return true;
        }

        public void Reset()
        {
            i = -1;
        }

        public object Current
        {
            get { return i; }
        }

        int IEnumerator<int>.Current
        {
            get { return i; }
        }

        public void Dispose() { }
    }
}