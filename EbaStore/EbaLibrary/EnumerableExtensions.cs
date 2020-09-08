using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EbaLibrary
{
    public static class EnumerableExtensions
    {
        public static bool CollectionsEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return (a.Count() == b.Count() &&
                (!a.Except(b).Any() && !b.Except(a).Any()));
        }
    }
}
