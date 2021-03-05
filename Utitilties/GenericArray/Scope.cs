using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.GenericArray
{
    public class Scope : IScope, IComparable<Scope>
    {
        public int First { get; private set; }

        public int Last { get { return Length == 0 ? 0 : First + Length - 1; } }

        public int Length { get; private set; }

        public Scope(int first, int length)
        {
            if (first < 0 || length < 0)
                throw new IndexOutOfRangeException();

            First = first;
            Length = length;
        }

        public Scope(IScope other)
            : this(other.First, other.Length)
        { }

        public override string ToString()
        {
            return String.Format("{0}-{1}({2})", First, Last, Length);
        }

        //public void ResetLength(int newLength)
        //{
        //    if (newLength < 1)
        //        throw new Exception();

        //    Length = newLength;
        //}

        public void SetLast(int last)
        {
            if (Length != 0)
                throw new Exception("maybe no need to reset the scope?");
            else if (last < First)
                throw new ArgumentOutOfRangeException("The ending of the scope can not be less than starting(" + First.ToString() + "): " + last.ToString());

            Length = last - First + 1;
        }

        public bool Covers(int number)
        {
            return First <= number && Last >= number;
        }

        public bool Covers(IScope another)
        {
            return First <= another.First && Last >= another.Last;
        }

        public Scope IntersectionWith(IScope another)
        {
            int first = Math.Min(First, another.First);
            int last = Math.Max(Last, another.Last);
            return new Scope(first, last - first + 1);
        }

        #region IComparable<IScope> 成员

        public int CompareTo(IScope other)
        {
            return First.CompareTo(other.First);
        }

        #endregion

        #region IComparable<Scope> 成员

        public int CompareTo(Scope other)
        {
            return First.CompareTo(other.First);
        }

        #endregion
    }

    public class TextScope : Scope, ITextScope
    {
        public static string TextOf(char[] charArray, IScope range)
        {
            if (range.Last >= charArray.Length)
                throw new IndexOutOfRangeException();

            char[] segment = new char[range.Length];
            Array.Copy(charArray, range.First, segment, 0, range.Length);
            return new string(segment);
        }

        public static string TextOf(char[] charArray, int start, int length)
        {
            if (start < 0 || start + length >= charArray.Length)
                throw new IndexOutOfRangeException();

            char[] segment = new char[length];
            Array.Copy(charArray, start, segment, 0, length);
            return new string(segment);

        }

        public char[] Page { get; private set; }

        public string RawText { get { return TextOf(Page, this); } }

        public TextScope(char[] context, int first, int length)
            : base(first, length)
        {
            Page = context;
        }

        public TextScope(char[] context, IScope range)
            : base(range)
        {
            Page = context;
        }

        public TextScope(TextScope another)
            : base(another)
        {
            Page = another.Page;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}: {2}", First, Last, RawText);
        }

    }
}
