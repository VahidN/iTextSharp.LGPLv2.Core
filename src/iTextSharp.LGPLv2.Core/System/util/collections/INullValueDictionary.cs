namespace System.util;

/// <summary>
///     This custom IDictionary doesn't throw a KeyNotFoundException while accessing its value by a given key
/// </summary>
public interface INullValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    new TValue this[TKey key] { get; set; }

    INullValueDictionary<TKey, TValue> Clone();
}