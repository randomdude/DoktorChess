using System.Collections.Generic;

namespace doktorChess
{
    public class speedySquareListEnumerator : IEnumerator<int>
    {
        private readonly bool[] squares;

        private int i = -1;

        public speedySquareListEnumerator(bool[] newSquares)
        {
            squares = newSquares;
        }

        public bool MoveNext()
        {
            // Find the next set element
            do
            {
                i++;
                if (i == squares.Length)
                    return false;
            } while ( !squares[i] );

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