using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.GenericArray
{
    public static class SmartFinder
    {
        public static int LeastMatchedLength = 2;

        #region Functions for String Indexer
        public static string BestMatchOf(string word, string pattern)
        {
            int index = IndexOf(word, pattern);

            if (index != -1)
                return pattern;
            else if (pattern.Length == 1)
                return "";

            for (int len = pattern.Length - 1; len >= LeastMatchedLength; len--)
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

        public static int IndexOf(string context, string pattern)
        {
            return Indexer<char>.IndexOf(context.ToCharArray(), pattern.ToCharArray());
        }

        #region ScopeOfPattern()
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
                foreach (int i in matches)
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
            closingIndexes.RemoveAll(x => x <= openingIndexes[0] + leadingPettern.Length);

            for (int i = 0; i < openingIndexes.Count; i++)
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
        #endregion

        #region StringOfPattern()
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
        #endregion

        #region InsideOfPattern()
        public static List<string> InsideOfPattern(string context, string leadingPettern, string endingPettern, int startIndex)
        {
            List<String> result = new List<String>();
            List<Scope> ranges = ScopesOfPattern(context, leadingPettern, endingPettern, startIndex);

            int leadinglen = leadingPettern.Length;
            int endingLen = endingPettern.Length;
            foreach (Scope range in ranges)
            {
                result.Add(context.Substring(range.First + leadinglen, range.Length - leadinglen - endingLen));
            }
            return result;
        }
        public static List<string> InsideOfPattern(string context, string leadingPettern, string endingPettern)
        {
            return InsideOfPattern(context, leadingPettern, endingPettern, 0);
        }
        #endregion

        #region AllIndexOf()
        public static List<int> AllIndexOf(String context, char unique)
        {
            return Indexer<Char>.AllIndexOf(context.ToCharArray(), unique);
        }
        public static List<int> AllIndexOf(String context, char unique, int startIndex)
        {
            return Indexer<Char>.AllIndexOf(context.ToCharArray(), unique, startIndex);
        }
        public static List<int> AllIndexOf(String context, char unique, bool ignoreCase)
        {
            if (ignoreCase)
                return Indexer<Char>.AllIndexOf(context.ToLower().ToCharArray(), Char.ToLower(unique));
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
        #endregion

        #region LastIndexOf()
        public static int LastIndexOf(String context, char unique)
        {
            return Indexer<char>.LastIndexOf(context.ToCharArray(), unique);
        }

        public static int LastIndexOf(String context, string pattern)
        {
            if (pattern.Length == 1)
                return LastIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return -1;
            else
                return Indexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray());
        }
        public static int LastIndexOf(String context, string pattern, int startIndex)
        {
            if (pattern.Length == 1)
                return LastIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return -1;
            else
                return Indexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray(), startIndex);
        }
        public static int LastIndexOf(String context, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
                return Indexer<Char>.LastIndexOf(context.ToLower().ToCharArray(), pattern.ToLower().ToCharArray());
            else
                return Indexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray());
        }
        public static int LastIndexOf(String context, string pattern, int startIndex, bool ignoreCase)
        {
            if (ignoreCase)
                return Indexer<Char>.LastIndexOf(context.ToLower().ToCharArray(), pattern.ToLower().ToCharArray(), startIndex);
            else
                return Indexer<Char>.LastIndexOf(context.ToCharArray(), pattern.ToCharArray(), startIndex);
        }
        #endregion

        #endregion

        #region Functions for chars Indexer

        public static List<int> AllIndexOf(char[] context, char unique)
        {
            return Indexer<Char>.AllIndexOf(context, unique);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern)
        {
            if (pattern.Length == 1)
                return AllIndexOf(context, pattern[0]);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return Indexer<Char>.AllIndexOf(context, pattern);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern, int startIndex)
        {
            if (pattern.Length == 1)
                return Indexer<Char>.AllIndexOf(context, pattern[0], startIndex);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return Indexer<Char>.AllIndexOf(context, pattern, startIndex);
        }

        public static List<int> AllIndexOf(char[] context, char[] pattern, Scope range)
        {
            if (pattern.Length == 1)
                return Indexer<Char>.AllIndexOf(context, pattern[0], range);
            else if (pattern == null || pattern.Length == 0)
                return new List<int>();
            else
                return Indexer<Char>.AllIndexOf(context, pattern, range);
        }


        public static int LastIndexOf(char[] context, char[] pattern)
        {
            return Indexer<Char>.LastIndexOf(context, pattern);
        }

        public static int LastIndexOf(char[] context, char[] pattern, int startIndex)
        {
            return Indexer<Char>.LastIndexOf(context, pattern, startIndex);
        }

        public static int LastIndexOf(char[] context, char[] pattern, Scope range)
        {
            return Indexer<Char>.LastIndexOf(context, pattern, range);
        }

        #endregion

        #region Functions for Bytes Indexer

        public static List<int> AllIndexOf(Byte[] context, Byte unique)
        {
            return Indexer<Byte>.AllIndexOf(context, unique);
        }
        public static List<int> AllIndexOf(Byte[] context, Byte[] pattern)
        {
            return Indexer<Byte>.AllIndexOf(context, pattern);
        }
        public static int LastIndexOf(Byte[] context, Byte[] pattern)
        {
            return Indexer<Byte>.LastIndexOf(context, pattern);
        }

        #endregion
    }

}
