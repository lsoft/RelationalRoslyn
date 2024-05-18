using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpProjectConsole
{
    internal sealed partial class MyClass1
    {
        public Dictionary<MyClass1, MyClass1> MyField;
        public readonly Dictionary<MyClass1, MyClass1> MyReadonlyField;

        public Dictionary<MyClass1, MyClass1> MyGetProperty { get; }
        public Dictionary<MyClass1, MyClass1> MyGetSetProperty { get; set; }

        public Dictionary<MyClass1, MyClass1> MyMethod1(
            List<MyClass1> argument,
            ref HashSet<MyClass1> refArgument,
            in MyClass1[] inArgument,
            out SortedSet<MyClass1> outArgument
            )
        {
            throw new NotImplementedException();
        }
    }
}
