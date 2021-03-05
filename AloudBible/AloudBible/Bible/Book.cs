using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.GenericArray;
using System.Runtime.Serialization;
using System.Data.SQLite;

namespace AloudBible.Bible
{
    [DataContract]
    public sealed class Book : Holder, IEnumerable<Chapter>, IComparable<Book>
    {
        #region Definitions

        public static Dictionary<BibleCode, Scope> Classifications = new Dictionary<BibleCode, Scope>
        {
            {BibleCode.OldTestament, new Scope(1, 39)},
            {BibleCode.Pentateuch, new Scope(1, 5)},
            {BibleCode.OldTimeHistory, new Scope(6, 12)},
            {BibleCode.Wisdom, new Scope(18, 5)},

            {BibleCode.Prophecy, new Scope(23, 17)},
            {BibleCode.MajorProphecy, new Scope(23, 5)},
            {BibleCode.MinorProphecy, new Scope(28, 12)},

            {BibleCode.NewTestament, new Scope(40, 27)},
            {BibleCode.Gospels, new Scope(40, 5)},
            {BibleCode.Biographies, new Scope(40, 4)},
            {BibleCode.Apostolic, new Scope(44, 1)},

            {BibleCode.Epistles, new Scope(45, 22)},
            {BibleCode.PaulineEpistles, new Scope(45, 14)},
            {BibleCode.GeneralEpistles, new Scope(59, 7)},
            {BibleCode.Apoclyptic, new Scope(66, 1)}
        };

        #endregion

        #region Property
        public Work TheCollection { get { return parent as Work; } }

        //public Scope ScopeInCollection { get { throw new NotImplementedException(); } }

        public override ComponentType ItemType
        {
            get { return ComponentType.Book; }
        }

        private BibleCode code = BibleCode.Unknown;
        //public override int Number
        //{
        //    get
        //    {
        //        return (int)code;
        //    }
        //    private set { code = value; }
        //}
        [DataMember(Name = "book", Order = 0)]
        public  int Number { get; set; }

        private List<BibleCode> folders = null;
        public List<BibleCode> Folders 
        {
            get
            {
                if (folders == null)
                {
                    folders = new List<BibleCode>();
                    foreach (var kvp in Classifications)
                    {
                        if (kvp.Value.Covers(Number))
                            folders.Add(kvp.Key);
                    }
                }

                return folders;
            }
        }

        public string Name { get; set; }

        [DataMember(Name = "title", Order = 1)]
        public string Abbreviation { get; private set; }

        public string Introduction { get; private set; }

        public string Author { get; private set; }

        public string Summary { get; private set; }

        [DataMember(Name = "chapters", Order = 2)]
        public List<Chapter> Chapters { get; set; }

        public Chapter this[int index]
        {
            get { return Chapters[index - 1]; }
        }

        public override IEnumerable<BookItem> Siblings
        {
            get
            {
                foreach (Chapter chapter in Chapters)
                {
                    yield return chapter;
                }
            }
        }

        public int ChapterCount
        {
            get { return Chapters.Count; }
        }
        
        private int sectionCount = 0;
        public int SectionCount
        {
            get
            {
                if (sectionCount < ChapterCount)
                {
                    sectionCount = 0;
                    foreach (Chapter chapter in Chapters)
                    {
                        sectionCount += chapter.SectionCount;
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
                    foreach (Chapter chapter in Chapters)
                    {
                        verseCount += chapter.VerseCount;
                    }
                }
                return verseCount;
            }
        }
        #endregion

        #region Constructor
        public Book(Work parent, BibleCode code, int offset)
            : base(parent, offset)
        {
            this.code = code;
            Abbreviation = Culture.AbbreviationOf(code);
            Name = Culture.NameOf(code);
            Chapters = new List<Chapter>();
            this.Number = (parent == null) ? 0 : parent.Siblings.Count() + 1;
        }
        #endregion

        #region Function

        public override void EndLoading()
        {
            if (Chapters.Count != 0)
                Chapters.Last().EndLoading();

            length = TheCollection.ContentLength - offset;
        }

        public Chapter AddChapter(int number)
        {
            if (number != Chapters.Count + 1)
                throw new Exception("The sequence of " + this.ToString() + " may be wrong!");

            if (Chapters.Count != 0)
                Chapters.Last().EndLoading();

            Chapter newChapter = new Chapter(this, number, TheCollection.ContentLength);
            Chapters.Add(newChapter);
            return newChapter;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i= 0; i< Folders.Count; i ++)
            {
                BibleCode code = Folders[i];
                sb.Append(Culture.BookNames[code][0] + "-");
            }

            sb.Append(TheCollection.Culture.BookNames[this.code][0]);

            return sb.ToString();
        }

