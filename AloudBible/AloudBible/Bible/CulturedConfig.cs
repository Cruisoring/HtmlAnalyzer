using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.NumberParser;
using System.IO;

namespace AloudBible.Bible
{
    public class CulturedConfig
    {
        #region Definitions
        public const int DefaultFormalNameIndex = 0;
        public static int DefaultAbbreviationIndex = 1;

        public static char[] DefaultWhiteSpaces = new char[] { ' ', '(', ')', '-', ',', '.', '_' };
        public static char[] DefaultVerseSplitter = new char[] { ' ', ':', '-' };

        #region For Chinese
        public static Dictionary<BibleCode, List<string>> ChineseNames = new Dictionary<BibleCode, List<string>>
        {
            //Written by Moses              
            { BibleCode.Genesis, new List<string>{"创世记", "创", "创世纪"}},            //1. Genesis 
            { BibleCode.Exodus, new List<string>{"出埃及记", "出"}},              //2. Exodus 
            { BibleCode.Leviticus, new List<string>{"利未记", "利"}},              //3. Leviticus 
            { BibleCode.Numbers, new List<string>{"民数记", "民"}},              //4. Numbers 
            { BibleCode.Deuteronomy, new List<string>{"申命记", "申"}},              //5. Deuteronomy 

            //OT Narratives
            { BibleCode.Joshua, new List<string>{"约书亚记", "书"}},              //6. Joshua 
            { BibleCode.Judges, new List<string>{"士师记", "士"}},              //7. Judges
            { BibleCode.Ruth, new List<string>{"路得记", "得"}},              //8. Ruth
            { BibleCode.Samuel1, new List<string>{"撒母耳记上", "撒上"}},              //9. 1 Samuel
            { BibleCode.Samuel2, new List<string>{"撒母耳记下", "撒下"}},              //10. 2 Samuel
            { BibleCode.Kings1, new List<string>{"列王记上", "王上", "列王纪上"}},              //11. 1 Kings
            { BibleCode.Kings2, new List<string>{"列王记下", "王下", "列王纪下"}},              //12. 2 Kings
            { BibleCode.Chronicles1, new List<string>{"历代记上", "代上", "历代志上"}},              //13. 1 Chronicles
            { BibleCode.Chronicles2, new List<string>{"历代记下", "代下", "历代志下"}},              //14. 2 Chronicles
            { BibleCode.Ezra, new List<string>{"以斯拉记", "拉", "以斯拉"}},              //15. Ezra
            { BibleCode.Nehemiah, new List<string>{"尼希米记", "尼"}},              //16. Nehemiah
            { BibleCode.Esther, new List<string>{"以斯帖记", "斯", "以斯贴", "以斯帖"}},              //17. Esther

            //Wisdom Literature
            { BibleCode.Job, new List<string>{"约伯记", "伯"}},              //18. Job
            { BibleCode.Psalms, new List<string>{"诗篇", "诗"}},              //19. Psalms
            { BibleCode.Proverbs, new List<string>{"箴言", "箴"}},              //20. Proverbs
            { BibleCode.Ecclesiastes, new List<string>{"传道书", "传"}},              //21. Ecclesiastes
            { BibleCode.Songs, new List<string>{"雅歌", "歌"}},              //22. Song of Songs

            //Major Prophets
            { BibleCode.Isaiah, new List<string>{"以赛亚书", "赛"}},              //23. Isaiah
            { BibleCode.Jeremiah, new List<string>{"耶利米书", "耶"}},              //24. Jeremiah
            { BibleCode.Lamentations, new List<string>{"耶利米哀歌", "哀", "耶哀"}},              //25. Lamentations
            { BibleCode.Ezekiel, new List<string>{"以西结书", "结"}},              //26. Ezekiel
            { BibleCode.Daniel, new List<string>{"但以理书", "但"}},              //27. Daniel

            //Minor Prophets
            { BibleCode.Hosea, new List<string>{"何西阿书", "何"}},              //28. Hosea 
            { BibleCode.Joel, new List<string>{"约珥书", "珥"}},              //29. Joel{ BibleCode.
            { BibleCode.Amos, new List<string>{"阿摩司书", "摩"}},              //30. Amos
            { BibleCode.Obadiah, new List<string>{"俄巴底亚书", "俄"}},              //31. Obadiah
            { BibleCode.Jonah, new List<string>{"约拿书", "拿"}},              //32. Jonah
            { BibleCode.Micah, new List<string>{"弥迦书", "弥"}},              //33. Micah
            { BibleCode.Nahum, new List<string>{"那鸿书", "鸿"}},              //34. Nahum
            { BibleCode.Habakkuk, new List<string>{"哈巴谷书", "哈"}},              //35. Habakkuk
            { BibleCode.Zephaniah, new List<string>{"西番雅书", "番"}},              //36. Zephaniah
            { BibleCode.Haggai, new List<string>{"哈该书", "该"}},              //37. Haggai
            { BibleCode.Zechariah, new List<string>{"撒迦利亚书", "亚"}},              //38. Zechariah
            { BibleCode.Malachi, new List<string>{"玛拉基书", "玛"}},              //39. Malachi 

            //NT Narratives
            { BibleCode.Matthew, new List<string>{"马太福音", "太"}},              //40. Matthew
            { BibleCode.Mark, new List<string>{"马可福音", "可"}},              //41. Mark
            { BibleCode.Luke, new List<string>{"路加福音", "路"}},              //42. Luke
            { BibleCode.John, new List<string>{"约翰福音", "约"}},              //43. John
            { BibleCode.Acts, new List<string>{"使徒行传", "徒"}},              //44. Acts

            //Epistles by Paul 
            { BibleCode.Romans, new List<string>{"罗马书", "罗"}},              //45. Romans 
            { BibleCode.Corinthians1, new List<string>{"歌林多前书", "林前", "哥林多前书"}},              //46. 1 Corinthians 
            { BibleCode.Corinthians2, new List<string>{"歌林多后书", "林后", "哥林多后书"}},              //47. 2 Corinthians 
            { BibleCode.Galatians, new List<string>{"加拉太书", "加"}},              //48. Galatians 
            { BibleCode.Ephesians, new List<string>{"以弗所书", "弗"}},              //49. Ephesians 
            { BibleCode.Philippians, new List<string>{"腓立比书", "腓", "腓利比书"}},              //50. Philippians 
            { BibleCode.Colossians, new List<string>{"歌罗西书", "西"}},              //51. Colossians 
            { BibleCode.Thessalonians1, new List<string>{"帖撒罗尼迦前书", "帖前", "贴前", "贴撒罗尼迦前书"}},              //52. 1 Thessalonians
            { BibleCode.Thessalonians2, new List<string>{"帖撒罗尼迦后书", "帖后", "贴后", "贴撒罗尼迦后书"}},              //53. 2 Thessalonians 
            { BibleCode.Timothy1, new List<string>{"提摩太前书", "提前"}},              //54. 1 Timothy 
            { BibleCode.Timothy2, new List<string>{"提摩太后书", "提后"}},              //55. 2 Timothy 
            { BibleCode.Titus, new List<string>{"提多书", "多"}},              //56. Titus 
            { BibleCode.Philemon, new List<string>{"腓利门书", "门"}},              //57. Philemon

            //General Epistles 
            { BibleCode.Hebrews, new List<string>{"希伯来书", "来"}},              //58. Hebrews 
            { BibleCode.James, new List<string>{"雅各书", "雅"}},              //59. James 
            { BibleCode.Peter1, new List<string>{"彼得前书", "彼前"}},              //60. 1 Peter 
            { BibleCode.Peter2, new List<string>{"彼得后书", "彼后"}},              //61. 2 Peter 
            { BibleCode.John1, new List<string>{"约翰一书", "约一"}},              //62. 1 John 
            { BibleCode.John2, new List<string>{"约翰二书", "约二"}},              //63. 2 John 
            { BibleCode.John3, new List<string>{"约翰三书", "约三"}},              //64. 3 John 
            { BibleCode.Jude, new List<string>{"犹大书", "犹"}},              //65. Jude

            //Apocalyptic Epistle by John
            { BibleCode.Revelation, new List<string>{"启示录", "启"}},             //66. Revelation

            { BibleCode.Verses, new List<string>{"节", "节", "句"} },
            { BibleCode.Paragraphs, new List<string>{"段", "段"} },
            { BibleCode.Chapters, new List<string>{"章", "章", "篇", "回", "目"} },
            { BibleCode.Books, new List<string>{"书", "书", "卷"} },

            { BibleCode.OldTestament, new List<string>{"旧约", "旧约", "旧约全书", "旧约圣经"}},

            {BibleCode.Pentateuch, new List<string>{"律法卷", "律法书", "摩西五经"}},
            {BibleCode.OldTimeHistory, new List<string>{"历史卷", "历史书"}},
            {BibleCode.Wisdom, new List<string>{"智慧卷", "智慧书", "诗歌书"}},

            {BibleCode.Prophecy, new List<string>{"先知卷", "先知书"}},
            {BibleCode.MajorProphecy, new List<string>{"大先知卷", "大先知书"}},
            {BibleCode.MinorProphecy, new List<string>{"小先知卷", "小先知书"}},

            { BibleCode.NewTestament, new List<string>{"新约", "新约", "新约全书", "新约圣经"}},             
            {BibleCode.Gospels, new List<string>{"福音卷", "福音书"}},
            {BibleCode.Biographies, new List<string>{"福音四书"}},
            {BibleCode.Apostolic, new List<string>{"宗徒大事录"}},

            {BibleCode.Epistles, new List<string>{"书信卷", "教义卷"}},
            {BibleCode.PaulineEpistles, new List<string>{"保罗卷", "保罗书信卷"}},
            {BibleCode.GeneralEpistles, new List<string>{"大公卷", "普通书信卷"}},
            {BibleCode.Apoclyptic, new List<string>{"预言卷"}},

            { BibleCode.Scripture, new List<string>{"圣经", "圣经", "新旧约全书"}}
        };
        #endregion

