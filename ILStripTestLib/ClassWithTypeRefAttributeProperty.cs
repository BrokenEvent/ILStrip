namespace ILStripTest
{
  public class ClassWithTypeRefAttributeProperty
  {
    [TypeRef(typeof(EmptyClass))]
    public int Property { get; set; }
  }
}
