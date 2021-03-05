using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AloudBible.Bible
{
    public enum ComponentType
    {
        Verse,
        Paragraph,
        Chapter,
        Book,
        Volume,
        Collection
    }

    [DataContract]
    public abstract class BookItem //: IComparable<BookItem>
    {
        public virtual CulturedConfig Culture { get { return Parent.Culture; } }

        //[DataMember]
        //public abstract int Number { get; set; }
        public abstract ComponentType ItemType { get; }
        public abstract Holder Parent { get; }


        //#region IComparable<BookItem> 成员

        //public virtual int CompareTo(BookItem other)
        //{
        //    if (this.Type == other.Type)
        //        return this.Number.CompareTo(other.Number);
        //    else
        //        throw new NotImplementedException("Should not be called!");
        //}

        //#endregion
    }
}