        #region For English
        public static Dictionary<BibleCode, List<string>> EnglishNames = new Dictionary<BibleCode, List<string>>
        {
            //Written by Moses              
            { BibleCode.Genesis, new List<string>{"Genesis", "Gn", "Gen."}},            //1. Genesis 
            { BibleCode.Exodus, new List<string>{"Exodus", "Ex"}},              //2. Exodus 
            { BibleCode.Leviticus, new List<string>{"Leviticus", "Lv", "Lev."}},              //3. Leviticus 
            { BibleCode.Numbers, new List<string>{"Numbers", "Num"}},              //4. Numbers 
            { BibleCode.Deuteronomy, new List<string>{"Deuteronomy", "Dt", "Deut."}},              //5. Deuteronomy 

            //OT Narratives
            { BibleCode.Joshua, new List<string>{"Joshua", "Jo", "Josh."}},              //6. Joshua 
            { BibleCode.Judges, new List<string>{"Judges", "Jgs", "Judg."}},              //7. Judges
            { BibleCode.Ruth, new List<string>{"Ruth", "Ru", "Ruth"}},              //8. Ruth
            { BibleCode.Samuel1, new List<string>{"1 Samuel", "1 Sm", "1 Sam."}},              //9. 1 Samuel
            { BibleCode.Samuel2, new List<string>{"2 Samuel", "2 Sm", "2 Sam"}},              //10. 2 Samuel
            { BibleCode.Kings1, new List<string>{"1 Kings", "1 Kgs", "1 Kings"}},              //11. 1 Kings
            { BibleCode.Kings2, new List<string>{"2 Kings", "2 Kgs", "2 Kings"}},              //12. 2 Kings
            { BibleCode.Chronicles1, new List<string>{"1 Chronicles", "1 Chr", "1 Chron."}},              //13. 1 Chronicles
            { BibleCode.Chronicles2, new List<string>{"2 Chronicles", "2 Chr", "2 Chron."}},              //14. 2 Chronicles
            { BibleCode.Ezra, new List<string>{"Ezra", "Ezr", "Ezra"}},              //15. Ezra
            { BibleCode.Nehemiah, new List<string>{"Nehemiah", "Neh", "Neh."}},              //16. Nehemiah
            { BibleCode.Esther, new List<string>{"Esther", "Est", "Esth"}},              //17. Esther

            //Wisdom Literature
            { BibleCode.Job, new List<string>{"Job", "Jb"}},              //18. Job
            { BibleCode.Psalms, new List<string>{"Psalms", "Ps", "Pss"}},              //19. Psalms
            { BibleCode.Proverbs, new List<string>{"Proverbs", "Prv", "Prov."}},              //20. Proverbs
            { BibleCode.Ecclesiastes, new List<string>{"Ecclesiastes", "Eccl", "Eccles."}},              //21. Ecclesiastes
            { BibleCode.Songs, new List<string>{"Song of Songs", "Sg", "Song of Sol."}},              //22. Song of Songs
 
            //Major Prophets
            { BibleCode.Isaiah, new List<string>{"Isaiah", "Is", "Isa."}},              //23. Isaiah
            { BibleCode.Jeremiah, new List<string>{"Jeremiah", "Jer"}},              //24. Jeremiah
            { BibleCode.Lamentations, new List<string>{"Lamentations", "Lam"}},              //25. Lamentations
            { BibleCode.Ezekiel, new List<string>{"Ezekiel", "Ez", "Ezek."}},              //26. Ezekiel
            { BibleCode.Daniel, new List<string>{"Daniel", "Dn", "Dan."}},              //27. Daniel

            //Minor Prophets
            { BibleCode.Hosea, new List<string>{"Hosea", "Hos"}},              //28. Hosea 
            { BibleCode.Joel, new List<string>{"Joel", "Jl"}},              //29. Joel{ BibleCode.
            { BibleCode.Amos, new List<string>{"Amos", "Am"}},              //30. Amos
            { BibleCode.Obadiah, new List<string>{"Obadiah", "Ob", "Obad."}},              //31. Obadiah
            { BibleCode.Jonah, new List<string>{"Jonah", "Jon"}},              //32. Jonah
            { BibleCode.Micah, new List<string>{"Micah", "Mi", "Mic."}},              //33. Micah
            { BibleCode.Nahum, new List<string>{"Nahum", "Na", "Nah."}},              //34. Nahum
            { BibleCode.Habakkuk, new List<string>{"Habakkuk", "Hb", "Hab."}},              //35. Habakkuk
            { BibleCode.Zephaniah, new List<string>{"Zephaniah", "Zep", "Zeph."}},              //36. Zephaniah
            { BibleCode.Haggai, new List<string>{"Haggai", "Hg", "Hag."}},              //37. Haggai
            { BibleCode.Zechariah, new List<string>{"Zechariah", "Zec", "Zech."}},              //38. Zechariah
            { BibleCode.Malachi, new List<string>{"Malachi", "Mal"}},              //39. Malachi 

            //NT Narratives
            { BibleCode.Matthew, new List<string>{"Matthew", "Mt", "Matt."}},              //40. Matthew
            { BibleCode.Mark, new List<string>{"Mark", "Mk"}},              //41. Mark
            { BibleCode.Luke, new List<string>{"Luke", "Lk"}},              //42. Luke
            { BibleCode.John, new List<string>{"John (Gospel)", "Jn"}},              //43. John
            { BibleCode.Acts, new List<string>{"Acts of the Apostles", "Acts"}},              //44. Acts

            //Epistles by Paul 
            { BibleCode.Romans, new List<string>{"Romans", "Rom"}},              //45. Romans 
            { BibleCode.Corinthians1, new List<string>{"1 Corinthians", "1 Cor"}},              //46. 1 Corinthians 
            { BibleCode.Corinthians2, new List<string>{"2 Corinthians", "2 Cor"}},              //47. 2 Corinthians 
            { BibleCode.Galatians, new List<string>{"Galatians", "Gal"}},              //48. Galatians 
            { BibleCode.Ephesians, new List<string>{"Ephesians", "Eph"}},              //49. Ephesians 
            { BibleCode.Philippians, new List<string>{"Philippians", "Phil"}},              //50. Philippians 
            { BibleCode.Colossians, new List<string>{"Colossians", "Col"}},              //51. Colossians 
            { BibleCode.Thessalonians1, new List<string>{"1 Thessalonians", "1 Thes", "1 Thess."}},              //52. 1 Thessalonians
            { BibleCode.Thessalonians2, new List<string>{"2 Thessalonians", "2 Thes", "2 Thess."}},              //53. 2 Thessalonians 
            { BibleCode.Timothy1, new List<string>{"1 Timothy", "1 Tm", "1 Tim."}},              //54. 1 Timothy 
            { BibleCode.Timothy2, new List<string>{"2 Timothy", "2 Tm", "2 Tim."}},              //55. 2 Timothy 
            { BibleCode.Titus, new List<string>{"Titus", "Ti"}},              //56. Titus 
            { BibleCode.Philemon, new List<string>{"Philemon", "Phlm"}},              //57. Philemon

            //General Epistles 
            { BibleCode.Hebrews, new List<string>{"Hebrews", "Heb"}},              //58. Hebrews 
            { BibleCode.James, new List<string>{"James", "Jas"}},              //59. James 
            { BibleCode.Peter1, new List<string>{"1 Peter", "1 Pt", "1 Pet."}},              //60. 1 Peter 
            { BibleCode.Peter2, new List<string>{"2 Peter", "2 Pt", "2 Pet."}},              //61. 2 Peter 
            { BibleCode.John1, new List<string>{"1 John (Epistle)", "1 Jn"}},              //62. 1 John 
            { BibleCode.John2, new List<string>{"2 John (Epistle)", "2 Jn"}},              //63. 2 John 
            { BibleCode.John3, new List<string>{"3 John (Epistle)", "3 Jn"}},              //64. 3 John 
            { BibleCode.Jude, new List<string>{"Jude", "Jude"}},              //65. Jude

            //Apocalyptic Epistle by John
            { BibleCode.Revelation, new List<string>{"Revelation", "Rv", "Rev.", "Apocalypse"}},             //66. Revelation

            //Common names for bible
            { BibleCode.Verses, new List<string>{"Verse", "Verse", "Section", "Sentence"}},
            { BibleCode.Paragraphs, new List<string>{"Paragraph", "Para"}},
            { BibleCode.Chapters, new List<string>{"Chapter", "Chap", "Chp", "Ch"}},
            { BibleCode.Books, new List<string>{"Book", "Book", "Bk"}},

            { BibleCode.OldTestament, new List<string>{"Old Testament", "Old Testaments"}},          //Old Testament   

            {BibleCode.Pentateuch, new List<string>{"Pentateuch", "Law", "Torah", "Moses", "Five Books"}},
            {BibleCode.OldTimeHistory, new List<string>{"Old Time History", "Old Testament History"}},
            {BibleCode.Wisdom, new List<string>{"Wisdom", "Praise", "Writings"}},

            {BibleCode.Prophecy, new List<string>{"Prophecy", "Prophets"}},
            {BibleCode.MajorProphecy, new List<string>{"Major Prophecy", "Major Prophets"}},
            {BibleCode.MinorProphecy, new List<string>{"Minor Prophecy", "Minor Prophets"}},

            { BibleCode.NewTestament, new List<string>{"New Testament", "New Testaments"}},
         
            {BibleCode.Gospels, new List<string>{"Gospels"}},
            {BibleCode.Biographies, new List<string>{"Biographies", "Canonical Gospels"}},
            {BibleCode.Apostolic, new List<string>{"Apostolic History", "Apostolic"}},

            {BibleCode.Epistles, new List<string>{"Epistles", "Law", "Torah", "Moses", "Five Books of Moses"}},
            {BibleCode.PaulineEpistles, new List<string>{"Pauline Epistles"}},
            {BibleCode.GeneralEpistles, new List<string>{"General Epistles"}},
            {BibleCode.Apoclyptic, new List<string>{"Apoclyptic"}},

            { BibleCode.Scripture, new List<string>{"Bible", "Scripture"}}

        };
        #endregion
        #endregion

