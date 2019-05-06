using System.Windows;

using ILStripWPFTestLib.ViewModel;

namespace ILStripWPFTestLib.UI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new UsedViewModel();
    }
  }
}
