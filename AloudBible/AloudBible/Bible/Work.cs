using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.GenericArray;
using System.Runtime.Serialization;
using System.Data.SQLite;
using System.IO;

namespace AloudBible.Bible
{
    public enum BibleCode
    {
        Unknown = -1,

        //Written by Moses              
        Genesis = 1,            //1. Genesis 
        Exodus,              //2. Exodus 
        Leviticus,              //3. Leviticus 
        Numbers,              //4. Numbers 
        Deuteronomy,              //5. Deuteronomy 

        //OT Narratives
        Joshua,              //6. Joshua 
        Judges,              //7. Judges
        Ruth,              //8. Ruth
        Samuel1,              //9. 1 Samuel
        Samuel2,              //10. 2 Samuel
        Kings1,              //11. 1 Kings
        Kings2,              //12. 2 Kings
        Chronicles1,              //13. 1 Chronicles
        Chronicles2,              //14. 2 Chronicles
        Ezra,              //15. Ezra
        Nehemiah,              //16. Nehemiah
        Esther,              //17. Esther

        //Wisdom Literature
        Job,              //18. Job
        Psalms,              //19. Psalms
        Proverbs,              //20. Proverbs
        Ecclesiastes,              //21. Ecclesiastes
        Songs,              //22. Song of Songs

        //Major Prophets
        Isaiah,              //23. Isaiah
        Jeremiah,              //24. Jeremiah
        Lamentations,              //25. Lamentations
        Ezekiel,              //26. Ezekiel
        Daniel,              //27. Daniel

        //Minor Prophets
        Hosea,              //28. Hosea 
        Joel,              //29. Joel
        Amos,              //30. Amos
        Obadiah,              //31. Obadiah
        Jonah,              //32. Jonah
        Micah,              //33. Micah
        Nahum,              //34. Nahum
        Habakkuk,              //35. Habakkuk
        Zephaniah,              //36. Zephaniah
        Haggai,              //37. Haggai
        Zechariah,              //38. Zechariah
        Malachi,              //39. Malachi 

        //NT Narratives
        Matthew,              //40. Matthew
        Mark,              //41. Mark
        Luke,              //42. Luke
        John,              //43. John
        Acts,              //44. Acts

        //Epistles by Paul 
        Romans,              //45. Romans 
        Corinthians1,              //46. 1 Corinthians 
        Corinthians2,              //47. 2 Corinthians 
        Galatians,              //48. Galatians 
        Ephesians,              //49. Ephesians 
        Philippians,              //50. Philippians 
        Colossians,              //51. Colossians 
        Thessalonians1,              //52. 1 Thessalonians
        Thessalonians2,              //53. 2 Thessalonians 
        Timothy1,              //54. 1 Timothy 
        Timothy2,              //55. 2 Timothy 
        Titus,              //56. Titus 
        Philemon,              //57. Philemon

        //General Epistles 
        Hebrews,              //58. Hebrews 
        James,              //59. James 
        Peter1,              //60. 1 Peter 
        Peter2,              //61. 2 Peter 
        John1,              //62. 1 John 
        John2,              //63. 2 John 
        John3,              //64. 3 John 
        Jude,              //65. Jude

        //Apocalyptic Epistle by John
        Revelation,              //66. Revelation

        //Other keyword identities
        Verses = 100,
        BookIndicator = 300,
        Paragraphs = 500,
        Chapters = 700,
        Books = 900,

        OldTestament = 1000,     //Old Testament, 1 - 39

        Pentateuch = 1100,      // books 1 - 5
        OldTimeHistory = 1300,      // books 6 - 17
        Wisdom = 1500,          // books 18 - 22
        Prophecy = 1700,        // books 23 - 39
        MajorProphecy = 1710,   // books 23 - 27
        MinorProphecy = 1760,   // books 28 - 39

        NewTestament = 2000,    // Books 40 - 66

        Gospels = 2100,         // Books 40 - 44
        Biographies = 2110,     // Books 40 - 43
        Apostolic = 2180,       // Book 44

