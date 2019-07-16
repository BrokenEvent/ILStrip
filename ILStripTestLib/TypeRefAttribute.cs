using System;

namespace ILStripTest
{
  public class TypeRefAttribute: Attribute
  {
    public Type CustomType { get; }

    public TypeRefAttribute(Type customType)
    {
      CustomType = customType;
    }
  }
}
