using System;
using System.Collections.Generic;

namespace ILStripTest
{
  class ClassWithAttributedField
  {
    [Custom]
    private int i;
  }
}
