using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAnalyzer.MarkupLanguageHelper;

namespace HtmlAnalyzer.GenericPatternIndexer
{
    public class GenericIndexer<T> where T : struct, IComparable<T>
    {
        public static int IndexOf(T[] context, T unique)
        {
            return Array.IndexOf(context, unique);
            //for (int i = 0; i < context.Length; i++)
            //{
            //    //if (unique == word[i])
            //    if (unique .CompareTo(context[i]) == 0) 
            //        return i;
            //}
            //return -1;
        }

        public static int IndexOf(T[] context, T unique, int startIndex)
        {
            return Array.IndexOf(context, unique, startIndex);
            //for (int i = Math.Max(0, startIndex); i < context.Length; i++)
            //{
            //    if (unique.CompareTo(context[i]) == 0)
            //        return i;
            //}
            //return -1;
        }

        public static int IndexOf(T[] context, T[] pattern, int startIndex)
        {
            int patternLength = pattern.Length;
            int contextLength = context.Length;

            if (patternLength > contextLength)
                return -1;

            for (int i = startIndex; i < contextLength-patternLength+1; i ++)
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

        public static List<int> AllIndexOf(T[] context, T unique)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < context.Length; i ++ )
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
                for (; j < patternLength; j ++, k++ )
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

            for (int i = startIndex; i >= 0; i -- )
            {
                for(int j = pattern.Length - 1; j >= 0; j--)
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

        //public static int LastIndexOf(T[] word, T[] pattern, int startIndex)
        //{
        //    int patternLength = pattern.Length;
        //    int contextLength = word.Length;

        //    if (patternLength > contextLength - startIndex)
        //        return -1;

        //    int start = Math.Max(startIndex, contextLength - patternLength + 1);

        //    for (int i = start; i < contextLength - patternLength + 1; i--)
        //    {
        //        int j = 0;
        //        for (k = i; j < patternLength; j++, k++)
        //        {
        //            if (pattern[j].CompareTo(word[k]) != 0)
        //                break;
        //        }

        //        if (j == patternLength)
        //            return i;
        //    }

        //    return -1;
        //}

        //public static int LastIndexOf(T[] word, T[] pattern)
        //{
        //    return IndexOf(word, pattern, 0);
        //}

    }

    public static class Finder
    {
        #region Functions for String Indexer
        public static int LeastMatchedLength = 2;

        public static string BestMatchOf(string word, string pattern)
        {
            int index = IndexOf(word, pattern);

            if (index != -1)
                return pattern;
            else if (pattern.Length == 1)
                return "";

            for(int len = pattern.Length-1; len >= LeastMatchedLength; len--)
            {
                List<string> subs = new List<string>();

                for (int i = 0; i <= pattern.Length - len; i++)
                {
                    string subPattern = pattern.Substring(i, len);

                    if (!subs.Contains(subPattern))
                        subs.Add(subPattern);
                }

                foreach (string s in subs)
                {
                    index = IndexOf(word, s);
                    if (index != -1)
                        return s;
                }
            }
            return "";
        }

        public static List<Scope> ScopesOfPattern(string text, string opening, string closing, int startIndex)
        {
            return ScopesOfPattern(text.ToCharArray(), opening.ToCharArray(), closing.ToCharArray(), startIndex);
        }

        public static List<Scope> ScopesOfPattern(char[] context, string opening, string closing, int startIndex)
        {
            return ScopesOfPattern(context, opening.ToCharArray(), closing.ToCharArray(), startIndex);
        }

        public static List<Scope> ScopesOfPattern(char[] context, string opening, string closing)
        {
            return ScopesOfPattern(context, opening.ToCharArray(), closing.ToCharArray(), 0);
        }

        public static List<Scope> ScopesOfPattern(string context, string leadingPettern, string endingPettern)
        {
            return ScopesOfPattern(context, leadingPettern, endingPettern, 0);
        }

