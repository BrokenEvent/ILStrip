using System;
using System.Windows.Markup;

namespace ILStripWPFTestLib.ViewModel
{
  class TestExtension: MarkupExtension
  {
    public string value;

    public TestExtension(string value)
    {
      this.value = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return value;
    }
  }
}
