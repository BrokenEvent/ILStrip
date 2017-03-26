namespace ILStripTest
{
  public class RegularClass: IInterface
  {
    public void DoPublic()
    {
      new EmptyClass();
    }

    private void DoPrivate()
    {
      new EmptyClass2();
    }
  }
}
