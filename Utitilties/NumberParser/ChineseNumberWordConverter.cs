using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.NumberParser
{
    public class ChineseNumberWordConverter : NumberWordConverter
    {
        #region Static definitions
        public static List<string> ChineseAndIndicators = new List<String> { "零" };

        public static List<string> ChineseDotIndicators = new List<String> { "点" };

        static ChineseNumberWordConverter()
        {
            ChineseNumberWordConverter converter = new ChineseNumberWordConverter(
            Encoding.GetEncoding("gb2312"),
            new Dictionary<int, List<string>>
            {
                {10, new List<string>{"十","拾"}},
                {20, new List<string>{"廿"}},
                {30, new List<string>{"卅"}},
                {100, new List<string>{"百", "佰"}},
                {1000, new List<string>{"千", "仟"}},
                {10000, new List<string>{"万"}},
                {100000000, new List<string>{"亿"}}
            },
            new List<string>() { "○一二三四五六七八九", "０１２３４５６７８９", "零壹贰叁肆伍陆柒捌玖", "0123456789" }
            );

            Converters.Add(converter.TheEncoding, converter);
        }
        
        #region Codes deleted on Dec 12, 2011
        
        //private static ChineseNumberWordConverter converter = new ChineseNumberWordConverter(
        //    Encoding.GetEncoding("gb2312"),
        //    new Dictionary<int, List<string>>
        //    {
        //        {10, new List<string>{"十","拾"}},
        //        {20, new List<string>{"廿"}},
        //        {30, new List<string>{"卅"}},
        //        {100, new List<string>{"百", "佰"}},
        //        {1000, new List<string>{"千", "仟"}},
        //        {10000, new List<string>{"万"}},
        //        {100000000, new List<string>{"亿"}}
        //    },
        //    new List<string>() { "○一二三四五六七八九", "０１２３４５６７８９", "零壹贰叁肆伍陆柒捌玖", "0123456789" }
        //    );

        /// <summary>
        /// Open function to convert string of words to number, but shall be avoided.
        /// </summary>
        /// <param name="words">Word string to be parsed.</param>
        /// <returns>The number returned.</returns>
        //public static int FromWords(string words)
        //{
        //    return converter.FromWords(words);
        //}

        /// <summary>
        /// Function to convert number to string of words with predefined characters.
        /// </summary>
        /// <param name="number">The number</param>
        /// <param name="samples">
        /// The characters shall be used to replace the default ones.
        /// <example>
        /// For example, 234002052 by default will be converted to "二亿三千四百万零二千零五十二",
        ///     but if the samples is set to "佰零壹贰叁肆拾", then the output will be "贰亿叁千肆佰万零贰千零五拾贰"
        ///     any characters appeared in the samples will replace the default ones, thus "贰" will replace any "二"s for digit of "2".
        /// </example>
        /// </param>
        /// <returns>The converted string in words.</returns>
        //public static string ToWords(int number, string samples)
        //{
        //    return converter.toWords(number, samples);
        //}
        #endregion
        #endregion

        char[] groupIndicators = null;
        char[] allCharacters = null;

        protected ChineseNumberWordConverter(Encoding theEncoding, Dictionary<int, List<string>> numbers, List<string> onesNames)
            : base(theEncoding, false, true, 10, "", null)
        {
            AndWords = ChineseAndIndicators;
            PointWords = ChineseDotIndicators;

            AllWords.AddRange(AndWords);
            AllWords.AddRange(PointWords);

            NumberNameDict = numbers;

            foreach (string names in onesNames)
            {
                if (names.Length != 10)
                    throw new Exception();

                for (int i = 0; i < names.Length; i ++ )
                {
                    string ch = names[i].ToString();

                    if (!NumberNameDict.ContainsKey(i))
                        NumberNameDict.Add(i, new List<string>());

                    if (!NumberNameDict[i].Contains(ch))
                        NumberNameDict[i].Add(ch);
                }
            }

            groupNums = new List<int>();

            int max = NumberNameDict.Keys.Max();
            for (int i = firstGroupNum; i <= max; i*= 10 )
            {
                if (NumberNameDict.ContainsKey(i))
                {
                    //GroupNameDict.Add(i, NumberNameDict[i][0]);
                    groupNums.Add(i);
                }
            }
            groupNums.Sort();

            List<char> indicators = new List<char>();
            foreach(int num in groupNums)
            {
                foreach (string s in NumberNameDict[num])
                {
                    if (s.Length == 1)
                        indicators.Add(s[0]);
                }
            }
            groupIndicators = indicators.ToArray();

            groupNums.Reverse();

            foreach (KeyValuePair<int, List<string>> kvp in NumberNameDict)
            {
                foreach (string s in kvp.Value)
                {
                    WordNameDict.Add(s, kvp.Key);

                    if (!AllWords.Contains(s))
                        AllWords.Add(s);
                }
            }

            indicators.Clear();

            foreach (string s in AllWords)
            {
                if (s.Length == 1)
                    indicators.Add(s[0]);
            }
            allCharacters = indicators.ToArray();
        }


        /// <summary>
        /// The FromWords() for Chinese culture.
        /// </summary>
        /// <param name="numberInWords">The words string of the number.</param>
        /// <returns>The number.</returns>
        public override int FromWords(string numberInWords)
        {
            if (groupIndicators != null && numberInWords.IndexOfAny(groupIndicators) == -1)
                return fromAsIsWords(numberInWords);
            else
                return base.FromWords(numberInWords);
        }

        /// <summary>
        /// Special version for word strings with no group word included. 
        /// </summary>
        /// <example>
        /// 12345 shall be read as "一万二千三百四十五", but sometime it may be presented as "一二三四五": one word for one digit.
        /// Then fromAsIsWords() is applied to parse the later correctly.
        /// </example>
        /// <param name="words"></param>
        /// <returns></returns>
        private int fromAsIsWords(string words)
        {
            string asIsString = "";

            foreach (char ch in words)
            {
                asIsString += WordNameDict[ch.ToString()];
            }
            return int.Parse(asIsString);
        }

        /// <summary>
        /// ToWord() for Chinese culture.
        /// </summary>
        /// <summary>
        /// Function to convert number to string of words with predefined characters.
        /// </summary>
        /// <param name="number">The number</param>
        /// <param name="samples">
        /// The characters shall be used to replace the default ones.
        /// <example>
        /// For example, 234002052 by default will be converted to "二亿三千四百万零二千零五十二",
        ///     but if the samples is set to "佰零壹贰叁肆拾", then the output will be "贰亿叁千肆佰万零贰千零五拾贰"
        ///     any characters appeared in the samples will replace the default ones, thus "贰" will replace any "二"s for digit of "2".
        /// </example>
        /// </param>
        /// <returns>The converted string in words.</returns>
        private string toWords(int number, string samples)
        {
            string result = ToWords(number);

            foreach (char ch in samples)
            {
                if (allCharacters.Contains(ch) && WordNameDict.ContainsKey(ch.ToString()))
                {
                    int digit = WordNameDict[ch.ToString()];
                    if (digit > 9 && !groupNums.Contains(digit))
                        continue;

                    string digitStr = NumberNameDict[digit][0];

                    if (digitStr.Length != 1 || digitStr[0] == ch)
                        continue;

                    result = result.Replace(digitStr[0], ch);
                }
            }

            return result;
        }
    }
}
