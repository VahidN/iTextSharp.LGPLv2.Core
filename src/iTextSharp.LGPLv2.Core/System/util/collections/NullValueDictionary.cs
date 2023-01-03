namespace System.util;

/// <summary>
///     This custom IDictionary doesn't throw a KeyNotFoundException while accessing its value by a given key
/// </summary>
public interface INullValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    new TValue this[TKey key] { get; set; }

    INullValueDictionary<TKey, TValue> Clone();
}

/// <summary>
///     This custom IDictionary doesn't throw a KeyNotFoundException while accessing its value by a given key
/// </summary>
public class NullValueDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INullValueDictionary<TKey, TValue>
{
    public NullValueDictionary()
    {
    }

    public NullValueDictionary(int capacity) : base(capacity)
    {
    }

    public NullValueDictionary(IDictionary<TKey, TValue> dictionary)
        : base(dictionary, null)
    {
    }

    TValue INullValueDictionary<TKey, TValue>.this[TKey key]
    {
        get
        {
            TryGetValue(key, out var val);
            return val;
        }
        set => base[key] = value;
    }

    public INullValueDictionary<TKey, TValue> Clone() => new NullValueDictionary<TKey, TValue>(this);
}