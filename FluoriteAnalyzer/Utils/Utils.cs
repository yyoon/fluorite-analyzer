using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FluoriteAnalyzer.Utils
{
    public static class Utils
    {
        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            T result = value;
            if (result.CompareTo(min) < 0)
            {
                result = min;
            }
            if (result.CompareTo(max) > 0)
            {
                result = max;
            }
            return result;
        }

        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (T element in source)
            {
                yield return element;

                if (predicate(element))
                {
                    yield break;
                }
            }
        }

        public static IEnumerable<T> SkipUntil<T>(this IEnumerable<T> source, Func<T, bool>  predicate)
        {
            using (var iterator = source.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    if (predicate(iterator.Current))
                    {
                        break;
                    }
                }
                while (iterator.MoveNext())
                {
                    yield return iterator.Current;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
    }
}