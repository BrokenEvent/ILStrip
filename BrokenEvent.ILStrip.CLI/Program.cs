using System;

using Plossum.CommandLine;

namespace BrokenEvent.ILStrip.CLI
{
  class Program
  {
    static int Main(string[] args)
    {
      CommandLineOptions options = new CommandLineOptions();
      CommandLineParser parser = new CommandLineParser(options);
      parser.Parse();

      if (parser.HasErrors)
      {
        Console.WriteLine(parser.UsageInfo.ToString(78, true));
#if DEBUG
        Console.ReadKey();
#endif
        return 1;
      }

      if (options.DoHelp)
      {
        Console.WriteLine(parser.UsageInfo.ToString());
#if DEBUG
        Console.ReadKey();
#endif
        return 0;
      }

      if (!options.Silent)
        Console.WriteLine("BrokenEvent.ILStrip.CLI, (C)2017, Broken Event");

      if (!options.Silent)
        Console.WriteLine("Reading: " + options.InputFilename);

      using (ILStrip ilStrip = new ILStrip(options.InputFilename))
      {
        //ilStrip.EntryPoints.Add("ILStripWPFTestLib.UI.MainWindow");
        ilStrip.EntryPoints.Add("ILStripWPFTestLib.UI.UnusedWindow");
        ilStrip.RemoveUnknownResources = true;

        if (!options.Silent)
          ilStrip.Logger = new CommandLineLogger();

        foreach (string s in options.EntryPoints)
          ilStrip.EntryPoints.Add(s);

        ilStrip.ScanUsedClasses();
        ilStrip.ScanUnusedClasses();

        if (options.CleanUnusedClasses)
          ilStrip.CleanupUnusedClasses();

        if (options.CleanUnusedReferences)
          ilStrip.CleanupUnusedReferences();

        if (options.CleanUnusedResources)
          ilStrip.CleanupUnusedResources();

        if (options.HideApi)
        {
          foreach (string s in options.HideExclusions)
            ilStrip.MakeInternalExclusions.Add(s);

          ilStrip.MakeInternal();
        }

        if (!options.Silent)
          Console.WriteLine("Writing: " + options.OutputFilename);
        ilStrip.Save(options.OutputFilename);
      }

#if DEBUG
      Console.ReadKey();
#endif
      return 0;
    }
  }
}
