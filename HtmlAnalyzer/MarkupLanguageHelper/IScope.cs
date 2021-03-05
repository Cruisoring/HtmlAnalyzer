using System;
namespace HtmlAnalyzer.MarkupLanguageHelper
{
    public interface IScope : IComparable<IScope>
    {
        int First { get; }
        int Last { get; }
        int Length { get; }

        bool Covers(IScope another);

        Scope IntersectionWith(IScope another);
    }

    public interface ITextScope : IScope
    {
        string RawText { get; }
    }

    public class Scope : IScope, IComparable<Scope>
    {
        public int First { get; private set; }

        public int Last { get { return First + Length - 1; } }

        public int Length { get; private set; }

        public Scope(int first, int length)
        {
            if (first < 0 || length < 1)
                throw new IndexOutOfRangeException();

            First = first;
            Length = length;
        }

        public Scope(IScope other) : this(other.First, other.Length)
        {}

        public override string ToString()
        {
            return String.Format("{0}-{1}({2})", First, Last, Length);
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
