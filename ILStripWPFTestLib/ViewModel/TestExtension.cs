using System;
using System.Windows.Markup;

namespace ILStripWPFTestLib.ViewModel
{
  class TestExtension: MarkupExtension
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return "Test String";
    }
  }
}
