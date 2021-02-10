using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.Kernel.Extesions
{
    public static class MethodExtensions
    {
        public static Func<T, TResult> ThreadSafeMemoize<T, TResult>(this Func<T, TResult> f)
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            return a => cache.GetOrAdd(a, f);
        }

        public static Func<A, R> Memoize<A, R>(this Func<A, R> f)
        {
            var map = new Dictionary<A, R>();
            return a =>
            {
                if (map.TryGetValue(a, out R value))
                    return value;

                var handler = f;
                if (handler != null)
                    value = handler(a);
                map.Add(a, value);
                return value;
            };
        }

        public static Func<TParam1, TParam2, TReturn> Memoize<TParam1, TParam2, TReturn>(this Func<TParam1, TParam2, TReturn> func)
        {
            var map = new Dictionary<Tuple<TParam1, TParam2>, TReturn>();
            return (param1, param2) =>
            {
                var key = Tuple.Create(param1, param2);
                if (!map.TryGetValue(key, out TReturn result))
                {
                    var handler = func;
                    if (handler != null)
                        result = handler(param1, param2);
                    map.Add(key, result);
                }
                return result;
            };
        }
    }
}
