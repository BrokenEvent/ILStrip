using System.Collections.Generic;

namespace ILStripTest
{
  public class ClassWithGeneric
  {
#pragma warning disable 169
    private List<EmptyClass> emptyClasses;
#pragma warning restore 169

    public void Do<T>(T arg)
    {
      
    }

    public void Do()
    {
      Do<EmptyClass2>(null);
    }
  }
}
