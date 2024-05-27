using System.Collections.Generic;

namespace Utils
{
    public class IdList<T> where T : IHasId
    {
        public Dictionary<string, T>.KeyCollection Keys => _items.Keys;
        public Dictionary<string, T>.ValueCollection Values => _items.Values;

        private Dictionary<string, T> _items = new();

        public T this[string key] => _items[key];

        public bool Add(T value)
        {
            if (Contains(value.Id)) return false;
            _items[value.Id] = value;
            return true;
        }

        public bool Replace(T value)
        {
            if (!Contains(value.Id)) return false;
            _items[value.Id] = value;
            return true;
        }

        public bool Remove(T value)
        {
            return Remove(value.Id);
        }

        public bool Remove(string key)
        {
            return _items.Remove(key);
        }

        public bool Contains(T value)
        {
            return Contains(value.Id);
        }

        public bool Contains(string key)
        {
            return _items.ContainsKey(key);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
