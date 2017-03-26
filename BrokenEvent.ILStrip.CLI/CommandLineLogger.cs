using System;

namespace BrokenEvent.ILStrip.CLI
{
  class CommandLineLogger: ILStripLogger
  {
    public void LogMessage(string msg)
    {
      Console.WriteLine(msg);
    }
  }
}
