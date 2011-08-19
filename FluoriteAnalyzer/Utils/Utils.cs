using System;

namespace FluoriteAnalyzer.Utils
{
    public class Utils
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
    }
}