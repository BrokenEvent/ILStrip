using System.Windows.Forms;

namespace ILStripWinFormsTestLib
{
  public partial class UnusedForm : Form
  {
    public UnusedForm()
    {
      InitializeComponent();
    }
  }

  public class ControlOfUnusedForm: Control { }
}
