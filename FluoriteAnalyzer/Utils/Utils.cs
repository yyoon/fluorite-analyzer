using System;
using System.Collections.Generic;
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
                    break;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
    }
}