using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILStripTest
{
  public class ClassWithNestedClass
  {
    public string DoWithNested()
    {
      return new NestedClass().Do();
    }

    public class NestedClass
    {
      public string Do()
      {
        return "string";
      }
    }
  }
}