        Epistles = 2500,        // Books 45 - 66
        PaulineEpistles = 2510,        // Books 45 - 58
        GeneralEpistles = 2550,         // Books 59 - 65
        Apoclyptic = 2590,      // Book 66

        Scripture = 10000
    }

    [DataContract]
    public class Work : Holder, IEnumerable<Book>
    {
        #region Consts Definitions
        public const String TABLE_WORKS = "works";
	    public const String TABLE_BOOKS = "books";
	    public const String TABLE_CHAPTERS = "chapters";
	    public const String TABLE_SECTIONS = "sections";
	    public const String TABLE_VERSES = "verses";
	
	    //*/ Views to facilitate accessing items
	    public const String VIEW_BOOKS = "viewBooks";
	    public const String VIEW_CHAPTERS = "viewChapters";
	    public const String VIEW_SECTIONS = "viewSections";
	    public const String VIEW_VERSES = "viewVerses";
	    //*/
	
	    // Columns within tables
	    public const String KEY_ID = "_id";
	    // public const String KEY_ORDINAL = "ordinal";
	    // public const String KEY_CONTAINER = "container";
	    public const String KEY_TITLE = "title";
        public const String KEY_ABBREVIATION = "abbreviation";
	    public const String KEY_CODE = "code";
	    public const String KEY_DESCRIPTION = "description";
	    public const String KEY_LOCALE = "locale";
	    public const String KEY_CHARSET = "charset";	
	    public const String KEY_CONTENT = "content";
	    public const String KEY_METADATA = "metaData";
	    public const String KEY_VERSION = "version";
	    public const String KEY_BOOKS_OFFSET = "booksOffset";
	    public const String KEY_CHAPTERS_OFFSET = "chaptersOffset";
	    public const String KEY_SECTIONS_OFFSET = "sectionsOffset";
	    public const String KEY_VERSES_OFFSET	= "versesOffset";
	
	    public const String KEY_WORK = "work";
	    public const String KEY_BOOK = "book";
	    public const String KEY_CHAPTER = "chapter";
	    public const String KEY_SECTION = "section";
	    public const String KEY_VERSE = "verse";
	
	    public const String KEY_BYTE_START = "bStart";
	    public const String KEY_BYTE_LENGTH = "bLength";
	    public const String KEY_CHAR_START = "cStart";
	    public const String KEY_CHAR_LENGTH = "length";
	
	    // column names refer to the parent containers
	    public const String KEY_WORK_CODE	= "workCode";
	    public const String KEY_BOOK_ID = "bookId";
	    public const String KEY_CHAPTER_ID = "chapterId";
	    public const String KEY_SECTION_ID = "sectionId";
	
	    // column names refer to the first sibling item for performance optimization ?
	    public const String KEY_BOOK_COUNT = "bookCount";
	    public const String KEY_CHAPTER_COUNT = "chapterCount";
	    public const String KEY_SECTION_COUNT = "sectionCount";
	    public const String KEY_VERSE_COUNT = "verseCount";
        #endregion

        #region Property
        public override ComponentType ItemType
        {
            get { return ComponentType.Collection; }
        }
        //public int Number { get; set; }

        private CulturedConfig theCulture = null;
        public override CulturedConfig Culture { get { return theCulture; } }

        public override Holder Parent { get { return null; } }

        //public override int Number
        //{
        //    get { return (int)BibleCode.Scripture; }
        //}

        //private string buffer = "";
        private StringBuilder sb = null;

        public StringBuilder SBuilder { get { return sb; } }

        //private char[] content = null;
        //public override char[] Content { get { return content; } }

        [DataMemberAttribute(Name="version", Order=0)]
        public string Version { get; private set; }

        [DataMember(Name="length", Order=1)]
        public int ContentLength { get { return sb.Length; } private set { ;} }

        // public override int PageOffset { get { return  0;} }

