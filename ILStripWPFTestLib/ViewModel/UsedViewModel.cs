using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ILStripWPFTestLib.ViewModel
{
  class UsedViewModel: INotifyPropertyChanged
  {
    private bool isChecked;

    public bool IsChecked
    {
      get { return isChecked; }
      set
      {
        isChecked = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
