using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.NumberParser;
using Utitilties.GenericArray;
using System.Runtime.Serialization;
using System.Data.SQLite;

namespace AloudBible.Bible
{
    [DataContract]
    [KnownType(typeof(Verse))]
    [KnownType(typeof(Section))]
    public sealed class Chapter : Holder, IEnumerable<Verse>, IComparable<Chapter>
    {
        #region Property
        public override ComponentType ItemType
        {
            get { return ComponentType.Chapter; }
        }

        [DataMemberAttribute(Name ="c", Order=0)]
        public int Number { get; set; }

        public Book TheBook { get { return parent as Book; } }

        [DataMemberAttribute(Name = "p", Order = 1)]
        public List<Section> Sections { get; set; }

        [DataMemberAttribute(Name="v", Order=2)]
        public List<Verse> Verses { get; set; }

        public Verse this[int index]
        {
            get { return Verses[index - 1]; }
        }

        public override IEnumerable<BookItem> Siblings
        {
            get 
            {
                foreach (Verse verse in Verses)
                {
                    yield return verse;
                }
            }
        }

        public int SectionCount { get { return Sections.Count; } }

        public int VerseCount { get { return Verses.Count; } }

        #endregion

        #region Constructor
        public Chapter(Book parent, int number, int offset)
            : base(parent, offset)
        {
            Verses = new List<Verse>();
            Sections = new List<Section>();
            this.Number = (parent == null) ? 0 : parent.Siblings.Count() + 1;
        }

        #endregion

        #region Function
        public override int IndexOf(BookItem sibling)
        {
            int result = -1;
            if (sibling is Verse)
            {
                Verse verse = sibling as Verse;
                if (verse.Parent != this)
                    return -1;
                else
                    result = Verses.IndexOf(verse);
            }
            else if (sibling is Section)
            {
                Section para = sibling as Section;
                if (para.Parent != this)
                    return -1;
                else
                    result = Sections.IndexOf(para);
            }
            else
                throw new NotImplementedException();

            return result == -1 ? result : result + 1;
        }

        public override void EndLoading()
        {
            int offset = TheBook.TheCollection.ContentLength;
            length = offset - TheScope.First;
            if (Sections.Count != 0)
            {
                Section lastSec = Sections.Last();
                lastSec.Length = offset - lastSec.First;
                lastSec.VerseCount = Verses.Count - lastSec.VerseFrom + 1;
            }
        }

        public Verse AddVerse(int number, string text)
        {
            Verse verse = null;
            if (number == Verses.Count + 1)
            {
                verse = new Verse(this, text, TheBook.TheCollection.ContentLength);
            }
            else if (number == Verses.Count + 2)
            {
                verse = new Verse(this, "", TheBook.TheCollection.ContentLength);
                //Console.WriteLine(String.Format("{0}-{1}", this.TheBook.Abbreviation, verse));
                Verses.Add(verse);
                verse = new Verse(this, text, TheBook.TheCollection.ContentLength);
            }
            else
                throw new Exception("Sequence unexpected of " + number.ToString() + " for verse: " + text);

            TheBook.TheCollection.AddContent(text);
            Verses.Add(verse);
            return verse;
        }

        public Section AddSection(string text)
        {
            //if (Paragraphs.Count != 0 && Paragraphs.Last().Last < Verses.Count + 1)
            //    EndSibling();

            // Section newParagraph = null;
            int offset = TheBook.TheCollection.ContentLength;

            if (Sections.Count == 0)
            {
                Section newParagraph = new Section(this, text, offset, this.Verses.Count+1);
                Sections.Add(newParagraph);
                return newParagraph;
            }
            else if(Sections.Last().First == offset)
            {
                Sections.Last().Title += "\r\n" + text;
                return Sections.Last();
            }
            else
            {
                Section lastSec = Sections.Last();
                lastSec.Length = offset - lastSec.First;
                lastSec.VerseCount = Verses.Count() + 1 - lastSec.VerseFrom;
                Section newParagraph = new Section(this, text, offset, this.Verses.Count+1);
                Sections.Add(newParagraph);
                return newParagraph;
            }

        }

        #region IEnumerable<Verse> 

        public IEnumerator<Verse> GetEnumerator()
        {
            foreach (Verse verse in Verses)
            {
                yield return verse;
            }
        }

        #endregion

        #region IEnumerable 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IComparable<Chapter> 成员

        public int CompareTo(Chapter other)
        {
            return this.Number.CompareTo(other.Number);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}-{1}", TheBook.Abbreviation, Number);
        }

