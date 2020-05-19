namespace ILStripTest
{
  public class UserClass: IInterface
  {
    public void DoPublic()
    {
      new EmptyClass();
    }

    private void DoPrivate()
    {
      new AttributedClass();
    }

    private void RecursiveMethod()
    {
      RecursiveMethod();
    }
  }
}
