using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.GenericArray;
using System.Runtime.Serialization;

namespace AloudBible.Bible
{
    [DataContract]
    public sealed class Section : BookItem, IEnumerable<Verse>, IComparable<Section>
    {
        #region Property
        public override ComponentType ItemType
        {
            get { return ComponentType.Paragraph; }
        }

        public Chapter TheChapter { get; private set; }

        public override Holder Parent
        {
            get { return TheChapter; }
        }

        [DataMember(Name = "p", Order=0)]
        public  int Number { get; set; }

        [DataMemberAttribute(Name = "t", Order = 1)]
        public string Title { get; set; }

        //public Scope VerseScope { get; set; }

        [DataMember(Name = "f", Order = 2)]
        public int First { get; set; }

        [DataMember(Name = "l", Order = 3)]
        public int Length { get; set; }

        public int VerseFrom { get; set; }
        public int VerseCount { get; set; }
        
        #endregion

        #region Constructor
        public Section(Chapter parent, string title, int offset, int verseFrom)
        {
            TheChapter = parent;
            this.Title = title;
            this.VerseFrom = verseFrom;

            Number = TheChapter.Sections.Count + 1;

            //if (Number == 1 || TheChapter.Verses.Count == 0)
                First = offset;
        }
        #endregion

        #region Function

        public override string ToString()
        {
            //return string.Format("{0}({1}-{2}): {3}", TheChapter, First, Last, Title);
            return string.Format("{0}({1}-?): {2}", TheChapter, Number, Title);
        }
        

        //public override int IndexOf(BookItem sibling)
        //{
        //    if (sibling is Verse)
        //    {
        //        Verse verse = sibling as Verse;

        //        if (verse.Parent != Parent)
        //            return -1;

        //        int index = verse.Number;

        //        if (First > index || Last < index)
        //            return -1;
        //        else
        //            return index + 1 - First;
        //    }
        //    else
        //        throw new NotImplementedException();
        //}

        #region IEnumerable<Verse> 成员

        public IEnumerator<Verse> GetEnumerator()
        {
            throw new NotImplementedException();
            //for (int i = VerseScope.First; i <= VerseScope.Last; i ++ )
            //{
            //    yield return TheChapter[i];
            //}
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion

        #region IComparable<Paragraph> 成员

        public int CompareTo(Section other)
        {
            if (this.Parent == other.Parent)
                return this.Number.CompareTo(other.Number);
            else
                return this.TheChapter.CompareTo(other.TheChapter);
        }

        #endregion
    }
}