        public static Dictionary<Encoding, CulturedConfig> Configs = new Dictionary<Encoding, CulturedConfig>();

        static CulturedConfig()
        {
            CulturedConfig english = new CulturedConfig(Encoding.UTF8, false, EnglishNames, new List<string> { "The" }, new List<string> { "Introduction", "Author", "Brief" });
            CulturedConfig chinese = new CulturedConfig(Encoding.GetEncoding("gb2312"), true, ChineseNames, new List<string> { "第", "之" }, new List<string> { "简介", "作者",  "提要" });
        }

        #region Static functions
        public static bool TryGuess(string text, ref BibleCode code, ref Encoding encoding)
        {
            List<BibleCode> matched = new List<BibleCode>();

            foreach (var cfg in Configs)
            {
                foreach (KeyValuePair<BibleCode, List<string>> dict in cfg.Value.BookNames)
                {
                    foreach (string s in dict.Value)
                    {
                        if (text.Contains(s))
                        {
                            encoding = cfg.Key;
                            matched.Add(dict.Key);
                        }
                    }
                }

                if (matched.Count != 0)
                    break;
            }

            if (matched.Count == 0)
            {
                code = BibleCode.Unknown;
                return false;
            }

            code = matched.Max();
            return true;
        }

        private static string nameOf(string fullPath)
        {
            int index = fullPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;

            int last = fullPath.IndexOf('.', index);

            if (last == -1)
                return fullPath.Substring(index, fullPath.Length - index);
            else
                return fullPath.Substring(index, last - index);
        }

