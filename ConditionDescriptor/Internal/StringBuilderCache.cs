using System;
using System.Text;

namespace Sag.Data.Common.Query
{
    #region class stringBuilderCache

    internal static class StringBuilderCache
    {

        private const int maxCapacity = 360;
        private const int defaultCapacity = 16; // == StringBuilder.DefaultCapacity

        [ThreadStatic]
        private static StringBuilder? t_cachedInstance;

        public static StringBuilder GetInstance(int capacity = defaultCapacity)
        {
            if (capacity <= maxCapacity)
            {
                var sb = t_cachedInstance;
                if (sb != null)
                {

                    if (capacity <= sb.Capacity)
                    {
                        t_cachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }

            return new StringBuilder(capacity);
        }

        /// <summary>ToString() the stringbuilder, Release it to the cache, and return the resulting string.</summary>
        public static string GetString(StringBuilder sb)
        {
            string result = sb.ToString();
            if (sb.Capacity <= maxCapacity)
            {
                t_cachedInstance = sb;
            }
            return result;
        }
    }

#endregion blockbase
 
}