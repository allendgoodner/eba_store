using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EbaLibrary
{
    public class DiscriminatedUnion
    {
        private Dictionary<string, Type> _union;

        public DiscriminatedUnion(Dictionary<string, Type> union)
        {
            _union = union;
        }

        public object Activate(string match, object[] arguments = null)
        {
            var t = _union[match];

            return t.GetConstructors().Single().Invoke(arguments);
        }
    }
}
