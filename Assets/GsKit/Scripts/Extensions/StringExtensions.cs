using System;
using System.Collections.Generic;

namespace GsKit.Extensions
{
    public static class StringExtensions
    {
        public static IList<int> AllIndexesOf(this string str, string val, int startIndex, int count, StringComparison comparisonType)
        {
            int index = startIndex;
            int realCount = count;
            List<int> indexes = new List<int>();
            int valueLength = val.Length;
            while ((index != -1) && (count > 0))
            {
                Console.WriteLine(realCount);
                index = str.IndexOf(val, index, realCount, comparisonType);
                indexes.Add(index);
                realCount = count - (index - startIndex + valueLength);
                index = index == str.Length - 1 || index == -1 ? -1 : index + valueLength;
            }

            indexes.RemoveAt(indexes.Count - 1);
            return indexes;
        }

        public static IList<int> AllIndexesOf(this string str, string val, int startIndex, StringComparison comparisonType)
        {
            return AllIndexesOf(str, val, startIndex, str.Length - startIndex, comparisonType);
        }

        public static IList<int> AllIndexesOf(this string str, string val, StringComparison comparisonType)
        {
            return AllIndexesOf(str, val, 0, str.Length, comparisonType);
        }
    }
}