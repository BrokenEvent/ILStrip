using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ILStripWPFTestLib.ViewModel;

namespace ILStripWPFTestLib.UI
{
  /// <summary>
  /// Interaction logic for UnusedWindow.xaml
  /// </summary>
  public partial class UnusedWindow : Window
  {
    public UnusedWindow()
    {
      InitializeComponent();
      DataContext = new UnusedViewModel();
    }
  }
}
