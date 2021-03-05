using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utitilties.GenericArray;
using System.Runtime.Serialization;

namespace AloudBible.Bible
{
    [DataContract]
    public abstract class Holder : BookItem
    {
        #region Property
        //public override int Number { get { return Parent == null ? -1 : Parent.IndexOf(this); } }

        protected Holder parent = null;
        public override Holder Parent { get{ return parent;} }

        public abstract IEnumerable<BookItem> Siblings { get;}

        //public virtual char[] Content { get { return Parent.Content; } }

        //public virtual int ContentLength { get { return Parent.ContentLength; } }

        public string Text { get { return TextOf(TheScope.First, TheScope.Length); } }

        protected int offset = 0;
        //protected int pageOffset = 0;
        //public virtual int PageOffset { get { return this.pageOffset + Parent.PageOffset; } }

        protected int length = 0;
        public Scope TheScope { get { return new Scope(offset, length); } }

        #endregion

        #region Constructor
        protected Holder() {}

        protected Holder(Holder parent, int offset)
        {
            if (parent == null || parent.ItemType <= this.ItemType)
                throw new Exception("The parent type shall be greater than own.");

            this.parent = parent;
            this.offset = offset;
            //this.Number = (parent == null) ? 0 : parent.Siblings.Count() + 1;
        }
        #endregion

        #region Function
        //public virtual string TextOf(Scope scope)
        //{
        //    return Parent.TextOf(scope);
        //}

        public virtual string TextOf(int start, int length)
        {
            return Parent.TextOf(start, length);
        }

        public abstract void EndLoading();

        public abstract int IndexOf(BookItem sibling);
        #endregion

    }
}
