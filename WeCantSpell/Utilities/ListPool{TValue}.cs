using System;
using System.Collections.Generic;

namespace WeCantSpell.Utilities
{
    internal static class ListPool<TValue>
    {
        private const int MaxCachedListCount = 20;

        [ThreadStatic]
        private static List<TValue> Cache;

        public static List<TValue> Get()
        {
            var result = Steal(ref Cache);
            if (result == null)
            {
                result = new List<TValue>();
            }

            result.Clear();
            return result;
        }

        public static List<TValue> GetWithMinimumCapacity(int minCapacity)
        {
            var result = Steal(ref Cache, minCapacity);
            if (result == null)
            {
                result = new List<TValue>(minCapacity);
            }

            result.Clear();
            return result;
        }

        public static void Return(List<TValue> list)
        {
            if (list == null || list.Count > MaxCachedListCount)
            {
                return;
            }

            Cache = list;
        }

        private static List<TValue> Steal(ref List<TValue> source)
        {
            var taken = source;
            source = null;
            return taken;
        }

        private static List<TValue> Steal(ref List<TValue> source, int minCapacity)
        {
            var taken = source;
            if(taken == null || taken.Capacity < minCapacity)
            {
                return null;
            }

            source = null;
            return taken;
        }
    }
}
