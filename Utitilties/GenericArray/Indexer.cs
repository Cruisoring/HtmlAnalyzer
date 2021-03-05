using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.GenericArray
{
    public class Indexer<T> where T : IComparable<T>
    {
        #region IndexOf()
        public static int IndexOf(T[] context, T unique)
        {
            return Array.IndexOf(context, unique);
        }

        public static int IndexOf(T[] context, T unique, int startIndex)
        {
            return Array.IndexOf(context, unique, startIndex);
        }

        public static int IndexOf(T[] context, T[] pattern, int startIndex)
        {
            int patternLength = pattern.Length;
            int contextLength = context.Length;

            if (patternLength > contextLength)
                return -1;

            for (int i = startIndex; i < contextLength - patternLength + 1; i++)
            {
                int j = 0;
                for (int k = i; j < patternLength; j++, k++)
                {
                    if (pattern[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == patternLength)
                    return i;
            }

            return -1;
        }

        public static int IndexOf(T[] context, T[] pattern, Scope range)
        {
            if (range.First < 0 || range.Last >= context.Length)
                return -1;

            int patternLength = pattern.Length;
            int contextLength = context.Length;

            if (patternLength > contextLength || patternLength > range.Length)
                return -1;

            for (int i = range.First; i < range.Last - patternLength + 1; i++)
            {
                int j = 0;
                for (int k = i; j < patternLength; j++, k++)
                {
                    if (pattern[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == patternLength)
                    return i;
            }

            return -1;
        }

        public static int IndexOf(T[] context, T[] pattern)
        {
            return IndexOf(context, pattern, 0);
        }
        #endregion

        #region LastIndexOf()
        public static int LastIndexOf(T[] context, T unique)
        {
            return Array.LastIndexOf(context, unique);
            //for (int i = context.Length - 1; i >= 0 ; i++)
            //{
            //    if (unique.CompareTo(context[i]) == 0)
            //        return i;
            //}
            //return -1;
        }

        public static int LastIndexOf(T[] context, T unique, int startIndex)
        {
            return Array.LastIndexOf(context, unique, startIndex);

            //for (int i = Math.Min(context.Length-1, startIndex); i >= 0; i++)
            //{
            //    if (unique.CompareTo(context[i]) == 0)
            //        return i;
            //}
            //return -1;
        }

        public static int LastIndexOf(T[] context, T[] pattern)
        {
            return LastIndexOf(context, pattern, context.Length - 1);
        }

        public static int LastIndexOf(T[] context, T[] pattern, int startIndex)
        {
            if (startIndex > context.Length || startIndex < 0)
                return -1;

            if (pattern.Length == 1)
                return Array.LastIndexOf(context, pattern[0], startIndex);

            for (int i = startIndex; i >= 0; i--)
            {
                for (int j = pattern.Length - 1; j >= 0; j--)
                {
                    if (context[i].CompareTo(pattern[j]) != 0)
                    {
                        break;
                    }
                    if (j == 0)
                        return i;
                }
            }
            return -1;

            //int patternLength = pattern.Length;

            //if (patternLength > context.Length)
            //    return -1;

            //List<int> indexOfLast = AllIndexOf(context, pattern[patternLength - 1]);

            //if (indexOfLast.Count == 0)
            //    return -1;

            //int toBeNeglected = indexOfLast.FindLastIndex(x => x < patternLength - 1);

            //if (toBeNeglected != -1)
            //{
            //    indexOfLast.RemoveRange(0, toBeNeglected);
            //    if (indexOfLast.Count == 0)
            //        return -1;
            //}

            //toBeNeglected = indexOfLast.FindIndex(x => x > startIndex);

            //if (toBeNeglected != -1)
            //{
            //    indexOfLast.RemoveRange(toBeNeglected, indexOfLast.Count-toBeNeglected);
            //    if (indexOfLast.Count == 0)
            //        return -1;
            //}

            //for (int i = 0; i < indexOfLast.Count; i++)
            //{
            //    int j = 0, k = indexOfLast[i] - patternLength;
            //    for (; j < patternLength - 1; j++, k++)
            //    {
            //        if (pattern[j].CompareTo(context[k]) != 0)
            //            break;
            //    }

            //    if (j == patternLength)
            //        return i;
            //}

            //return -1;
        }

        public static int LastIndexOf(T[] context, T[] pattern, Scope range)
        {
            int patternLength = pattern.Length;

            if (range.First < 0 || range.Last >= context.Length || patternLength > range.Length)
                return -1;

            List<int> indexOfLast = AllIndexOf(context, pattern[patternLength - 1], range);

            if (indexOfLast.Count == 0)
                return -1;

            int toBeNeglected = indexOfLast.FindLastIndex(x => x + 1 - patternLength < range.First);

            if (toBeNeglected != -1)
            {
                indexOfLast.RemoveRange(0, toBeNeglected);
                if (indexOfLast.Count == 0)
                    return -1;
            }

            toBeNeglected = indexOfLast.FindIndex(x => x > range.First);

            if (toBeNeglected != -1)
            {
                indexOfLast.RemoveRange(toBeNeglected, indexOfLast.Count - toBeNeglected);
                if (indexOfLast.Count == 0)
                    return -1;
            }

            for (int i = 0; i < indexOfLast.Count; i++)
            {
                int j = 0, k = indexOfLast[i] - patternLength;
                for (; j < patternLength - 1; j++, k++)
                {
                    if (pattern[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == patternLength)
                    return i;
            }

            return -1;
        }
        #endregion

        #region AllIndexOf()
        public static List<int> AllIndexOf(T[] context, T unique)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < context.Length; i++)
            {
                if (unique.CompareTo(context[i]) == 0)
                    result.Add(i);
            }

            return result;
        }

        public static List<int> AllIndexOf(T[] context, T unique, int startIndex)
        {
            List<int> result = new List<int>();

            for (int i = Math.Max(0, startIndex); i < context.Length; i++)
            {
                if (unique.CompareTo(context[i]) == 0)
                    result.Add(i);
            }

            return result;
        }

        public static List<int> AllIndexOf(T[] context, T unique, Scope range)
        {
            List<int> result = new List<int>();

            if (range.Length > context.Length)
                return result;

            for (int i = range.First; i < range.Last; i++)
            {
                if (unique.CompareTo(context[i]) == 0)
                    result.Add(i);
            }

            return result;
        }

        public static List<int> AllIndexOf(T[] context, T[] pattern, int startIndex)
        {
            int patternLength = pattern.Length;
            List<int> result = new List<int>();

            if (patternLength > context.Length)
                return result;

            List<int> indexesOfFirst = AllIndexOf(context, pattern[0], startIndex);
            indexesOfFirst.RemoveAll(x => x > context.Length - patternLength);

            //int spaceNeeded = context.Length-patternLength;

            //int toBeNeglected = indexesOfFirst.FindLastIndex(x => x > spaceNeeded);

            //if (toBeNeglected != -1)
            //{
            //    indexesOfFirst.RemoveRange(toBeNeglected, indexesOfFirst.Count - toBeNeglected);
            //}

            for (int i = 0; i < indexesOfFirst.Count; i++)
            {
                int j = 0, k = indexesOfFirst[i];
                for (; j < patternLength; j++, k++)
                {
                    if (pattern[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == patternLength)
                    result.Add(indexesOfFirst[i]);
            }

            return result;
        }

        public static List<int> AllIndexOf(T[] context, T[] pattern, Scope range)
        {
            int patternLength = pattern.Length;
            List<int> result = new List<int>();

            if (range.First < 0 || range.Last >= context.Length || patternLength > range.Length)
                return result;

            List<int> indexesOfFirst = AllIndexOf(context, pattern[0], range);

            for (int i = 0; i < indexesOfFirst.Count; i++)
            {
                int j = 1, k = indexesOfFirst[i] + 1;
                for (; j < patternLength; j++, k++)
                {
                    if (pattern[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == patternLength)
                    result.Add(indexesOfFirst[i]);
            }

            return result;
        }

        public static List<int> AllIndexOf(T[] context, T[] pattern)
        {
            return AllIndexOf(context, pattern, 0);
        }
        #endregion
    }
}
