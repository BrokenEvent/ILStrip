using System;

namespace ILStripTest
{
  class ClassWithEvents
  {
    public event Action<EmptyClass> Event;
  }
}
