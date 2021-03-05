using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.NumberParser
{
    public class EnglishNumberWordConverter : NumberWordConverter
    {
        public static List<string> EnglishAndIndicator = new List<String> { "and", "-" };
        static EnglishNumberWordConverter()
        {
            EnglishNumberWordConverter converter = new EnglishNumberWordConverter(
                        new Dictionary<int, List<string>>
                        {
                            {0, new List<string>{"zero"}},
                            {1, new List<string>{"one"}},
                            {2, new List<string>{"two"}},
                            {3, new List<string>{"three"}},
                            {4, new List<string>{"four"}},
                            {5, new List<string>{"five"}},
                            {6, new List<string>{"six"}},
                            {7, new List<string>{"seven"}},
                            {8, new List<string>{"eight"}},
                            {9, new List<string>{"nine"}},
                            {10, new List<string>{"ten"}},
                            {11, new List<string>{"eleven"}},
                            {12, new List<string>{"twelve"}},
                            {13, new List<string>{"thirteen"}},
                            {14, new List<string>{"fourteen"}},
                            {15, new List<string>{"fifteen"}},
                            {16, new List<string>{"sixteen"}},
                            {17, new List<string>{"seventeen"}},
                            {18, new List<string>{"eighteen"}},
                            {19, new List<string>{"nineteen"}},
                            {20, new List<string>{"twenty", "score", "scores"}},
                            {30, new List<string>{"thirty"}},
                            {40, new List<string>{"forty"}},
                            {50, new List<string>{"fifty"}},
                            {60, new List<string>{"sixty"}},
                            {70, new List<string>{"seventy"}},
                            {80, new List<string>{"eighty"}},
                            {90, new List<string>{"ninety"}},
                            {100, new List<string>{"hundred", "hundreds"}},
                            {1000, new List<string>{"thousand", "thousands"}},
                            {1000000, new List<string>{"million", "millions"}},
                            {1000000000, new List<string>{"billion", "billions"}}//,
                            //{1000000000000, new List<string>{"Trillion", "Trillions"}}
                        },
                        EnglishAndIndicator,
                        new List<String> { "dot" }
                        );

            Converters.Add(converter.TheEncoding, converter);
        }

        #region Static definitions

        //public static string ToWords(int number)
        //{
        //    return converter.ToWords(number);
        //}

        //public static string ToWords(int number, WordsFormat format)
        //{
        //    return converter.toWords(number, format);
        //}

        //private static EnglishNumberWordConverter converter = new EnglishNumberWordConverter(
        //    new Dictionary<int, List<string>>
        //    {
        //        {0, new List<string>{"zero"}},
        //        {1, new List<string>{"one"}},
        //        {2, new List<string>{"two"}},
        //        {3, new List<string>{"three"}},
        //        {4, new List<string>{"four"}},
        //        {5, new List<string>{"five"}},
        //        {6, new List<string>{"six"}},
        //        {7, new List<string>{"seven"}},
        //        {8, new List<string>{"eight"}},
        //        {9, new List<string>{"nine"}},
        //        {10, new List<string>{"ten"}},
        //        {11, new List<string>{"eleven"}},
        //        {12, new List<string>{"twelve"}},
        //        {13, new List<string>{"thirteen"}},
        //        {14, new List<string>{"fourteen"}},
        //        {15, new List<string>{"fifteen"}},
        //        {16, new List<string>{"sixteen"}},
        //        {17, new List<string>{"seventeen"}},
        //        {18, new List<string>{"eighteen"}},
        //        {19, new List<string>{"nineteen"}},
        //        {20, new List<string>{"twenty", "score", "scores"}},
        //        {30, new List<string>{"thirty"}},
        //        {40, new List<string>{"forty"}},
        //        {50, new List<string>{"fifty"}},
        //        {60, new List<string>{"sixty"}},
        //        {70, new List<string>{"seventy"}},
        //        {80, new List<string>{"eighty"}},
        //        {90, new List<string>{"ninety"}},
        //        {100, new List<string>{"hundred", "hundreds"}},
        //        {1000, new List<string>{"thousand", "thousands"}},
        //        {1000000, new List<string>{"million", "millions"}},
        //        {1000000000, new List<string>{"billion", "billions"}}//,
        //        //{1000000000000, new List<string>{"Trillion", "Trillions"}}
        //    },
        //    new List<String> { "and", "-" },
        //    new List<String> { "dot" }
        //    );
        #endregion

        protected List<int> lowNums = null;

        protected EnglishNumberWordConverter(Dictionary<int, List<string>> numbers,
            List<string> andStrings, List<string> pointStrings)
            : base(Encoding.UTF8, true, true, 100, " ", Pluralizer.ToPlural)
        {
            NeedInsertAnd = insertEnglishAnd;
            AndWords = andStrings;
            PointWords = pointStrings;

            AllWords.AddRange(AndWords);
            AllWords.AddRange(PointWords);

            NumberNameDict = numbers;

            groupNums = new List<int>();
            int max = NumberNameDict.Keys.Max();
            for (int i = firstGroupNum; i <= max; i *= 10)
            {
                if (NumberNameDict.ContainsKey(i))
                {
                    //GroupNameDict.Add(i, NumberNameDict[i][0]);
                    groupNums.Add(i);
                }
            }
            groupNums.Sort();
            groupNums.Reverse();

            lowNums = (from i in NumberNameDict.Keys
                       where i < firstGroupNum && i != 0
                       orderby i descending
                       select i
                           ).ToList();


            foreach (KeyValuePair<int, List<string>> kvp in NumberNameDict)
            {
                foreach (string s in kvp.Value)
                {
                    WordNameDict.Add(s, kvp.Key);

                    if (!AllWords.Contains(s))
                        AllWords.Add(s);
                }
            }

            if (IsCaseSensitive)
            {
                for(int i=0; i < AllWords.Count; i ++)
                {
                    AllWords[i] = AllWords[i].ToLower();
                }
            }

            ////To enforce the initialization of the Pluralizer class
            //bool isPlural = Pluralizer.IsNounPluralOfNoun("ones", "one");
        }

        public override string ToWords(int number)
        {
            if (number < firstGroupNum)
            {
                if (NumberNameDict.ContainsKey(number))
                    return NumberNameDict[number][0];

                int largest = (from i in lowNums
                               where i <= number
                               select i).First();
                int remained = number - largest;

                return string.Format("{0}{1}{2}", NumberNameDict[largest][0], AndWords[1], NumberNameDict[remained][0]);
            }
            else
                return base.ToWords(number);
        }

        protected string toWords(int number, WordsFormat format)
        {
            string lowCase = ToWords(number);
            if (format == WordsFormat.LowCaseOnly)
                return lowCase;
            else if (format == WordsFormat.UpperCaseOnly)
                return lowCase.ToUpper();

            string[] subs = lowCase.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < subs.Length; i++)
            {
                char[] chars = subs[i].ToCharArray();
                chars[0] = (char)(chars[0] + 'A' - 'a');
                sb.Append(new string(chars) + ((i == subs.Length-1) ? "" : Space));
            }

            return sb.ToString();

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://coreex.googlecode.com/svn-history/r195/branches/development/Source/CoreEx.Common/Extensions/Pluralizer.cs"/>
    public static class Pluralizer
    {
        #region public APIs

        public static string ToPlural(string noun)
        {
            return AdjustCase(ToPluralInternal(noun), noun);
        }

        public static string ToSingular(string noun)
        {
            return AdjustCase(ToSingularInternal(noun), noun);
        }

        public static bool IsNounPluralOfNoun(string plural, string singular)
        {
            return String.Compare(ToSingularInternal(plural), singular, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion

        #region Special Words Table

        static string[] _specialWordsStringTable = new string[] {
            "agendum",          "agenda",           "",
            "albino",           "albinos",          "",
            "alga",             "algae",            "",
            "alumna",           "alumnae",          "",
            "apex",             "apices",           "apexes",
            "archipelago",      "archipelagos",     "",
            "bacterium",        "bacteria",         "",
            "beef",             "beefs",            "beeves",
            "bison",            "",                 "",
            "brother",          "parsedElements",         "brethren",
            "candelabrum",      "candelabra",       "",
            "carp",             "",                 "",
            "casino",           "casinos",          "",
            "child",            "children",         "",
            "chassis",          "",                 "",
            "chinese",          "",                 "",
            "clippers",         "",                 "",
            "cod",              "",                 "",
            "codex",            "codices",          "",
            "commando",         "commandos",        "",
            "corps",            "",                 "",
            "cortex",           "cortices",         "cortexes",
            "cow",              "cows",             "kine",
            "criterion",        "criteria",         "",
            "datum",            "data",             "",
            "debris",           "",                 "",
            "diabetes",         "",                 "",
            "ditto",            "dittos",           "",
            "djinn",            "",                 "",
            "dynamo",           "",                 "",
            "elk",              "",                 "",
            "embryo",           "embryos",          "",
            "ephemeris",        "ephemeris",        "ephemerides",
            "erratum",          "errata",           "",
            "extremum",         "extrema",          "",
            "fiasco",           "fiascos",          "",
            "fish",             "fishes",           "fish",
            "flounder",         "",                 "",
            "focus",            "focuses",          "foci",
            "fungus",           "fungi",            "funguses",
            "gallows",          "",                 "",
            "genie",            "genies",           "genii",
            "ghetto",           "ghettos",           "",
            "graffiti",         "",                 "",
            "headquarters",     "",                 "",
            "herpes",           "",                 "",
            "homework",         "",                 "",
            "index",            "indices",          "indexes",
            "inferno",          "infernos",         "",
            "japanese",         "",                 "",
            "jumbo",            "jumbos",            "",
            "latex",            "latices",          "latexes",
            "lingo",            "lingos",           "",
            "mackerel",         "",                 "",
            "macro",            "macros",           "",
            "manifesto",        "manifestos",       "",
            "measles",          "",                 "",
            "money",            "moneys",           "monies",
            "mongoose",         "mongooses",        "mongoose",
            "mumps",            "",                 "",
            "murex",            "murecis",          "",
            "mythos",           "mythos",           "mythoi",
            "news",             "",                 "",
            "octopus",          "octopuses",        "octopodes",
            "ovum",             "ova",              "",
            "ox",               "ox",               "oxen",
            "photo",            "photos",           "",
            "pincers",          "",                 "",
            "pliers",           "",                 "",
            "pro",              "pros",             "",
            "rabies",           "",                 "",
            "radius",           "radiuses",         "radii",
            "rhino",            "rhinos",           "",
            "salmon",           "",                 "",
            "scissors",         "",                 "",
            "series",           "",                 "",
            "shears",           "",                 "",
            "silex",            "silices",          "",
            "simplex",          "simplices",        "simplexes",
            "soliloquy",        "soliloquies",      "soliloquy",
            "species",          "",                 "",
            "stratum",          "strata",           "",
            "swine",            "",                 "",
            "trout",            "",                 "",
            "tuna",             "",                 "",
            "vertebra",         "vertebrae",        "",
            "vertex",           "vertices",         "vertexes",
            "vortex",           "vortices",         "vortexes",
        };

        #endregion

        #region Suffix Rules Table

        static string[] _suffixRulesStringTable = new string[] {
            "ch",       "ches",
            "sh",       "shes",
            "ss",       "sses",

            "ay",       "ays",
            "ey",       "eys",
            "iy",       "iys",
            "oy",       "oys",
            "uy",       "uys",
            "y",        "ies",

            "ao",       "aos",
            "eo",       "eos",
            "io",       "ios",
            "oo",       "oos",
            "uo",       "uos",
            "o",        "oes",

            "cis",      "ces",
            "sis",      "ses",
            "xis",      "xes",

            "louse",    "lice",
            "mouse",    "mice",

            "zoon",     "zoa",

            "man",      "men",

            "deer",     "deer",
            "fish",     "fish",
            "sheep",    "sheep",
            "itis",     "itis",
            "ois",      "ois",
            "pox",      "pox",
            "ox",       "oxes",

            "foot",     "feet",
            "goose",    "geese",
            "tooth",    "teeth",

            "alf",      "alves",
            "elf",      "elves",
            "olf",      "olves",
            "arf",      "arves",
            "leaf",     "leaves",
            "nife",     "nives",
            "life",     "lives",
            "wife",     "wives",
        };

        #endregion

        #region Implementation Details

        class Word
        {
            public readonly string Singular;
            public readonly string Plural;
            public readonly string Plural2;

            public Word(string singular, string plural, string plural2)
            {
                Singular = singular;
                Plural = plural;
                Plural2 = plural2;
            }
        }

        class SuffixRule
        {
            string _singularSuffix;
            string _pluralSuffix;

            public SuffixRule(string singular, string plural)
            {
                _singularSuffix = singular;
                _pluralSuffix = plural;
            }

            public bool TryToPlural(string word, out string plural)
            {
                if (word.EndsWith(_singularSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    plural = word.Substring(0, word.Length - _singularSuffix.Length) + _pluralSuffix;
                    return true;
                }
                else
                {
                    plural = null;
                    return false;
                }
            }

            public bool TryToSingular(string word, out string singular)
            {
                if (word.EndsWith(_pluralSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    singular = word.Substring(0, word.Length - _pluralSuffix.Length) + _singularSuffix;
                    return true;
                }
                else
                {
                    singular = null;
                    return false;
                }
            }
        }

        static Dictionary<string, Word> _specialSingulars;
        static Dictionary<string, Word> _specialPlurals;
        static List<SuffixRule> _suffixRules;

        static Pluralizer()
        {
            // populate lookup tables for special words
            _specialSingulars = new Dictionary<string, Word>(StringComparer.OrdinalIgnoreCase);
            _specialPlurals = new Dictionary<string, Word>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < _specialWordsStringTable.Length; i += 3)
            {
                string s = _specialWordsStringTable[i];
                string p = _specialWordsStringTable[i + 1];
                string p2 = _specialWordsStringTable[i + 2];

                if (string.IsNullOrEmpty(p))
                {
                    p = s;
                }

                Word w = new Word(s, p, p2);

                _specialSingulars.Add(s, w);
                _specialPlurals.Add(p, w);

                if (!string.IsNullOrEmpty(p2))
                {
                    _specialPlurals.Add(p2, w);
                }
            }

            // populate suffix rules list
            _suffixRules = new List<SuffixRule>();

            for (int i = 0; i < _suffixRulesStringTable.Length; i += 2)
            {
                string singular = _suffixRulesStringTable[i];
                string plural = _suffixRulesStringTable[i + 1];
                _suffixRules.Add(new SuffixRule(singular, plural));
            }
        }

        static string ToPluralInternal(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            // lookup special words
            Word word;

            if (_specialSingulars.TryGetValue(s, out word))
            {
                return word.Plural;
            }

            // apply suffix rules
            string plural;

            foreach (SuffixRule rule in _suffixRules)
            {
                if (rule.TryToPlural(s, out plural))
                {
                    return plural;
                }
            }

            // apply the default rule
            return s + "s";
        }

        static string ToSingularInternal(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            // lookup special words
            Word word;

            if (_specialPlurals.TryGetValue(s, out word))
            {
                return word.Singular;
            }

            // apply suffix rules
            string singular;

            foreach (SuffixRule rule in _suffixRules)
            {
                if (rule.TryToSingular(s, out singular))
                {
                    return singular;
                }
            }

            // apply the default rule
            if (s.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return s.Substring(0, s.Length - 1);
            }

            return s;
        }

        static string AdjustCase(string s, string template)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            // determine the type of casing of the template string
            bool foundUpperOrLower = false;
            bool allLower = true;
            bool allUpper = true;
            bool firstUpper = false;

            for (int i = 0; i < template.Length; i++)
            {
                if (Char.IsUpper(template[i]))
                {
                    if (i == 0) firstUpper = true;
                    allLower = false;
                    foundUpperOrLower = true;
                }
                else if (Char.IsLower(template[i]))
                {
                    allUpper = false;
                    foundUpperOrLower = true;
                }
            }

            // change the case according to template
            if (foundUpperOrLower)
            {
                if (allLower)
                {
                    s = s.ToLowerInvariant();
                }
                else if (allUpper)
                {
                    s = s.ToUpperInvariant();
                }
                else if (firstUpper)
                {
                    if (!Char.IsUpper(s[0]))
                    {
                        s = s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
                    }
                }
            }

            return s;
        }

        #endregion
    }
}
