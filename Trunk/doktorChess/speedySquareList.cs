using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace doktorChess
{
    /// <summary>
    /// This 'list' contains a grid of squarePos, which are enumerable as flat co-ordinates of set values.
    /// </summary>
    public class speedySquareList : IEnumerable<int>
    {
        // This array of squares contains a each square or null.
        private bool[] squares = new bool[Board.sizeX * Board.sizeY];

        // We keep a linked-list of set squares so that we can iterate really quickly over them.
        private LinkedList<int> llTrueSquares = new LinkedList<int>();
        // And because we're really performance-critical, we have a lookup of flattened squarePos to linkedList
        // elements so we don't need to search our linked list.
        private LinkedListNode<int>[] llTrueSquareIndexes = new LinkedListNode<int>[Board.sizeX * Board.sizeY]; 

        private int count = 0;

        public int Count
        {
            get { return count; }
        }

        public bool this[int x, int y]
        {
            get { return this[squarePos.flatten(x, y)]; }
            set { Set(squarePos.flatten(x, y), value); }
        }

        public bool this[squarePos pos]
        {
            get { return this[pos.flatten()]; }
            set { Set(pos.flatten(), value); }
        }

        public bool this[int flattenedPos]
        {
            get { return squares[flattenedPos]; }
            set { Set(flattenedPos, value); }
        }

        public void Clear()
        {
            squares = new bool[Board.sizeX * Board.sizeY];
            llTrueSquares.Clear();
            count = 0;
        }

        private void Set(int flattenedPos, bool newVal)
        {
#if DEBUG
            if (squares[flattenedPos] == newVal)
                throw new AssertFailedException("Speedylist entry duplicated - is that OK?");
#endif

            if (newVal)
            {
                count++;
                llTrueSquareIndexes[flattenedPos] = llTrueSquares.AddLast(flattenedPos);
            }
            else
            {
                count--;
                llTrueSquares.Remove(llTrueSquareIndexes[flattenedPos]);
            }

            squares[flattenedPos] = newVal;
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return llTrueSquares.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return llTrueSquares.GetEnumerator();
        }
    }
}