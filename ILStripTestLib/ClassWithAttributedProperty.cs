using System;
using System.Collections.Generic;

namespace ILStripTest
{
  class ClassWithAttributedProperty
  {
    [Custom]
    public string Test { get; }
  }
}
