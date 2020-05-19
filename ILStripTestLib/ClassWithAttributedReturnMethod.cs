namespace ILStripTest
{
  class ClassWithAttributedReturnMethod
  {
    [return: Custom]
    public object Do()
    {
      return null;
    }
  }
}
