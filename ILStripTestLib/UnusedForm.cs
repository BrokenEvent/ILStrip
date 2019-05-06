using System.Windows.Forms;

namespace ILStripTest
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
