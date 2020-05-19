using System;

namespace ILStripTest
{
  [AttributeTwo]
  class AttributeOne: Attribute
  {
    
  }

  [AttributeOne]
  class AttributeTwo: Attribute
  {
    
  }
}