        public static List<Scope> ScopesOfPattern(char[] context, char[] leadingPettern, char[] endingPettern, int startIndex)
        {
            List<Scope> result = new List<Scope>();
            if (leadingPettern == null || leadingPettern.Length == 0 || endingPettern == null || endingPettern.Length == 0)
            {
                return result;
            }
            else if (leadingPettern == endingPettern)
            {
                List<int> matches = AllIndexOf(context, leadingPettern, startIndex);
                foreach(int i in matches)
                {
                    result.Add(new Scope(i, i + leadingPettern.Length));
                }
                return result;
            }

            int endingPetternLength = endingPettern.Length;

            List<int> openingIndexes = AllIndexOf(context, leadingPettern, startIndex);

            if (openingIndexes.Count == 0)
                return result;

            List<int> closingIndexes = AllIndexOf(context, endingPettern, openingIndexes[0]);

            if (closingIndexes.Count == 0)
                return result;
            else if (closingIndexes.Count == 1 || openingIndexes.Count == 1)
            {
                int len = closingIndexes[0] + endingPetternLength - openingIndexes[0];
                result.Add(new Scope(openingIndexes[0], len));
                return result;
            }

            Stack<int> stack = new Stack<int>();

            int lastEnding, leading, ending, nextLeading, nextEndingIndex = 0;
            for (int i = 0, j = 0; i < openingIndexes.Count; i++)
            {
                nextLeading = openingIndexes[i];
                lastEnding = closingIndexes[j];

                if (stack.Count == 0 || lastEnding > nextLeading)
                {
                    stack.Push(nextLeading);
                    continue;
                }

                //nextLeading shall always be greater than lastEnding
                if (lastEnding == nextLeading)
                    throw new Exception();

                do
                {
                    leading = stack.Pop();

                    ending = closingIndexes[nextEndingIndex++];

                    if (leading < ending)
                    {
                        Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                        result.Add(newMatch);
                    }
                    else
                        throw new Exception();
                }while(stack.Count != 0 && nextEndingIndex <= j);

                j++;
                stack.Push(nextLeading);
            }

            if (stack.Count() == 0 || nextEndingIndex == closingIndexes.Count)
                return result;
            else if (stack.Count() == 1 && nextEndingIndex == closingIndexes.Count-1)
            {
                leading = stack.Pop();

                ending = closingIndexes[nextEndingIndex++];

                if (leading < ending)
                {
                    Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                    result.Add(newMatch);
                    return result;
                }
                else
                    throw new Exception();
            }
            else
            {
                do 
                {
                    leading = stack.Pop();
                    ending = closingIndexes.Find(x => x > leading + leadingPettern.Length);
                    Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                    result.Add(newMatch);
                } while (stack.Count != 0);

                return result;
            }
            //Only nested pairs are expected
            //Stack<int> undertermined = new Stack<int>();
            //Stack<int> unpairedEndings = new Stack<int>();

            //foreach(int i in closingIndexes)
            //{
            //    unpairedEndings.Push(i);
            //}

            //undertermined.Push(openingIndexes[0]);

            //int lastEnding, leading, nextLeading, peek = unpairedEndings.Peek();
            //for (int i = 1; i < openingIndexes.Count; i ++)
            //{
            //    nextLeading = openingIndexes[i];
                
            //    if (unpairedEndings.Count == 0)
            //        break;

            //    peek = unpairedEndings.Peek();

            //    if (peek == nextLeading)
            //        throw new Exception();
            //    else if (peek < nextLeading)
            //    {
            //        while(undertermined.Count != 0)
            //        {
            //            leading = undertermined.Pop();
            //            lastEnding = unpairedEndings.Pop();
            //            Scope newMatch = new Scope(leading, lastEnding-leading+endingPetternLength);
            //            result.Add(newMatch);

            //            peek = unpairedEndings.Peek();
            //            if (peek >= nextLeading)
            //                break;
            //            else if (peek > undertermined.Peek())
            //                continue;
            //        };

            //        if (peek < nextLeading)
            //        {
            //            do 
            //            {
            //                unpairedEndings.Pop();
            //                peek = unpairedEndings.Peek();
            //            } while (peek <= nextLeading);
            //        }
                        
            //        undertermined.Push(nextLeading);
            //    }
            //    else
            //    {
            //        leading = undertermined.Pop();
            //        lastEnding = unpairedEndings.Pop();
            //        Scope newMatch = new Scope(leading, lastEnding - nextLeading + endingPetternLength);
            //        result.Add(newMatch);
            //    }
            //}

            //if (unpairedEndings.Count == 0 || undertermined.Count == 0)
            //    return result;
            //else if (undertermined.Count != 0)
            //{
            //    leading = undertermined.Pop();
            //    lastEnding = unpairedEndings.Pop();
            //    Scope newMatch = new Scope(leading, lastEnding - leading + endingPetternLength);
            //    result.Add(newMatch);
            //    return result;
            //}
            //else
            //    throw new Exception();
        }

