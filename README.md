[![Build Status](https://img.shields.io/appveyor/ci/BrokenEvent/ILStrip/master.svg?style=flat-square)](https://ci.appveyor.com/project/BrokenEvent/ilstrip)
[![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://raw.githubusercontent.com/BrokenEvent/ILStrip/master/LICENSE)
[![Github Releases](https://img.shields.io/github/downloads/BrokenEvent/ILStrip/total.svg?style=flat-square)](https://github.com/BrokenEvent/ILStrip/releases)

# BrokenEvent.ILStrip

C# Assembly unused classes/references/resources cleaner.

## Features

* Used/unused classes analysis.
* Unused classes cleanup.
* Unused references clanup.
* Unused resources cleanup.
* Entry points support (points to start the usage anaylsis).
* Hide public API with **internal** access modifier.

## Usecase

The main goal is to filter .NET assemblies from unused classes after merging with [ILRepack](https://github.com/ststeiger/ILRepack). It will not optimize your code or somehow change its behavior, but will erase all that is not used in current assembly.

## Usage

### Code Example

In the reasons of convenience, all operations are divided into separate methods, that should be called in sequence:

```C#
ILStrip stripper =
  new ILStrip(inputPath);
// new ILStrip(inputStream);
// new ILStrip(assemblyDefinition);
    
// add whatever you want by name.
// executable's Main(string[] args) will be added automatically
stripper.EntryPoints.Add("MyNamespace.MyClass");
    
// walk the assembly opcodes to find all types, references and resources used
// all resources named "%usedType%.resources" will be counted as used
stripper.ScanUsedClasses();

// walk the assembly types to find all that is not used
stripper.ScanUnusedClasses();

// remove all unused types
stripper.CleanupUnusedClasses();

// exclude necessary resources from cleanup
stripper.UnusedResourceExclusions.Add("MyNamespace.MyImage.png");

// remove all unused WinForms resources
stripper.CleanupUnusedResources();

// remove all unused references
stripper.CleanupUnusedReferences();

// exclude the target public API
stripper.MakeInternalExclusions.Add("MyNamespace.MyClass");

// hide all other with internal
stripper.MakeNotPublic();

stripper.Save(outputPath);
// stripper.Save(outputStream);
```

### Commandline Tool

In reasons of convenience there is a commandline tool, built from improvised means. This make the ILStrip to work as standalone or as a part of any build script.
Usage:

    Syntax: BrokenEvent.ILStrip.CLI.exe /i:%inputFile /o:%outputFile [options]

    /?, /Help - Displays the help message
    /classes, /c - Clean unused classes.
    /entry - User defined entry points list to start analysis. Multiple values.
    /exclude - Exclusions from hiding list. Multiple values.
    /hide, /h - Hide public API with internal.
    /input, /i - Input assembly filename to process.
    /output, /o - Output assembly filename to save.
    /refs, /r - Clean unused references.
    /res, /rs - Clean unused WinForms resources.
    /s, /silent - Suppress logging.

### Analysis/How it works
The ILStrip will parse all the methods of classes and find all the types that was used. Then parse them and so on. The starting points of the scanning are called "entry points". If the assembly is executable, the main entry method is is used as entry point automatically. You may add another entry points for classes, that will be instantinated with *reflection* or somehow else. The classes, that have no internal references, but you want them to remain in assembly. Same was done with the external references.

After building this list (`stripper.ScanUsedClasses()`) the anything that isn't listed as used is trash. `ScanUnusedClasses()` builds the complete list of it.

## Credits
Uses [Mono.Cecil](https://github.com/jbevain/cecil).

(C) 2017, Broken Event. [brokenevent.com](http://brokenevent.com)

### NuGet

There is no nuget package for now, but we'll make it if the community require.

## Download
[BrokenEvent.ILStrip](https://ci.appveyor.com/api/projects/BrokenEvent/ilstrip/artifacts/BrokenEvent.ILStrip.zip)

[BrokenEvent.ILStrip.CLI](https://ci.appveyor.com/api/projects/BrokenEvent/ilstrip/artifacts/BrokenEvent.ILStrip.CLI.zip)