        public static Work FromFolder(string path)
        {
            string name = nameOf(path);

            Work books = new Work(name);

            CulturedConfig culture = books.Culture;

            Importer importer = new Importer(books, culture);

            DirectoryInfo directory = new DirectoryInfo(path);

            Book book = null;

            List<string> bookNames = (from dir in directory.GetDirectories()
                                      from f in dir.GetFiles("*.txt")
                                      select f.FullName).ToList();

            SortedDictionary<BibleCode, string> fileCodes = culture.CodesOf(bookNames);

            List<BibleCode> codes = fileCodes.Keys.ToList();

            //string sss = null;
            foreach (var fileCode in codes)
            {
                book = importer.Load(fileCode, fileCodes[fileCode]);
                //sss = book.Text;
            }

            books.EndLoading();
            //sss = book.Text;
            return books;
        }

        #endregion

        #region Property
        public Encoding TheEncoding { get; private set; }

        public NumberWordConverter NumberParser { get; private set; }

        public bool CharAsString { get; private set; }

        public Dictionary<BibleCode, List<string>> BookNames { get; private set; }

        public Dictionary<string, BibleCode> ReverseNames { get; private set; }

        public List<string> Prepositions { get; private set; }

        public List<string> Reserved { get; private set; }

        public List<string> Keywords { get; private set; }

