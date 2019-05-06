using System.Collections.Generic;

namespace ILStripTest
{
  public class ClassWithGeneric<TClass1>
  {
#pragma warning disable 169
    private List<EmptyClass> emptyClasses;
#pragma warning restore 169

    public void Do<TClass2>(TClass2 arg)
    {
      
    }

    public void Do()
    {
      Do<AttributedClass>(null);
    }

    public static void DoStatic()
    {
      new ClassWithGeneric<EmptyClass>().Do();
    }
  }
}
