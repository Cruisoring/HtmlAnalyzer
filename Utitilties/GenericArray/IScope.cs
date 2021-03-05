using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utitilties.GenericArray
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
}
