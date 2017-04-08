#region headers

// Copyright (c) 2017 Matthias Jansen
// See the LICENSE file in the project root for more information.

#endregion

#region imports

using System.Collections.Generic;

#endregion

namespace ExceptionManager.Extensions
{
    public static class EnumerableExtensions
    {
        public static TVal GetValueByKeyOrDefault<TKey, TVal>(this IReadOnlyDictionary<TKey, TVal> dict, TKey key)
        {
            dict.TryGetValue(key, out TVal value);
            return value;
        }
    }
}