using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExceptionShield.Test.Strategies
{
    public static class DictionaryExtensions
    {
        public static ReadOnlyDictionary<TKey, TVal> ToReadOnlyDictionary<TKey, TVal>(
            this Dictionary<TKey, TVal> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TVal>(dictionary);
        }
    }
}