        [DataMemberAttribute(Name="books", Order=2)]
        public List<Book> Books { get; set; }

        public override IEnumerable<BookItem> Siblings
        {
            get
            {
                foreach (Book book in Books)
                {
                    yield return book;
                }
            }
        }

        public Book this[BibleCode code]
        {
           get 
           {
               if (code < BibleCode.Genesis || code > BibleCode.Revelation)
                   throw new ArgumentOutOfRangeException(code.ToString());

               int num = (int)code;

               foreach (Book bk in Books)
               {
                   if (bk.Number == num)
                       return bk;
               }
               return null; 
           }
        }

        public int BookCount
        {
            get { return Books.Count; }
        }

        private int chapterCount = 0;
        public int ChapterCount
        {
            get
            {
                if (chapterCount <= BookCount)
                {
                    chapterCount = 0;
                    foreach (Book book in Books)
                    {
                        chapterCount += book.ChapterCount;
                    }
                }
                return chapterCount;
            }
        }

        private int sectionCount = 0;
        public int SectionCount
        {
            get
            {
                if (sectionCount < ChapterCount)
                {
                    sectionCount = 0;
                    foreach (Book book in Books)
                    {
                        sectionCount += book.SectionCount;
                    }
                }
                return sectionCount;
            }
        }

        private int verseCount = 0;
        public int VerseCount
        {
            get
            {
                if (verseCount < ChapterCount)
                {
                    verseCount = 0;
                    foreach (Book book in Books)
                    {
                        verseCount += book.VerseCount;
                    }
                }
                return verseCount;
            }
        }
        #endregion

        #region Constructor
        public Work(string name)
        {
            Books = new List<Book>();
            //buffer = "";
            sb = new StringBuilder();
            BibleCode code = BibleCode.Unknown;
            offset = 0;
            Encoding theEncoding = null;
            CulturedConfig.TryGuess(name, ref code, ref theEncoding);

            if (code != BibleCode.Scripture)
            {
                //throw new Exception();
                theEncoding = Encoding.GetEncoding("gb2312");
                theCulture = CulturedConfig.Configs[theEncoding];
                Version = name;
            }
            else
            {
                theCulture = CulturedConfig.Configs.ContainsKey(theEncoding) ? CulturedConfig.Configs[theEncoding] : CulturedConfig.Configs[Encoding.UTF8];

                foreach (string s in Culture.BookNames[BibleCode.Scripture])
                {
                    if (name.Contains(s))
                    {
                        Version = name.Replace(s, "");
                        break;
                    }
                }
            }

        }

        public Work(string version, Encoding encoding) : base(null, 0)
        {
            Books = new List<Book>();
            theCulture = CulturedConfig.Configs.ContainsKey(encoding) ? CulturedConfig.Configs[encoding] : CulturedConfig.Configs[Encoding.UTF8];
            Version = version;
        }
        #endregion

        #region Function

        public void AddContent(string text)
        {
            //buffer += text;
            sb.Append(text);
        }

        //public override string TextOf(Scope scope)
        //{
        //    if (content == null)
        //        return sb.ToString().Substring(scope.First, scope.Length);
        //    else
        //        return TextScope.TextOf(content, scope);
        //    //return content == null ? buffer.Substring(scope.First, scope.Length) : base.TextOf(scope);
        //}

        public override string TextOf(int start, int length)
        {
            //if (content == null)
                return sb.ToString().Substring(start, length);
            //else
            //    return TextScope.TextOf(content, start, length);
            //return content == null ? buffer.Substring(scope.First, scope.Length) : base.TextOf(scope);
        }

        public Book AddBook(BibleCode code)
        {
            if (this[code] != null)
                throw new Exception();

            if (Books.Count != 0)
                Books.Last().EndLoading();

            Book newBook = new Book(this, code, sb.Length);
            Books.Add(newBook);

            return newBook;
        }

        public override void EndLoading()
        {
            if (Books.Count != 0)
                Books.Last().EndLoading();

            //content = sb.ToString().ToCharArray();
            //sb = null;
        }

