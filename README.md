# BrokenEvent.ILStrip

C# Assembly unused classes/references/resources cleaner.

## Features

* Used/unused classes analysis.
* Unused classes cleanup.
* Unused references clanup.
* Unused resources cleanup (used to clean resources of unused WinForms forms).
* Entry points support (points to start the usage anaylsis).
* Hide public API with **internal** access modifier.

## Usecase

The main goal is to filter .NET assemblies from unused classes after merging with [ILRepack](https://github.com/ststeiger/ILRepack). It will not optimize your code or somehow change its behavior, but will erase all that is not used in current assembly.

## Usage

### Example

In the reasons of convenience, all operations are divided into separate methods, that should be called in sequence:

    ILStrip stripper =
      new ILStrip(inputPath);
    // new ILStrip(inputStream);
    // new ILStrip(assemblyDefinition);
    
    // add whatever you want by name.
    // executable's Main(string[] args) will be added automatically
    stripper.EntryPoints.Add("MyNamespace.MyClass");
    
    // walk the assembly opcodes to find all types used
    stripper.ScanUsedClasses();

    // walk the assembly types to list all that is not used
    stripper.ScanUnusedClasses();

    // remove all unused types
    stripper.CleanupUnusedClasses();

    // remove all unused WinForms resources (unusedType.FullName + ".resources")
    stripper.CleanupUnusedResources();

    // remove all unused references
    stripper.CleanupUnusedReferences();

    // exclude the target public API
    stripper.MakeInternalExclusions.Add("MyNamespace.MyClass");

    // hide all other with internal
    stripper.MakeNotPublic();

    stripper.Save(outputPath);
    // stripper.Save(outputStream);

### Analysis/How it works
The ILStrip will parse all the method of types and find all the types that was used. Then parse them and so on. The starting points of the scanning are called "entry points". If the assembly is executable, the main entry method is is used as entry point automatically. You may add another entry points for classes, that will be instantinated with *reflection* or somehow else. The classes, that have no internal references, but you want them to remain in assembly. Same was done with the external references.

After building this list (stripper.ScanUsedClasses()) the anything that isn't listed as used, is trash. ScanUnusedClasses() builds the complete list of it.

## Credits
Uses [Mono.Cecil](https://github.com/jbevain/cecil).

(C) 2017, Broken Event. [brokenevent.com](http://brokenevent.com)

### NuGet

There is no nuget package for now, but we'll make it if the community require.