        internal void toSQLite(SQLiteCommand command, Counters counters, Encoding encoding)
        {
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            int bytesFirst = counters.ByteOffset;
            int charsFirst = counters.CharOffset;
            int[] firstVerses = new int[SectionCount];

            parameters.Add(Work.KEY_BOOK_ID, counters.BooksOffset.ToString());
            parameters.Add(Work.KEY_CHAPTER, Number.ToString());
            parameters.Add(Work.KEY_SECTIONS_OFFSET, counters.SectionsOffset.ToString());
            parameters.Add(Work.KEY_VERSES_OFFSET, counters.VersesOffset.ToString());
            parameters.Add(Work.KEY_SECTION_COUNT, SectionCount.ToString());
            parameters.Add(Work.KEY_VERSE_COUNT, VerseCount.ToString());
            parameters.Add(Work.KEY_BYTE_START, counters.ByteOffset.ToString());
            parameters.Add(Work.KEY_BYTE_LENGTH, "0");
            parameters.Add(Work.KEY_CHAR_START, counters.CharOffset.ToString());
            parameters.Add(Work.KEY_CHAR_LENGTH, "0");

            String chapterInsertion = SQLiteDatabase.InsertCommandTextOf(Work.TABLE_CHAPTERS, parameters);
            command.CommandText = chapterInsertion;
            SQLiteDatabase.Insert(command);

            counters.ChaptersOffset++;

            Dictionary<String, String> verseParameters = new Dictionary<String, String>();
            verseParameters.Add(Work.KEY_CHAPTER_ID, counters.ChaptersOffset.ToString());
            verseParameters.Add(Work.KEY_VERSE, "0");
            verseParameters.Add(Work.KEY_BYTE_START, "0");
            verseParameters.Add(Work.KEY_BYTE_LENGTH, "0");
            verseParameters.Add(Work.KEY_CHAR_START, "0");
            verseParameters.Add(Work.KEY_CHAR_LENGTH, "0");

            Section section = (SectionCount == 0 ? null : Sections[0]);
            Verse verse = null;

            int byteLength = 0;
            String verseInsertText = "";
            for (int i =0, j = 0; i < Verses.Count; i ++)
            {
                verse = Verses[i];
                byteLength = encoding.GetByteCount(verse.Text);
                
                verseParameters[Work.KEY_VERSE] = verse.Number.ToString();
                verseParameters[Work.KEY_BYTE_START] = counters.ByteOffset.ToString();
                verseParameters[Work.KEY_BYTE_LENGTH] = byteLength.ToString();
                verseParameters[Work.KEY_CHAR_START] = counters.CharOffset.ToString();
                verseParameters[Work.KEY_CHAR_LENGTH] = verse.Length.ToString();

                verseInsertText = SQLiteDatabase.InsertCommandTextOf(Work.TABLE_VERSES, verseParameters);
                command.CommandText = verseInsertText;
                command.ExecuteNonQuery();
                if (section != null && verse.First == section.First)
                {
                    firstVerses[j] = counters.VersesOffset;
                    j++;

                    section = (j < SectionCount) ? Sections[j] : null;
                }

                counters.ByteOffset += byteLength;
                counters.CharOffset += verse.Length;
                counters.VersesOffset++;
            }

            Dictionary<String, String> sectionParameters = new Dictionary<String, String>();
            sectionParameters.Add(Work.KEY_CHAPTER_ID, counters.ChaptersOffset.ToString());
            sectionParameters.Add(Work.KEY_SECTION, "0");
            sectionParameters.Add(Work.KEY_TITLE, "");
            sectionParameters.Add(Work.KEY_VERSES_OFFSET, "0");
            sectionParameters.Add(Work.KEY_VERSE_COUNT, "0");
            String sectionInsertText = null;
            for (int j = 0; j < SectionCount; j++)
            {
                section = Sections[j];

                sectionParameters[Work.KEY_SECTION] = section.Number.ToString();
                sectionParameters[Work.KEY_TITLE] = section.Title;
                sectionParameters[Work.KEY_VERSES_OFFSET] = firstVerses[j].ToString();
                //sectionParameters[Work.KEY_VERSE_FIRST] = section.VerseFrom.ToString();
                sectionParameters[Work.KEY_VERSE_COUNT] = section.VerseCount.ToString();                
                //int count = ((j == SectionCount - 1) ? counters.VersesOffset - firstVerses[j] 
                //    : firstVerses[j + 1] - firstVerses[j]) + 1;
                //sectionParameters[Work.KEY_VERSE_COUNT] = count.ToString();

                sectionInsertText = SQLiteDatabase.InsertCommandTextOf(Work.TABLE_SECTIONS, sectionParameters);
                command.CommandText = sectionInsertText;
                command.ExecuteNonQuery();

                counters.SectionsOffset++;
            }

            int bytesLength = counters.ByteOffset - bytesFirst;
            int charsLength = counters.CharOffset - charsFirst;
            parameters.Clear();
            parameters.Add(Work.KEY_BYTE_LENGTH, bytesLength.ToString());
            parameters.Add(Work.KEY_CHAR_LENGTH, charsLength.ToString());
            String chapterUpdate = SQLiteDatabase.UpdateCommandTextOf(Work.TABLE_CHAPTERS, parameters, String.Format("{0}='{1}'", Work.KEY_ID, counters.ChaptersOffset));
            command.CommandText = chapterUpdate;
            command.ExecuteNonQuery();
        }

        #endregion

    }
}