        public override string ToString()
        {
            return BibleCode.Scripture.ToString();
        }

        public override int IndexOf(BookItem sibling)
        {
            int result = -1;
            if (sibling is Book)
            {
                Book book = sibling as Book;
                if (book.Parent != this)
                    return -1;
                else
                    result = Books.IndexOf(book);
            }
            else
                throw new NotImplementedException();

            return result == -1 ? result : result + 1;
        }

        #region IEnumerable<Book> 

        public IEnumerator<Book> GetEnumerator()
        {
           foreach (Book book in Books)
           {
               yield return book;
           }
        }

        #endregion

        #region IEnumerable 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
           return GetEnumerator();
        }

        #endregion

        public void toSQLite(string dbName, Encoding encoding)
        {
            SQLiteConnection connection = null;
            SQLiteTransaction transaction = null;
            SQLiteCommand command = null;
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            Counters counters = new Counters();

            try
            {
                StreamWriter sw = new StreamWriter("C:/temp/content.NCV", false, encoding);
                sw.Write(sb.ToString());
                sw.Close();

                connection = new SQLiteConnection("Data Source=" + dbName);

                connection.Open();

                //transaction = connection.BeginTransaction();
                command = connection.CreateCommand();
                //command.Transaction = transaction;

                parameters.Clear();
                parameters.Add(KEY_CODE, "NCV");
                parameters.Add(KEY_WORK, Version);
                parameters.Add(KEY_VERSION, "1");
                parameters.Add(KEY_CHARSET, encoding.EncodingName);
                parameters.Add(KEY_LOCALE, "zh");
                parameters.Add(KEY_CONTENT, "content.NCV");
                parameters.Add(KEY_DESCRIPTION, "");
                parameters.Add(KEY_METADATA, "");
                parameters.Add(KEY_BOOKS_OFFSET, counters.BooksOffset.ToString());
                parameters.Add(KEY_CHAPTERS_OFFSET, counters.ChaptersOffset.ToString());
                parameters.Add(KEY_SECTIONS_OFFSET, counters.SectionsOffset.ToString());
                parameters.Add(KEY_VERSES_OFFSET, counters.VersesOffset.ToString());
                parameters.Add(KEY_BOOK_COUNT, BookCount.ToString());
                parameters.Add(KEY_CHAPTER_COUNT, ChapterCount.ToString());
                parameters.Add(KEY_SECTION_COUNT, SectionCount.ToString());
                parameters.Add(KEY_VERSE_COUNT, VerseCount.ToString());
                parameters.Add(KEY_BYTE_LENGTH, "0");
                parameters.Add(KEY_CHAR_LENGTH, "0");

                String editonInsertion = SQLiteDatabase.InsertCommandTextOf(TABLE_WORKS, parameters);
                command.CommandText = editonInsertion;
                command.ExecuteNonQuery();

                foreach (Book book in Books)
                {
                    book.toSQLite(command, counters, encoding);
                }

                parameters.Clear();
                parameters.Add(KEY_BYTE_LENGTH, counters.ByteOffset.ToString());
                parameters.Add(KEY_CHAR_LENGTH, counters.CharOffset.ToString());
                String workUpdate = SQLiteDatabase.UpdateCommandTextOf(Work.TABLE_WORKS, parameters, String.Format("{0}='{1}'", Work.KEY_CODE, "NCV"));
                command.CommandText = workUpdate;
                SQLiteDatabase.Insert(command);

                //transaction.Commit();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();

                    }
                    catch (SQLiteException ex2)
                    {
                        Console.WriteLine("Transaction rollback failed.");
                        Console.WriteLine("Error: {0}", ex2.ToString());
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Closing connection failed.");
                        Console.WriteLine("Error: {0}", ex.ToString());
                    }
                    finally
                    {
                        connection.Dispose();
                    }
                }
            } 
        }
    }

#endregion
}
