using System.Collections;

namespace System.util
{
    /// <summary>
    /// Summary description for ListIterator.
    /// </summary>
    public class ListIterator
    {
        readonly ArrayList _col;
        int _cursor;
        int _lastRet = -1;

        public ListIterator(ArrayList col)
        {
            _col = col;
        }

        public bool HasNext()
        {
            return _cursor != _col.Count;
        }

        public object Next()
        {
            object next = _col[_cursor];
            _lastRet = _cursor++;
            return next;
        }

        public object Previous()
        {
            int i = _cursor - 1;
            object previous = _col[i];
            _lastRet = _cursor = i;
            return previous;
        }

        public void Remove()
        {
            if (_lastRet == -1)
                throw new InvalidOperationException();
            _col.RemoveAt(_lastRet);
            if (_lastRet < _cursor)
                _cursor--;
            _lastRet = -1;
        }
    }
}
