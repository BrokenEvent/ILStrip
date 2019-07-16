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
