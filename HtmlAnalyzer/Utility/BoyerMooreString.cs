using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlAnalyzer.Utility
{
    /// <summary>
    /// Implements a multi-stage byte array. Uses less memory than a byte
    /// array large enough to hold an offset for each Unicode character.
    /// </summary>
    /// <see cref="http://www.blackbeltcoder.com/Articles/algorithms/fast-text-search-with-boyer-moore"/>
    class UnicodeSkipArray
    {
        // Pattern length used for default byte value
        private byte _patternLength;
        // Default byte array (filled with default value)
        private byte[] _default;
        // Array to hold byte arrays
        private byte[][] _skipTable;
        // Size of each block
        private const int BlockSize = 0x100;

        /// <summary>
        /// Initializes this UnicodeSkipTable instance
        /// </summary>
        /// <param name="patternLength">Length of BM pattern</param>
        public UnicodeSkipArray(int patternLength)
        {
            // Default value (length of pattern being searched)
            _patternLength = (byte)patternLength;
            // Default table (filled with default value)
            _default = new byte[BlockSize];
            InitializeBlock(_default);
            // Master table (array of arrays)
            _skipTable = new byte[BlockSize][];
            for (int i = 0; i < BlockSize; i++)
                _skipTable[i] = _default;
        }

        /// <summary>
        /// Sets/gets a value in the multi-stage tables.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                // Return value
                return _skipTable[index / BlockSize][index % BlockSize];
            }
            set
            {
                // Get array that contains value to set
                int i = (index / BlockSize);
                // Does it reference the default table?
                if (_skipTable[i] == _default)
                {
                    // Yes, value goes in a new table
                    _skipTable[i] = new byte[BlockSize];
                    InitializeBlock(_skipTable[i]);
                }
                // Set value
                _skipTable[i][index % BlockSize] = value;
            }
        }

        /// <summary>
        /// Initializes a block to hold the currentTag "nomatch" value.
        /// </summary>
        /// <param name="block">Block to be initialized</param>
        private void InitializeBlock(byte[] block)
        {
            for (int i = 0; i < BlockSize; i++)
                block[i] = _patternLength;
        }
    }

    /// <summary>
    /// Implements Boyer-Moore search algorithm
    /// </summary>
    public class BoyerMooreString
    {
        public string Pattern { get; private set; }
        public int Length { get { return Pattern.Length; } }
        public bool IgnoreCase { get; private set; }
        private UnicodeSkipArray _skipArray;

        // Returned index when no match found
        public const int InvalidIndex = -1;

        public BoyerMooreString(string pattern)
        {
            Initialize(pattern, false);
        }

        public BoyerMooreString(string pattern, bool ignoreCase)
        {
            Initialize(pattern, ignoreCase);
        }

        /// <summary>
        /// Initializes this instance to search a new pattern.
        /// </summary>
        /// <param name="pattern">Pattern to search for</param>
        public void Initialize(string pattern)
        {
            Initialize(pattern, false);
        }

        /// <summary>
        /// Initializes this instance to search a new pattern.
        /// </summary>
        /// <param name="pattern">Pattern to search for</param>
        /// <param name="ignoreCase">If true, search is case-insensitive</param>
        public void Initialize(string pattern, bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            Pattern = IgnoreCase ? pattern.ToLower() : pattern;

            // Create multi-stage skip table
            _skipArray = new UnicodeSkipArray(Pattern.Length);

            for (int i = 0; i < Pattern.Length - 1; i++)
                _skipArray[Pattern[i]] = (byte)(Pattern.Length - i - 1);

            //// Initialize skip table for this pattern
            //if (IgnoreCase)
            //{
            //    for (int i = 0; i < Pattern.Length - 1; i++)
            //    {
            //        _skipArray[Char.ToLower(Pattern[i])] = (byte)(Pattern.Length - i - 1);
            //        _skipArray[Char.ToUpper(Pattern[i])] = (byte)(Pattern.Length - i - 1);
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < Pattern.Length - 1; i++)
            //        _skipArray[Pattern[i]] = (byte)(Pattern.Length - i - 1);
            //}
        }

        /// <summary>
        /// Searches for the currentTag pattern within the given text
        /// starting at the beginning.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Search(string text)
        {
            return Search(text, 0);
        }

        /// <summary>
        /// Searches for the currentTag pattern within the given text
        /// starting at the specified index.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="startIndex">Offset to begin search</param>
        /// <returns></returns>
        public int Search(string text, int startIndex)
        {
            int i = startIndex;

            string context = IgnoreCase ? text.ToLower() : text;

            // Loop while there's still room for search term
            while (i <= (context.Length - Pattern.Length))
            {
                // Look if we have a match at this position
                int j = Pattern.Length - 1;
                while (j >= 0 && Pattern[j] == context[i + j])
                    j--;

                if (j < 0)
                {
                    // Match found
                    return i;
                }

                // Advance to next comparision
                i += Math.Max(_skipArray[context[i + j]] - Pattern.Length + 1 + j, 1);
            }

            //// Loop while there's still room for search term
            //while (i <= (text.Length - Pattern.Length))
            //{
            //    // Look if we have a match at this position
            //    int j = Pattern.Length - 1;
            //    if (IgnoreCase)
            //    {
            //        while (j >= 0 && Char.ToUpper(Pattern[j]) == Char.ToUpper(text[i + j]))
            //            j--;
            //    }
            //    else
            //    {
            //        while (j >= 0 && Pattern[j] == text[i + j])
            //            j--;
            //    }

            //    if (j < 0)
            //    {
            //        // Match found
            //        return i;
            //    }

            //    // Advance to next comparision
            //    i += Math.Max(_skipArray[text[i + j]] - Pattern.Length + 1 + j, 1);
            //}
            // No match found
            return InvalidIndex;
        }

        public static string FirstTextBetween(string text, BoyerMooreString leading, char endChar)
        {
            int start = leading.Search(text);

            if (start == -1)
                return null;

            start += leading.Length;
            int end = text.IndexOf(endChar, start);

            if (end == -1)
                return null;

            return text.Substring(start, end - start);
        }

        public static string FirstTextBetween(string text, char leadChar, char endChar)
        {
            int start = text.IndexOf(leadChar);

            if (start == -1)
                return null;

            start ++;
            int end = text.IndexOf(endChar, start);

            if (end == -1)
                return null;

            return text.Substring(start, end - start);
        }
    }
}
