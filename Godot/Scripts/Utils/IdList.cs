using System.Collections.Generic;

namespace Brutalsky.Scripts.Utils;

public class IdList<TK, TV> where TK : notnull
{
    public Dictionary<TK, TV>.KeyCollection Keys => _items.Keys;
    public Dictionary<TK, TV>.ValueCollection Values => _items.Values;

    private Dictionary<TK, TV> _items = new();

    public TV this[TK key]
    {
        get => _items[key];
        set => _items[key] = value;
    }

    public bool Add(KeyValuePair<TK, TV> item)
    {
        return Add(item.Key, item.Value);
    }

    public bool Add(TK key, TV value)
    {
        if (Contains(key)) return false;
        _items[key] = value;
        return true;
    }

    public bool Remove(TK key)
    {
        return _items.Remove(key);
    }

    public bool Contains(TK key)
    {
        return _items.ContainsKey(key);
    }
}