        public Dictionary<BibleCode, LineTemplate> Templates { get; private set; }

        public LineTemplate this[BibleCode code]
        {
            get
            {
                return (Templates.ContainsKey(code)) ? Templates[code] : null;
            }
        }

        #endregion

        #region Constructor
        private CulturedConfig(Encoding encoding, bool charAsString, Dictionary<BibleCode, List<string>> bookNames, List<string> prepositions, List<string> reserved)
        {
            if (Configs.ContainsKey(encoding))
                throw new Exception();

            TheEncoding = encoding;
            NumberParser = NumberWordConverter.ConverterOf(encoding);
            CharAsString = charAsString;
            BookNames = bookNames;
            Prepositions = prepositions;
            Reserved = reserved;
            ReverseNames = new Dictionary<string, BibleCode>();
            Keywords = new List<string>();
            Templates = new Dictionary<BibleCode, LineTemplate>();

            foreach (var kvp in BookNames)
            {
                foreach (string s in kvp.Value)
                {
                    string str = s.Trim(DefaultWhiteSpaces);
                    if (!ReverseNames.ContainsKey(str))
                    {
                        ReverseNames.Add(str, kvp.Key);
                        Keywords.Add(str);
                    }
                }
            }

            Configs.Add(TheEncoding, this);
        }
        #endregion

