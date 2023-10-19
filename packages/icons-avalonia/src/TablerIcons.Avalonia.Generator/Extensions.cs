using System;
using System.Collections.Generic;
using System.Text;

namespace TablerIcons.Avalonia.Generator
{
    internal static class DictionaryExtensions
    {
        public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
