using System.Collections.Generic;
using System.IO;

using Plossum.CommandLine;

namespace BrokenEvent.ILStrip.CLI
{
  [CommandLineManager(ApplicationName = "BrokenEvent.ILStrip.CLI", Copyright = "(C)2017-2019, Broken Event", EnabledOptionStyles = OptionStyles.Windows)]
  internal class CommandLineOptions
  {
    private string inputFilename;

    [CommandLineOption(Name = "s", Aliases = "silent", Description = "Suppress logging.")]
    public bool Silent { get; set; }

    [CommandLineOption(Aliases = "i", Name = "input", Description = "Input assembly filename to process.", MinOccurs = 1)]
    public string InputFilename
    {
      get { return inputFilename; }
      set
      {
        if (!File.Exists(value))
          throw new InvalidOptionValueException("File not found: " + value);
        inputFilename = value;
      }
    }

    [CommandLineOption(Aliases = "o", Name = "output", Description = "Output assembly filename to save.", MinOccurs = 1)]
    public string OutputFilename { get; set; }

    [CommandLineOption(Name = "entry", Description = "User defined entry points list to start analysis. Multiple values.", RequireExplicitAssignment = true)]
    public List<string> EntryPoints { get; } = new List<string>();

    [CommandLineOption(Name = "exclude", Description = "Exclusions from hiding list. Multiple values.", RequireExplicitAssignment = true)]
    public List<string> HideExclusions { get; } = new List<string>();

    [CommandLineOption(Aliases = "c", Name = "classes", Description = "Clean unused classes.")]
    public bool CleanUnusedClasses { get; set; }

    [CommandLineOption(Aliases = "r", Name = "refs", Description = "Clean unused references.")]
    public bool CleanUnusedReferences { get; set; }

    [CommandLineOption(Aliases = "rs", Name = "res", Description = "Clean unused WinForms resources.")]
    public bool CleanUnusedResources { get; set; }

    [CommandLineOption(Aliases = "h", Name = "hide", Description = "Hide public API with internal.")]
    public bool HideApi { get; set; }

    [CommandLineOption(Name = "?", Aliases = "Help", Description = "Displays this help message")]
    public bool DoHelp { get; set; }
  }
}