        #region Function

        public SortedDictionary<BibleCode, string> CodesOf(List<string> original)
        {
            SortedDictionary<BibleCode, string> result = new SortedDictionary<BibleCode, string>();

            string name = null;
            foreach (string s in original)
            {
                name = nameOf(s);

                if (ReverseNames.ContainsKey(name))
                {
                    result.Add(ReverseNames[name], s);
                }
                else
                {
                    var matched = (from key in ReverseNames.Keys
                                   where name.Contains(key)
                                   orderby key.Length descending
                                   select ReverseNames[key]).ToArray();

                    if (matched.Count() == 0)
                        throw new Exception("Failed to construe " + name);
                    else
                    {
                        result.Add(matched[0], s);
                    }
                }
            }

            return result;
        }

        public bool IsBookName(string line, BibleCode book, ref string bookName)
        {
            if (book < BibleCode.Genesis || book > BibleCode.Revelation)
                throw new Exception();

            var names = (from s in BookNames[book]
                         where line.ToLower().Contains(s)
                         orderby s.Length descending
                         select s).ToList();
            
            if (names.Count == 0)
                return false;
            else
            {
                bookName = names.First();
                return true;
            }
        }

        public string NameOf(BibleCode code)
        {
            if (code < BibleCode.Genesis || code > BibleCode.Revelation)
                throw new Exception();

            return BookNames[code][0];
        }

        public string AbbreviationOf(BibleCode code)
        {
            if (code < BibleCode.Genesis || code > BibleCode.Revelation)
                throw new Exception();

            var shortest = (from s in BookNames[code]
                            orderby s.Length
                            select s).First();
            return shortest;
        }

        public bool IsContentIndicator(string line)
        {
            foreach (String key in Keywords)
            {
                if (line.StartsWith(key))
                    return true;
            }
            return false;
        }

        public bool TryParseBookContent(string line, ref BibleCode result, ref string text, ref int number)
        {
            if (tryParseVerse(line, BibleCode.Verses, ref text, ref number))
            {
                result = BibleCode.Verses;
                return true;
            }
            else if (tryParseChapter(line, BibleCode.Chapters, ref result, ref number))
            {
                result = BibleCode.Chapters;
                return true;
            }
            else if (tryParseParagraph(line, BibleCode.Paragraphs, ref text, ref number))
            {
                result = BibleCode.Paragraphs;
                return true;
            }
            else
            {
                result = BibleCode.Unknown;
                return false;
            }
        }

        //public bool TryParseVerseIndex(string trimed, BibleCode code, ref LineTemplate template, ref int number)
        //{
        //    if (!Templates.ContainsKey(BibleCode.Verses))
        //        throw new Exception("The Verse Template is expected here!");
        //    else if (this[BibleCode.Verses][0] != LineItemType.BookIndicator)
        //        throw new Exception("The verse may not has book indicator at the head!");

        //    string[] sections = trimed.Split(DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);


        //}

        private bool tryParseParagraph(string line, BibleCode likely, ref string text, ref int number)
        {
            if (likely != BibleCode.Paragraphs)
                throw new Exception();
            if (Templates.ContainsKey(BibleCode.Paragraphs))
            {
                return Templates[BibleCode.Paragraphs].IsCompliant(line, ref text);
            }
            else
            {
                string[] sections = line.Split(DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);

                if (int.TryParse(sections[0], out number))
                {
                    return false;
                }

                string title = line.Trim();
                int pos = line.IndexOf(title);
                string preposition = line.Substring(0, pos);

                LineTemplate template = new LineTemplate(BibleCode.Paragraphs, this, line, "", preposition, "");
                Templates.Add(likely, template);
                return template.IsCompliant(line, ref text);
            }
        }

