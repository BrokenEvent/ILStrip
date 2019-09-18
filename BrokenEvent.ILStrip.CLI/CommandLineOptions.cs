using System;
using System.Collections.Generic;
using System.IO;

using BrokenEvent.Shared.Algorithms;

namespace BrokenEvent.ILStrip.CLI
{
  [CommandModel("BrokenEvent.ILStrip.CLI, (c)2017-2019 Broken Event")]
  internal class CommandLineOptions
  {
    private string inputFilename;

    [Command(0, "Input assembly filename to process.", "input", isRequired:true)]
    public string InputFilename
    {
      get { return inputFilename; }
      set
      {
        if (!File.Exists(value))
          throw new ArgumentException("File not found: " + value);

        inputFilename = value;
      }
    }

    [Command(1, "Output assembly filename to save processed assembly.", "output", isRequired: true)]
    public string OutputFilename { get; set; }

    [Command("s", "Suppresses logging.", alias: "silent", isFlag:true)]
    public bool Silent { get; set; }

    [Command("e", "User defined entry point classes list to start analysis. Multiple values.", "MyNamespace.MyClass")]
    public List<string> EntryPoints { get; set; } = new List<string>();

    [Command("b", "User defined entry point BAMLs list to start analysis. Multiple values.", "ui/mainwindow.baml")]
    public List<string> EntryPointBamls { get; set; } = new List<string>();

    [Command("h", "Hide public API with internal access modifier.", alias:"hide", isFlag:true)]
    public bool HideApi { get; set; }

    [Command("he", "Exclusions for -h option. Multiple values.", "MyNamespace.MyClass")]
    public List<string> HideExclusions { get; set; } = new List<string>();

    [Command("u", "Removes all unknown resources.", isFlag:true)]
    public bool RemoveUnknownResources { get; set; }

    [Command("re", "Resource exclusions for -u option. Multiple values.", "MyNamespace.MyResource")]
    public List<string> ResourceExclusions { get; set; } = new List<string>();

    [Command("we", "WPF Resource exclusions for -u option. Multiple values.", "resources/myresource.png")]
    public List<string> WpfResourceExclusions { get; set; } = new List<string>();

    [Command("import", "List of assemblies which use current to import used types as entry points.", "MyApp.exe")]
    public List<string> ImportEntryPoints { get; set; } = new List<string>();
  }
}