        public static List<Scope> ScopesOfPattern(char[] context, char[] leadingPettern, char[] endingPettern, Scope range)
        {
            List<Scope> result = new List<Scope>();

            #region Check to see if it is out of range
            if (leadingPettern == null || leadingPettern.Length == 0 || endingPettern == null || endingPettern.Length == 0)
            {
                return result;
            }
            else if (leadingPettern == endingPettern)
            {
                List<int> matches = AllIndexOf(context, leadingPettern, range);
                foreach (int i in matches)
                {
                    if (i + leadingPettern.Length > range.Last)
                        break;

                    result.Add(new Scope(i, i + leadingPettern.Length));
                }
                return result;
            }
            #endregion

            int endingPetternLength = endingPettern.Length;

            List<int> openingIndexes = AllIndexOf(context, leadingPettern, range);

            if (openingIndexes.Count == 0)
                return result;

            List<int> closingIndexes = AllIndexOf(context, endingPettern, range);
            closingIndexes.RemoveAll(x => x <= openingIndexes[0]+leadingPettern.Length);

            for (int i=0; i < openingIndexes.Count; i ++)
            {
                int start = openingIndexes[i] + leadingPettern.Length;
                int last = (i == openingIndexes.Count - 1) ? range.Last : openingIndexes[i + 1] - 1;

                int endingIndex = closingIndexes.FindIndex(x => x >= start && x <= last);

                if (endingIndex == -1)
                    throw new Exception();

                result.Add(new Scope(openingIndexes[i], closingIndexes[endingIndex] - openingIndexes[i] + 1));
            }

            return result;

            #region Old Approach, not so efficient
            /*/ 

            if (closingIndexes.Count == 0)
                return result;
            else if (closingIndexes.Count == 1 || openingIndexes.Count == 1)
            {
                int len = closingIndexes[0] + endingPetternLength - openingIndexes[0];
                result.Add(new Scope(openingIndexes[0], len));
                return result;
            }

            Stack<int> stack = new Stack<int>();

            int lastEnding, leading, ending, nextLeading, nextEndingIndex = 0;
            for (int i = 0, j = 0; i < openingIndexes.Count; i++)
            {
                nextLeading = openingIndexes[i];
                lastEnding = closingIndexes[j];

                if (stack.Count == 0 || lastEnding > nextLeading)
                {
                    stack.Push(nextLeading);
                    continue;
                }

                //nextLeading shall always be greater than lastEnding
                if (lastEnding == nextLeading)
                    throw new Exception();

                do
                {
                    leading = stack.Pop();

                    ending = closingIndexes[nextEndingIndex++];

                    if (leading < ending)
                    {
                        Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                        result.Add(newMatch);
                    }
                    else
                        throw new Exception();
                } while (stack.Count != 0 && nextEndingIndex <= j);

                j++;
                stack.Push(nextLeading);
            }

            if (stack.Count() == 0 || nextEndingIndex == closingIndexes.Count)
                return result;
            else if (stack.Count() == 1 && nextEndingIndex == closingIndexes.Count - 1)
            {
                leading = stack.Pop();

                ending = closingIndexes[nextEndingIndex++];

                if (leading < ending)
                {
                    Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                    result.Add(newMatch);
                    return result;
                }
                else
                    throw new Exception();
            }
            else
            {
                do
                {
                    leading = stack.Pop();
                    ending = closingIndexes.Find(x => x > leading + leadingPettern.Length);
                    Scope newMatch = new Scope(leading, ending - leading + endingPetternLength);
                    result.Add(newMatch);
                } while (stack.Count != 0);

                return result;
            }
            //*/
            #endregion
        }

