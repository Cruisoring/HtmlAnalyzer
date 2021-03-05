using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.GenericArray;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AloudBible.Bible
{
    [DataContract]
    public class Verse : BookItem, IComparable<Verse>, IComparable<Section>
    {
        #region Property
        [DataMember(Name="v", Order = 0)]
        public int Number { get; set; }

        [IgnoreDataMember]
        public override ComponentType ItemType
        {
            get { return ComponentType.Verse; }
        }

        [IgnoreDataMember]
        public Chapter TheChapter { get; private set; }

        [IgnoreDataMember]
        public override Holder Parent { get { return TheChapter; } }

        //public Char[] Content { get { return Parent.Content; } }
        //[DataMember(Name="scope")]
        public Scope TheScope { get; private set; }

        [DataMemberAttribute(Name="f", Order=1)]
        public int First { get { return TheScope.First; } set { ;} }

        [DataMemberAttribute(Name="l", Order=2)]
        public int Length { get { return TheScope.Length; } set { ;} }

        //public Scope ScopeInChapter { get; private set; }

        //public Scope ScopeInBook { get { throw new NotImplementedException(); } }

        //public Scope ScopeInCollection { get { throw new NotImplementedException(); } }

        [IgnoreDataMember]
        public string Text { get { return TheChapter.TextOf(First, Length); } }
        #endregion

        public Verse(Chapter chapter, string text, int offset)
        {            
            TheChapter = chapter;
            TheScope = new Scope(offset, text.Length);
            Number = chapter.Verses.Count + 1;
        }

        #region Function
        public override string ToString()
        {
            return string.Format("{0}-{1}: {2}", TheChapter, Number, Text);
        }

        #region IComparable<Verse> 

        public int CompareTo(Verse other)
        {
            if (this.Parent == other.Parent)
                return this.Number.CompareTo(other.Number);
            else
                return this.TheChapter.CompareTo(other.TheChapter);
        }

        #endregion

        #region IComparable<Paragraph> 

        public int CompareTo(Section other)
        {
            if (this.Parent == other.Parent)
                return this.Number.CompareTo(other.Number);
            else
                return this.TheChapter.CompareTo(other.TheChapter);
        }

        #endregion
        #endregion
    }
}