        private bool tryParseVerse(string line, BibleCode likely, ref string text, ref int number)
        {
            if (likely != BibleCode.Verses)
                throw new Exception();

            if (Templates.ContainsKey(BibleCode.Verses))
            {
                return Templates[BibleCode.Verses].IsCompliant(line, ref text, ref number);
            }
            else
            {
                string[] sections = line.Split(DefaultVerseSplitter, StringSplitOptions.RemoveEmptyEntries);

                int count = sections.Count();

                if (count == 1)
                    return false;
                else if (int.TryParse(sections[count - 1], out number))
                    return false;
                else if (int.TryParse(sections[count-2], out number))
                {
                    LineTemplate template = new LineTemplate(BibleCode.Verses, this, line, sections[count-2]);
                    Templates.Add(BibleCode.Verses, template);

                    int indicatorIndex =template.Sequences.IndexOf(LineItemType.BookIndicator);
                    if (indicatorIndex != -1)
                    {
                        LineTemplate bookIndicator = new LineTemplate(BibleCode.BookIndicator, this, sections[indicatorIndex]);
                        Templates.Add(BibleCode.BookIndicator, bookIndicator);
                    }
                    else if ((indicatorIndex = template.Sequences.IndexOf(LineItemType.Leading))!= -1)
                    {
                        string leading = line.Substring(0, line.LastIndexOf(template.Splitter) - 1);
                        LineTemplate bookIndicator = new LineTemplate(BibleCode.BookIndicator, this, leading);
                        Templates.Add(BibleCode.BookIndicator, bookIndicator);
                    }

                    return template.IsCompliant(line, ref text, ref number);
                }
                else
                    return false;
            }
        }

        private bool tryParseChapter(string line, BibleCode likely, ref BibleCode result, ref int number)
        {
            if (Templates.ContainsKey(BibleCode.Chapters))
            {
                if (Templates[BibleCode.Chapters].IsCompliant(line, out number))
                {
                    result = BibleCode.Chapters;
                    return true;
                }
                else
                    return false;
            }
            else if (NumberParser.TryParse(line, out number))
            {
                string preposition = Prepositions.Find(x => ContainsKey(line, x));
                if (preposition != null)
                {
                    string indicator = BookNames[BibleCode.Chapters].First(x => ContainsKey(line, x));
                    string numberString = NumberParser.NumberStringOf(line);

                    LineTemplate template = new LineTemplate(BibleCode.Chapters, this, line, numberString, preposition, indicator);
                    Templates.Add(BibleCode.Chapters, template);
                    result = BibleCode.Chapters;
                    return true;
                }
                else
                {
                    string numberString = NumberParser.NumberStringOf(line);

                    LineTemplate template = new LineTemplate(BibleCode.Chapters, this, line, numberString, preposition, null);
                    Templates.Add(BibleCode.Chapters, template);
                    result = BibleCode.Chapters;
                    return true;                    
                }
            }
            else
            {
                return false;
            }
        }

        private bool containsName(string text, List<string> keys)
        {
            if (CharAsString)
            {
                foreach (string key in keys)
                {
                    if (text.Contains(key))
                        return true;
                }
            }
            else
            {
                string lowText = text.ToLower();
                foreach (string key in keys)
                {
                    if (lowText.Contains(key.ToLower()))
                        return true;
                }
            }

            return false;
        }

        public bool ContainsKey(string text, string key)
        {
            if (CharAsString)
            {
                return text.Contains(key);
            }
            else
            {
                return text.ToLower().Contains(key.ToLower());
            }
        }

