namespace System.util;

/// <summary>
///     Summary description for ListIterator.
/// </summary>
public class ListIterator<T>
{
    private readonly IList<T> _col;
    private int _cursor;
    private int _lastRet = -1;

    public ListIterator(IList<T> col) => _col = col;

    public bool HasNext() => _cursor != _col.Count;

    public T Next()
    {
        var next = _col[_cursor];
        _lastRet = _cursor++;
        return next;
    }

    public T Previous()
    {
        var i = _cursor - 1;
        var previous = _col[i];
        _lastRet = _cursor = i;
        return previous;
    }

    public void Remove()
    {
        if (_lastRet == -1)
        {
            throw new InvalidOperationException();
        }

        _col.RemoveAt(_lastRet);
        if (_lastRet < _cursor)
        {
            _cursor--;
        }

        _lastRet = -1;
    }
}