        public override int IndexOf(BookItem sibling)
        {
            int result = -1;
            if (sibling is Chapter)
            {
                Chapter chapter = sibling as Chapter;
                if (chapter.Parent != this)
                    return -1;
                else
                    result = Chapters.IndexOf(chapter);
            }
            else
                throw new NotImplementedException();

            return result == -1 ? result : result + 1;
        }

        #region IEnumerable<Chapter> 

        public IEnumerator<Chapter> GetEnumerator()
        {
            foreach (Chapter chapter in Chapters)
            {
                yield return chapter;
            }
        }

        #endregion

        #region IEnumerable 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IComparable<Book> 成员

        public int CompareTo(Book other)
        {
            return this.code.CompareTo(other.code);
        }

        #endregion

        internal void toSQLite(SQLiteCommand command, Counters counters, Encoding encoding)
        {
            using (SQLiteTransaction transaction = command.Connection.BeginTransaction())
            {
                command.Transaction = transaction;
                Dictionary<String, String> parameters = new Dictionary<String, String>();
                int bytesFirst = counters.ByteOffset;
                int charsFirst = counters.CharOffset;

                parameters.Add(Work.KEY_WORK_CODE, "NCV");
                parameters.Add(Work.KEY_BOOK, Number.ToString());
                parameters.Add(Work.KEY_TITLE, Name);
                parameters.Add(Work.KEY_ABBREVIATION, Abbreviation);
                parameters.Add(Work.KEY_METADATA, "");
                parameters.Add(Work.KEY_CHAPTERS_OFFSET, counters.ChaptersOffset.ToString());
                parameters.Add(Work.KEY_CHAPTER_COUNT, ChapterCount.ToString());
                parameters.Add(Work.KEY_SECTION_COUNT, SectionCount.ToString());
                parameters.Add(Work.KEY_VERSE_COUNT, VerseCount.ToString());
                parameters.Add(Work.KEY_BYTE_START, counters.ByteOffset.ToString());
                parameters.Add(Work.KEY_BYTE_LENGTH, "0");
                parameters.Add(Work.KEY_CHAR_START, counters.CharOffset.ToString());
                parameters.Add(Work.KEY_CHAR_LENGTH, "0");

                String bookInsertion = SQLiteDatabase.InsertCommandTextOf(Work.TABLE_BOOKS, parameters);
                command.CommandText = bookInsertion;
                counters.BooksOffset = SQLiteDatabase.Insert(command);

                foreach (Chapter chapter in Chapters)
                {
                    chapter.toSQLite(command, counters, encoding);
                }

                int bytesLength = counters.ByteOffset - bytesFirst;
                int charsLength = counters.CharOffset - charsFirst;
                parameters.Clear();
                parameters.Add(Work.KEY_BYTE_LENGTH, bytesLength.ToString());
                parameters.Add(Work.KEY_CHAR_LENGTH, charsLength.ToString());
                String bookUpdate = SQLiteDatabase.UpdateCommandTextOf(Work.TABLE_BOOKS, parameters, String.Format("{0}='{1}'", Work.KEY_ID, counters.BooksOffset));
                command.CommandText = bookUpdate;
                command.ExecuteNonQuery();
                counters.BooksOffset++;
                transaction.Commit();
            }

            Console.WriteLine(this.ToString() + " has been inserted into the SQLite.");
        }
        #endregion

    }
}