        public static List<string> StringsOfPettern(char[] context, char[] leadingPettern, char[] endingPettern, int startIndex)
        {
            List<String> result = new List<String>();
            List<Scope> ranges = ScopesOfPattern(context, leadingPettern, endingPettern, startIndex);

            foreach (Scope range in ranges)
            {
                result.Add(new String(context, range.First, range.Length));
            }
            return result;
        }
        public static List<string> StringsOfPettern(char[] context, string leadingPettern, string endingPettern)
        {
            return StringsOfPettern(context, leadingPettern.ToCharArray(), endingPettern.ToCharArray(), 0);
        }
        public static List<string> StringsOfPettern(char[] context, string leadingPettern, string endingPettern, int startIndex)
        {
            return StringsOfPettern(context, leadingPettern.ToCharArray(), endingPettern.ToCharArray(), startIndex);
        }
        public static List<string> StringsOfPettern(char[] context, string leadingPettern, string endingPettern, Scope range)
        {
            List<String> result = new List<String>();
            List<Scope> ranges = ScopesOfPattern(context, leadingPettern.ToCharArray(), endingPettern.ToCharArray(), range);

            foreach (Scope rg in ranges)
            {
                result.Add(new string(context, rg.First, rg.Length));
            }
            return result;
        }
        public static List<string> StringsOfPettern(string context, string leadingPettern, string endingPettern, int startIndex)
        {
            List<String> result = new List<String>();
            List<Scope> ranges = ScopesOfPattern(context, leadingPettern, endingPettern, startIndex);

            foreach (Scope range in ranges)
            {
                result.Add(context.Substring(range.First, range.Length));
            }
            return result;
        }
        public static List<string> StringsOfPettern(string context, string leadingPettern, string endingPettern)
        {
            return StringsOfPettern(context, leadingPettern, endingPettern, 0);
        }

        public static List<string> InsideOfPattern(string context, string leadingPettern, string endingPettern, int startIndex)
        {
            List<String> result = new List<String>();
            List<Scope> ranges = ScopesOfPattern(context, leadingPettern, endingPettern, startIndex);

            int leadinglen = leadingPettern.Length;
            int endingLen = endingPettern.Length;
            foreach (Scope range in ranges)
            {
                result.Add(context.Substring(range.First + leadinglen, range.Length-leadinglen-endingLen));
            }
            return result;
        }
        public static List<string> InsideOfPattern(string context, string leadingPettern, string endingPettern)
        {
            return InsideOfPattern(context, leadingPettern, endingPettern, 0);
        }

        public static List<int> AllIndexOf(String context, char unique)
        {
            return GenericIndexer<Char>.AllIndexOf(context.ToCharArray(), unique);
        }
        public static List<int> AllIndexOf(String context, char unique, int startIndex)
        {
            return GenericIndexer<Char>.AllIndexOf(context.ToCharArray(), unique, startIndex);
        }
        public static List<int> AllIndexOf(String context, char unique, bool ignoreCase)
        {            
            if (ignoreCase)
                return GenericIndexer<Char>.AllIndexOf(context.ToLower().ToCharArray(), Char.ToLower(unique));
            else
                return AllIndexOf(context, unique);
        }
        public static List<int> AllIndexOf(String context, char unique, int startIndex, bool ignoreCase)
        {
            if (ignoreCase)
                return AllIndexOf(context.ToLower(), Char.ToLower(unique), startIndex);
            else
                return AllIndexOf(context, unique, startIndex);
        }

