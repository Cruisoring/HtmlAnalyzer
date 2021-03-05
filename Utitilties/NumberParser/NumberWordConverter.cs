using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.NumberParser
{
    /// <summary>
    /// Define the output format of the words from number
    /// </summary>
    public enum WordsFormat
    {
        CapitalOnFirst = 0,
        LowCaseOnly = 1,
        UpperCaseOnly = 2
    }


    public abstract class NumberWordConverter
    {
        #region Static region

        public static string WhiteSpace = " ";

        public static char[] splitter = new char[] { ' ', ',', '-', ',', '\'' };

        public static Dictionary<Encoding, NumberWordConverter> Converters = new Dictionary<Encoding, NumberWordConverter>();

        //static NumberWordConverter()
        //{
            //NumberWordConverter converter = new EnglishNumberWordConverter(
            //new Dictionary<int, List<string>>
            //{
            //    {0, new List<string>{"zero"}},
            //    {1, new List<string>{"one"}},
            //    {2, new List<string>{"two"}},
            //    {3, new List<string>{"three"}},
            //    {4, new List<string>{"four"}},
            //    {5, new List<string>{"five"}},
            //    {6, new List<string>{"six"}},
            //    {7, new List<string>{"seven"}},
            //    {8, new List<string>{"eight"}},
            //    {9, new List<string>{"nine"}},
            //    {10, new List<string>{"ten"}},
            //    {11, new List<string>{"eleven"}},
            //    {12, new List<string>{"twelve"}},
            //    {13, new List<string>{"thirteen"}},
            //    {14, new List<string>{"fourteen"}},
            //    {15, new List<string>{"fifteen"}},
            //    {16, new List<string>{"sixteen"}},
            //    {17, new List<string>{"seventeen"}},
            //    {18, new List<string>{"eighteen"}},
            //    {19, new List<string>{"nineteen"}},
            //    {20, new List<string>{"twenty", "score", "scores"}},
            //    {30, new List<string>{"thirty"}},
            //    {40, new List<string>{"forty"}},
            //    {50, new List<string>{"fifty"}},
            //    {60, new List<string>{"sixty"}},
            //    {70, new List<string>{"seventy"}},
            //    {80, new List<string>{"eighty"}},
            //    {90, new List<string>{"ninety"}},
            //    {100, new List<string>{"hundred", "hundreds"}},
            //    {1000, new List<string>{"thousand", "thousands"}},
            //    {1000000, new List<string>{"million", "millions"}},
            //    {1000000000, new List<string>{"billion", "billions"}}//,
            //    //{1000000000000, new List<string>{"Trillion", "Trillions"}}
            //},
            //new List<String> { "and", "-" },
            //new List<String> { "dot" }
            //);
            //Converters.Add(converter.TheEncoding, converter);

            //converter = new ChineseNumberWordConverter(
            //Encoding.GetEncoding("gb2312"),
            //new Dictionary<int, List<string>>
            //{
            //    {10, new List<string>{"十","拾"}},
            //    {20, new List<string>{"廿"}},
            //    {30, new List<string>{"卅"}},
            //    {100, new List<string>{"百", "佰"}},
            //    {1000, new List<string>{"千", "仟"}},
            //    {10000, new List<string>{"万"}},
            //    {100000000, new List<string>{"亿"}}
            //},
            //new List<string>() { "○一二三四五六七八九", "０１２３４５６７８９", "零壹贰叁肆伍陆柒捌玖", "0123456789" }
            //);

            //Converters.Add(converter.TheEncoding, converter);
        //}

        static NumberWordConverter()
        {
            int count = ChineseNumberWordConverter.ChineseAndIndicators.Count;
            count = EnglishNumberWordConverter.EnglishAndIndicator.Count;
        }

        public static NumberWordConverter ConverterOf(Encoding encoding)
        {
            return Converters.ContainsKey(encoding) ? Converters[encoding] : Converters[Encoding.UTF8];
        }

        /// <summary>
        /// Function to determine if "AND" shall be inserted before the remained number words.
        /// </summary>
        /// <param name="number">The original number</param>
        /// <param name="remained">The remained part need to be converted to words</param>
        /// <returns>"true" if "And" is needed.</returns>
        protected static bool insertEnglishAnd(int number, int remained)
        {
            return (number > 100 && (remained > 0 && remained < 100));
        }

        /// <summary>
        /// Function to determine if ZERO shall be inserted before the remained number words, this function works for Chinese culture.
        /// </summary>
        /// <param name="number">The original number</param>
        /// <param name="remained">The remained part need to be converted to words</param>
        /// <returns>"true" if ZERO is needed.</returns>
        protected static bool insertZeroChinese(int number, int remained)
        {
            if (remained == 0)
                return false;

            string numStr = number.ToString();
            string remStr = remained.ToString();
            int len =numStr.Length - remStr.Length;
            return numStr.Substring(0, len)[len-1] == '0';
        }

        #endregion

        #region Property and Field
        //The encoding of the words, can be used to generate a dictionary later
        public Encoding TheEncoding { get; private set; }

        //Delegate to convert word in singular form to plural form, null if no need
        public Func<string, string> ToPlural { get; protected set; }

        //Delegate to determine if a ZERO or AND shall be inserted to the converted words
        public Func<int, int, bool> NeedInsertAnd { get; protected set; }

        //The converted words is case sensitive or not
        public bool IsCaseSensitive { get; private set; }

        //Not used yet
        public bool IsLeftToRight { get; private set; }

        //If SPACE shall be inserted between converted word
        public bool IsSpaceNeeded { get; private set; }

        //The default string to separate words of a converted number
        public string Space  { get; private set; }

        //Dictionary to store the different word format of any standalone number/digit
        public Dictionary<int, List<string>> NumberNameDict = null;

        //Dictionary to enumberate all words defined in NumberNameDict
        public Dictionary<string, int> WordNameDict = null;

        //The numbers less than this shall be taken care specially.
        //Ten as default for decimal system, that is: "Numbered or proceeding by tens; based on ten"
        protected int firstGroupNum = 10;

        //Group names for 10, 100, 1000, 10000 and etc.
        protected List<int> groupNums = null;

        //All words to denote "And" or "Zero"s, which means "0"s between digit of a number
        public List<string> AndWords { get; set; }

        //Shall be used for float number conversion, not implemented yet
        public List<string> PointWords { get; set; }

        //The buffer for storage of the converted words of a number
        public List<string> AllWords { get; set; }
        #endregion

        #region Constructors
        protected NumberWordConverter(Encoding theEncoding, bool isCaseSensitive, bool leftToRight, int firstGroup, string spaceString, Func<string, string> toPlural)
        {
            TheEncoding = theEncoding;
            IsCaseSensitive = isCaseSensitive;
            IsLeftToRight = leftToRight;
            IsSpaceNeeded = spaceString != null && spaceString != "";
            Space = IsSpaceNeeded ? spaceString : "";
            ToPlural = toPlural;
            WordNameDict = new Dictionary<string, int>();
            AllWords = new List<string>();
            firstGroupNum = firstGroup;
            NeedInsertAnd = insertZeroChinese;
            //GroupNameDict = new Dictionary<int, string>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Inistial processing of words string to get number.
        /// Simply get the splitted words.
        /// </summary>
        /// <param name="words">The string of a number as a serial of words</param>
        /// <returns>String array of the string, each contains a word.</returns>
        private string[] split(string words)
        {
            if (IsSpaceNeeded)
                return words.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            else
            {
                var charToString = from ch in words
                                   where !splitter.Contains(ch)
                                   select ch.ToString();
                return charToString.ToArray();
            }
        }

        /// <summary>
        /// The default function to retrieve number from words.
        /// </summary>
        /// <param name="numberInWords">The string composed of words of digits.</param>
        /// <returns>The number</returns>
        public virtual int FromWords(string numberInWords)
        {
            string words = IsCaseSensitive ? numberInWords.ToLower() : numberInWords;

            string[] sectors = split(words);

            return fromWords(sectors);
        }

        /// <summary>
        /// Function to get number from split words.
        /// </summary>
        /// <remarks>
        /// a stack<int> is used for parsing. It is very trick considering:  
        ///     The group name shall generally aligned from larger to smaller
        ///     A larger group name following a smaller one means a compound group. 
        ///     A smaller group name following a larger one means the previous part is a sound unit.
        /// </remarks>
        /// <param name="sectors">Words for each digits of the number</param>
        /// <returns>The number</returns>
        protected int fromWords(string[] sectors)
        {
            int result = 0, current, lastGroup=1, temp, maxGroup=1;
            Stack<int> stack = new Stack<int>();

            foreach (string s in sectors)
            {
                if (AllWords.Contains(s))
                {
                    if (AndWords.Contains(s))
                        continue;

                    if (WordNameDict.ContainsKey(s))
                    {
                        current = WordNameDict[s];

                        if (groupNums.Contains(current))
                        {
                            //The current group is higher than any existed group, thus the digits shall be increased: by Multiply!!!!
                            if(current>= maxGroup)
                            {
                                temp = stack.Count == 0 ? 1 : stack.Pop();
                                while (stack.Count!= 0)
                                {
                                    temp += stack.Pop();
                                };
                                temp *= current;
                                stack.Push(temp);
                                maxGroup *= current;
                                lastGroup = 1;
                            }
                            //The current group is higher than the last group, thus shall be add
                            else if (current > lastGroup)
                            {
                                temp = 0;

                                while(stack.Peek() < current)
                                {
                                    temp += stack.Pop();
                                };

                                temp *= current;
                                stack.Push(temp);
                                lastGroup = current;
                            }
                            else
                            {
                                temp = stack.Pop();
                                temp *= current;
                                stack.Push(temp);
                                lastGroup = current;
                            }
                        }
                        else
                        {
                            stack.Push(current);
                        }
                    }
                }
                else
                    throw new Exception();

            }

            do
            {
                result += stack.Pop();
            } while (stack.Count != 0);

            return result;
        }

        /// <summary>
        /// The generic function of convert a number to words.
        /// </summary>
        /// <param name="number">The objective word</param>
        /// <returns>String of the words</returns>
        public virtual string ToWords(int number)
        {
            if (NumberNameDict.ContainsKey(number) && !groupNums.Contains(number))
                return NumberNameDict[number][0];

            List<string> sections = new List<string>();
            int remained = number;

            for (int i = 0; i < groupNums.Count; i ++ )
            {
                if (remained < groupNums[i])
                    continue;

                int whole = remained / groupNums[i];
                sections.Add(ToWords(whole));

                if (ToPlural != null && whole != 1)
                    sections.Add(ToPlural(NumberNameDict[groupNums[i]][0]));
                else
                    sections.Add(NumberNameDict[groupNums[i]][0]);

                remained -= whole * groupNums[i];

                if (remained != 0 && NeedInsertAnd(number, remained))
                //if(remained != 0 && remained < 100)
                    sections.Add(AndWords[0]);
            }

            if (remained != 0)
                sections.Add(ToWords(remained));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < sections.Count-1; i++)
            {
                sb.Append(sections[i] + Space);
            }
            sb.Append(sections.Last());

            return sb.ToString();
        }

        /// <summary>
        /// The main function to try to retrieve number from string of words.
        /// </summary>
        /// <param name="numberInWords">The original word string of number</param>
        /// <param name="result">The converted number if successful</param>
        /// <returns>TRUE if parse successfully.</returns>
        public virtual bool TryParse(string numberInWords, out int result)
        {
            result = -1;

            try
            {
                string words = IsCaseSensitive ? numberInWords.ToLower() : numberInWords;

                string[] sectors = split(words);

                sectors = (from s in sectors
                            where AllWords.Contains(s)
                            select s).ToArray();

                result = fromWords(sectors);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string NumberStringOf(string numberInWords)
        {
            string words = IsCaseSensitive ? numberInWords.ToLower() : numberInWords;

            string[] sectors = split(words);

            sectors = (from s in sectors
                      where AllWords.Contains(s)
                      select s).ToArray();

            if (sectors.Count() == 0)
                return "";

            int first = words.IndexOf(sectors[0]);
            int last = words.LastIndexOf(sectors[sectors.Count() - 1]);
            int len = last - first + sectors.Last().Length;

            return numberInWords.Substring(first, len);
        }
        
        #region Deleted on Dec-05, 2011
        ///// <summary>
        ///// The function to wrap the FromWords().
        ///// </summary>
        ///// <param name="words">Word string to be parsed.</param>
        ///// <param name="result">The number of the words contained if successful.</param>
        ///// <returns>TRUE if parse is successful.</returns>
        //public bool TryParse(string words, out int result)
        //{
        //    return this.TryParse(words, out result);
        //}

        ///// <summary>
        ///// Function to convert number to string of words.
        ///// </summary>
        ///// <param name="number">The number in digits format.</param>
        ///// <returns>String in words denoting the number.</returns>
        //public string ToWords(int number)
        //{
        //    return this.ToWords(number);
        //}
        #endregion
        #endregion
    }
}
