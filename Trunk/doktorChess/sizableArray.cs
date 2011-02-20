using System;
using System.Collections;

namespace doktorChess
{
    public class sizableArray<T> : IEnumerable  
    {
        private readonly T[] elements;

        private int cursor = 0;

        public sizableArray(int newSize)
        {
            elements = new T[newSize];
        }

        public int Length
        {
            get { return cursor; }
        }

        public void Clear()
        {
            cursor = 0;
        }

        public void Add(T newItem)
        {
            elements[cursor] = newItem;
            cursor++;
        }

        public T[] getArray()
        {
            T[] nonNulls = Array.FindAll(elements, a => a != null);

            return nonNulls;
        }

        public void AddRange(IEnumerable toAdd)
        {
            foreach (T thisToAdd in toAdd)
                Add(thisToAdd);
        }

        public IEnumerator GetEnumerator()
        {
            return new sizableArrayEnumerator<T>(this);
        }

        public bool Exists(Predicate<T> match)
        {
            T[] nonNulls = Array.FindAll(elements, a => a != null);

            return (Array.Exists<T>(nonNulls, match));
        }

        public void bringToPosition(int posToMove, int newPos)
        {
            T tmp = elements[newPos];               // save old element
            elements[newPos] = elements[posToMove]; // set new element to old
            elements[posToMove] = tmp;              // set old element to our copy of new
        }

        public T this[int i]
        {
            get { return elements[i]; }
        }
    }

    public class sizableArrayEnumerator<T> : IEnumerator
    {
        private readonly sizableArray<T> _parent;
        private int i = -1;

        public sizableArrayEnumerator(sizableArray<T> parent)
        {
            _parent = parent;
        }

        public bool MoveNext()
        {
            i++;
            if (i == _parent.Length)
                return false;

            return true;
        }

        public void Reset()
        {
            i = -1;
        }

        public object Current
        {
            get { return _parent[i]; }
        }
    }
}