        public static List<int> AllIndexOf(String context, string pattern)
        {
            return AllIndexOf(context.ToCharArray(), pattern.ToCharArray());
        }
        public static List<int> AllIndexOf(String context, string pattern, int startIndex)
        {
            return AllIndexOf(context.ToCharArray(), pattern.ToCharArray(), startIndex);
        }
        public static List<int> AllIndexOf(String context, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
                return AllIndexOf(context.ToLower(), pattern.ToLower());
            else
                return AllIndexOf(context, pattern);
        }
        public static List<int> AllIndexOf(String context, string pattern, int startIndex, bool ignoreCase)
        {
            if (ignoreCase)
                return AllIndexOf(context.ToLower(), pattern.ToLower(), startIndex);
            else
                return AllIndexOf(context, pattern, startIndex);
        }

        public static int IndexOf(string context, string pattern)
        {
            return GenericIndexer<char>.IndexOf(context.ToCharArray(), pattern.ToCharArray());
        }

        public static int LastIndexOf(String context, char unique)
        {
            return GenericIndexer<char>.LastIndexOf(context.ToCharArray(), unique);
        }

        public static int LastIndexOf(String context, string pattern)
        {
            if (pattern.Length == 1)
                return LastIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return -1;
            else
                return GenericIndexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray());
        }
        public static int LastIndexOf(String context, string pattern, int startIndex)
        {
            if (pattern.Length == 1)
                return LastIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return -1;
            else
                return GenericIndexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray(), startIndex);
        }
        public static int LastIndexOf(String context, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
                return GenericIndexer<Char>.LastIndexOf(context.ToLower().ToCharArray(), pattern.ToLower().ToCharArray());
            else
                return GenericIndexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray());
        }
        public static int LastIndexOf(String context, string pattern, int startIndex, bool ignoreCase)
        {
            if (ignoreCase)
                return GenericIndexer<Char>.LastIndexOf(context.ToLower().ToCharArray(), pattern.ToLower().ToCharArray(), startIndex);
            else
                return GenericIndexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray(), startIndex);
        }

        #endregion

        #region Functions for chars Indexer

        public static List<int> AllIndexOf(char[] context, char unique)
        {
            return GenericIndexer<Char>.AllIndexOf(context, unique);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern)
        {
            if (pattern.Length == 1)
                return AllIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return GenericIndexer<Char>.AllIndexOf(context, pattern);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern, int startIndex)
        {
            if (pattern.Length == 1)
                return GenericIndexer<Char>.AllIndexOf(context, pattern[0], startIndex);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return GenericIndexer<Char>.AllIndexOf(context, pattern, startIndex);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern, Scope range)
        {
            if (pattern.Length == 1)
                return GenericIndexer<Char>.AllIndexOf(context, pattern[0], range);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return GenericIndexer<Char>.AllIndexOf(context, pattern, range);
        }


        public static int LastIndexOf(char[] context, char[] pattern)
        {
            return GenericIndexer<Char>.LastIndexOf(context, pattern);
        }

        public static int LastIndexOf(char[] context, char[] pattern, int startIndex)
        {
            return GenericIndexer<Char>.LastIndexOf(context, pattern, startIndex);
        }

        public static int LastIndexOf(char[] context, char[] pattern, Scope range)
        {
            return GenericIndexer<Char>.LastIndexOf(context, pattern, range);
        }

        #endregion

        #region Functions for Bytes Indexer

        public static List<int> AllIndexOf(Byte[] context, Byte unique)
        {
            return GenericIndexer<Byte>.AllIndexOf(context, unique);
        }
        public static List<int> AllIndexOf(Byte[] context, Byte[] pattern)
        {
            return GenericIndexer<Byte>.AllIndexOf(context, pattern);
        }
        public static int LastIndexOf(Byte[] context, Byte[] pattern)
        {
            return GenericIndexer<Byte>.LastIndexOf(context, pattern);
        }

        #endregion

        /*/
        #region Functions for SBytes Indexer

        public static List<int> AllIndexOf(SByte[] word, SByte unique)
        {
            return SBytesIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(SByte[] word, SByte[] pattern)
        {
            return SBytesIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(SByte[] word, SByte[] pattern)
        {
            return SBytesIndexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Single Array Indexer

        public static List<int> AllIndexOf(Single[] word, Single unique)
        {
            return SingleIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Single[] word, Single[] pattern)
        {
            return SingleIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Single[] word, Single[] pattern)
        {
            return SingleIndexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Int16 Array Indexer

        public static List<int> AllIndexOf(Int16[] word, Int16 unique)
        {
            return Int16Indexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Int16[] word, Int16[] pattern)
        {
            return Int16Indexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Int16[] word, Int16[] pattern)
        {
            return Int16Indexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Int32 Array Indexer

        public static List<int> AllIndexOf(Int32[] word, Int32 unique)
        {
            return Int32Indexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Int32[] word, Int32[] pattern)
        {
            return Int32Indexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Int32[] word, Int32[] pattern)
        {
            return Int32Indexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for UInt32 Array Indexer

        public static List<int> AllIndexOf(UInt32[] word, UInt32 unique)
        {
            return UInt32Indexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(UInt32[] word, UInt32[] pattern)
        {
            return UInt32Indexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(UInt32[] word, UInt32[] pattern)
        {
            return UInt32Indexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Int64 Array Indexer

        public static List<int> AllIndexOf(Int64[] word, Int64 unique)
        {
            return Int64Indexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Int64[] word, Int64[] pattern)
        {
            return Int64Indexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Int64[] word, Int64[] pattern)
        {
            return Int64Indexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for UInt64 Array Indexer

        public static List<int> AllIndexOf(UInt64[] word, UInt64 unique)
        {
            return UInt64Indexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(UInt64[] word, UInt64[] pattern)
        {
            return UInt64Indexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(UInt64[] word, UInt64[] pattern)
        {
            return UInt64Indexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Double Array Indexer

        public static List<int> AllIndexOf(Double[] word, Double unique)
        {
            return DoubleIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Double[] word, Double[] pattern)
        {
            return DoubleIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Double[] word, Double[] pattern)
        {
            return DoubleIndexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for Decimal Array Indexer

        public static List<int> AllIndexOf(Decimal[] word, Decimal unique)
        {
            return DecimalIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(Decimal[] word, Decimal[] pattern)
        {
            return DecimalIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(Decimal[] word, Decimal[] pattern)
        {
            return DecimalIndexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for DateTime Array Indexer

        public static List<int> AllIndexOf(DateTime[] word, DateTime unique)
        {
            return DateTimeIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(DateTime[] word, DateTime[] pattern)
        {
            return DateTimeIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(DateTime[] word, DateTime[] pattern)
        {
            return DateTimeIndexer.LastIndexOf(word, pattern);
        }

        #endregion

        #region Functions for DateTimeOffset Array Indexer

        public static List<int> AllIndexOf(DateTimeOffset[] word, DateTimeOffset unique)
        {
            return DateTimeOffsetIndexer.AllIndexOf(word, unique);
        }
        public static List<int> AllIndexOf(DateTimeOffset[] word, DateTimeOffset[] pattern)
        {
            return DateTimeOffsetIndexer.AllIndexOf(word, pattern);
        }
        public static int LastIndexOf(DateTimeOffset[] word, DateTimeOffset[] pattern)
        {
            return DateTimeOffsetIndexer.LastIndexOf(word, pattern);
        }

        #endregion
        //*/
    }
}