        #region Codes obsoleted since Dec 07, 2011
        /*/
        public bool TryParse(string trimed, BibleCode book, ref BibleCode result, ref string buffer)
        {
            int number = -1;
            switch (book)
            {
                case BibleCode.OldTestament:
                case BibleCode.NewTestament:
                case BibleCode.Scripture:
                    foreach (KeyValuePair<BibleCode, List<string>> dict in BookNames)
                    {
                        if (dict.Key < BibleCode.OldTestament)
                            continue;

                        foreach (string line in dict.Value)
                        {
                            if (trimed.Contains(line))
                            {
                                result = book;
                                buffer = line;
                                return true;
                            }
                        }
                    }
                    return false;
                case BibleCode.Books:
                    return tryParseBook(trimed, book, ref result);
                case BibleCode.Chapters:
                    return tryParseChapter(trimed, book, ref result, ref number);
                case BibleCode.Paragraphs:
                    return tryParseParagraph(trimed, book, ref buffer, ref number);
                case BibleCode.Verses:
                    return tryParseVerse(trimed, book, ref buffer, ref number);
                default:
                    if (ContainsKey(trimed, BookNames[book]))
                    {
                        result = book;
                        return true;
                    };
                    return false;
            }

            if (book >= BibleCode.OldTestament)
            {
                foreach (KeyValuePair<BibleCode, List<string>> dict in  BookNames)
                {
                    if (dict.Key < BibleCode.OldTestament)
                        continue;

                    foreach (string line in dict.Value)
                    {
                        if (trimed.Contains(line))
                        {
                            book = book;
                            trimed = line;
                            return true;
                        }
                    }
                }

                return false;
            }
            else if (book == BibleCode.Books)
            {
                for (BibleCode code = BibleCode.Genesis; code <= BibleCode.Revelation; code ++ )
                {
                    if(ContainsKey(trimed, BookNames[code]))
                    {
                        book = code;
                        return true;
                    }
                }

                return false;
            }
            else if (book >= BibleCode.Genesis && book <= BibleCode.Revelation)
            {
                if (ContainsKey(trimed, BookNames[book]))
                {
                    book = book;
                    return true;
                };
                return false;
            }
            else if (book == BibleCode.Chapters)
            {
                int number = -1;
                if ( isChapter(trimed, out number))
                {
                    book = book;
                    return true;
                }
            }
            else if (book == BibleCode.Paragraphs)
            {
                int number = -1;
                if (Templates.ContainsKey(BibleCode.Paragraphs))
                {
                    return Templates[BibleCode.Paragraphs].IsCompliant(trimed, ref trimed);
                }
                else
                {
                    if (NumberParser.TryGuess(trimed, out number))
                    {
                        return false;
                    }

                    string title = trimed.Trim();
                    int pos = trimed.IndexOf(title);
                    string preposition = trimed.Substring(0, pos);

                    LineTemplate template = new LineTemplate(BibleCode.Paragraphs, trimed, NumberParser, "", preposition, "");
                    Templates.Add(BibleCode.Chapters, template);
                    return true;                    
                }
            }
            else if (book == BibleCode.Verses)
            {
                int number = -1;
                if (Templates.ContainsKey(BibleCode.Verses))
                {
                    return Templates[BibleCode.Verses].IsCompliant(trimed, ref number, ref trimed);
                }
                else
                {
                    string[] sections = trimed.Split(DefaultWhiteSpaces);

                    for (int ch = sections.Count(); ch >= 0; ch ++ )
                    {
                        if (int.TryGuess(sections[ch], out number))
                        {
                            LineTemplate template = new LineTemplate(BibleCode.Verses, trimed, sections[ch]);
                            Templates.Add(BibleCode.Verses, template);
                            return true;
                        }
                    }
                }

            }

            return false;
            #endregion
        }

        private bool tryParseBookTitle(string trimed, BibleCode book)
        {
            if (book < BibleCode.Genesis || book > BibleCode.Revelation)
                throw new Exception();

            if (Templates.ContainsKey(BibleCode.Books))
            {
                return Templates[BibleCode.Books].IsCompliant(trimed, book);
            }
            else
            {
                if (!ContainsKey(trimed, BookNames[book]))
                    return false;

                LineTemplate template = new LineTemplate(BibleCode.Books, this, trimed, book);
                Templates.Add(BibleCode.Books, template);

                return template.IsCompliant(trimed, book);
            }
        }

        private bool tryParseBook(string trimed, BibleCode book, ref BibleCode result)
        {
            if (book != BibleCode.Books)
                throw new Exception();

            if (ReverseNames.ContainsKey(trimed))
            {
                result = ReverseNames[trimed];
                return true;
            }
            else
            {
                var matched = (from line in ReverseNames.Keys
                               where trimed.Contains(line)
                               orderby line.ContentLength descending
                               select line).ToArray();

                if (matched.Count() == 0)
                    return false;
                else
                {
                    result = ReverseNames[matched[0]];
                    return true;
                }
            }
        }

        //*/
        #endregion

        #region Codes obsoleted on Dec 06, 2011
        /*/
        private bool isChapter(string trimed, out int number)
        {
            if (Templates.ContainsKey(BibleCode.Chapters))
            {
                return Templates[BibleCode.Chapters].IsCompliant(trimed, out number);
            }
            else if (NumberParser.TryParse(trimed, out number))
            {
                string preposition = Prepositions.Find(x => ContainsKey(trimed, x));
                string indicator = BookNames[BibleCode.Chapters].First(x => ContainsKey(trimed, x));
                string numberString = NumberParser.NumberStringOf(trimed);

                LineTemplate template = new LineTemplate(BibleCode.Chapters, trimed, NumberParser, numberString, preposition, indicator);
                Templates.Add(BibleCode.Chapters, template);
                return true;
            }
            else
            {
                number = -1;
                return false;
            }
        }
        //*/
        #endregion

        #endregion
    }
}
