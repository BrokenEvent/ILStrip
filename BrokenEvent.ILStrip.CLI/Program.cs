using System;

using BrokenEvent.Shared.Algorithms;

namespace BrokenEvent.ILStrip.CLI
{
  class Program
  {
    static int Main(string[] args)
    {
      CommandLineOptions options = new CommandLineOptions();
      CommandLineParser<CommandLineOptions> parser = new CommandLineParser<CommandLineOptions>(options);
      parser.AssignmentSyntax = true;
      parser.WriteUsageOnError = true;

      if (!parser.Parse(args))
      {
#if DEBUG
        Console.ReadKey();
#endif
        return 1;
      }

      if (!options.Silent)
        Console.WriteLine("Reading: " + options.InputFilename);

      using (ILStrip ilStrip = new ILStrip(options.InputFilename))
      {
        if (!options.Silent)
          ilStrip.Logger = new CommandLineLogger();

        foreach (string s in options.EntryPoints)
          ilStrip.EntryPoints.Add(s);

        foreach (string s in options.ResourceExclusions)
          ilStrip.UnusedResourceExclusions.Add(s);
        foreach (string s in options.WpfResourceExclusions)
          ilStrip.UnusedWpfResourceExclusions.Add(s);

        ilStrip.RemoveUnknownResources = options.RemoveUnknownResources;

        ilStrip.ScanUsedClasses();
        ilStrip.ScanUnusedClasses();
        ilStrip.CleanupUnusedClasses();
        ilStrip.CleanupUnusedReferences();
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
