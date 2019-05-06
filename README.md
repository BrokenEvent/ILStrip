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
* Custom entry points for types that don't refernced, but should remain.
* Hide private types with `internal` access modifier.
* WPF/BAML support.

## Usecase

The main goal is to filter .NET assemblies from unused classes after merging with [ILRepack](https://github.com/ststeiger/ILRepack).
It will not optimize your code or somehow change its behavior, but will erase all that is not used in current assembly.

*ILStrip* works only on class level. If class is used somehow, all its members will remain even if they are not used.

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
stripper.EntryPointBamls.Add("ui/mywindow.baml");
    
// walk the assembly opcodes to find all types and references used
stripper.ScanUsedClasses();

// walk the assembly types to build list of all types that aren't used
stripper.ScanUnusedClasses();

// remove all unused types
stripper.CleanupUnusedClasses();

// exclude necessary resources from cleanup
stripper.UnusedResourceExclusions.Add("MyNamespace.MyImage.png");

// exclude necessary WPF resources from cleanup
stripper.UnusedWpfResourceExclusions.Add("res/myimage.png");

// remove all unused WinForms and WPF resources
stripper.CleanupUnusedResources();

// remove all unused references
stripper.CleanupUnusedReferences();

// exclude the target public API
stripper.MakeInternalExclusions.Add("MyNamespace.MyClass");

// hide all other with internal
stripper.MakeInternal();

stripper.Save(outputPath);
// stripper.Save(outputStream);
```

### Commandline Tool

In reasons of convenience there is a commandline tool, built from improvised means.
This makes the ILStrip to work as standalone or as a part of any build script.
Usage:

    Syntax:
    BrokenEvent.ILStrip.CLI.exe input output [-s] [-e MyNamespace.MyClass] [-h] [-he MyNamespace.MyClass] [-u] [-re MyNamespace.MyResource] [-we resources/myresource.png]

    Arguments:
    input           Input assembly filename to process.
    output          Output assembly filename to save processed assembly.
    -s, -silent     Suppresses logging. Optional.
    -e              User defined entry point classes list to start analysis. Multiple values. Optional.
    -h, -hide       Hide public API with internal access modifier. Optional.
    -he             Exclusions for -h option. Multiple values. Optional.
    -u              Removes all unknown resources. Optional.
    -re             Resource exclusions for -u option. Multiple values. Optional.
    -we             WPF Resource exclusions for -u option. Multiple values. Optional.


### Analysis/How it works
The *ILStrip* will parse all the methods of classes and find all the types that was used. Then parse them and so on.
The starting points of the scanning are called *entry points*. If the assembly is executable, the main entry method is used as entry point automatically.

You may add another *entry points* for classes, that will be instantinated with *reflection* or somehow else.
It is used for classes, that have no internal references, but you want them to remain in assembly.

After building this list (`stripper.ScanUsedClasses()`) anything that isn't listed as used is trash. `ScanUnusedClasses()` builds the complete list of it.

Parsing of XAML/BAML is also supported. Any class which is referenced in BAML will be counted as used even if is the only reference.
Including ResourceDictionaries will work same way. Loading ResourceDictionary from code can't be detected. You should add such BAMLs as entry points.

### WPF
The WPF `Application` doesn't use XAML for startup. The startup window is called from code with XAML resource name in string.
This couldn't be detected so in this case you should manually add window class (or window BAML) to entry points.

However, if the window is started from code manually (`new MyWindow().Show()` or something like this) it will be detected as usual type reference.

### Resources

#### Default

By default ILStrip will try to cleanup only the resources that are known as unused. By default the following resources will be removed:

* Any `%className%.resources` embedded resource for class that is surely known as unused. These are mostly WinForms forms and controls.
* Any `.baml` from WPF resource which isn't built for some class (like XAMLs for windows and controls) and isn't referenced by other used BAML.

All other resources will remain in result assembly.

#### Full Cleanup

You can enable `RemoveUnknownResources` option which will remove everything which isn't surely known as used. In this case the following resources will NOT be removed:

* Any `%className%.resources` embedded resource for class that is surely known as used. These are mostly WinForms forms and controls.
* Any embedded resource, which name is added to `UnusedResourceExclusions`.
* Any `.baml` from WPF resource which is used by another BAML or related with used class.
* Any WPF resource, which name was added to `UnusedWpfResourceExclusions`.

Any other resource will be removed. Be very careful when using this option.

Both cases are not related with .NET resource manager, which is generated by Visual Studio. This resource will remain as is in any case.

## Credits
Uses [Mono.Cecil](https://github.com/jbevain/cecil) from Mono project.

Uses [BamlParser](https://github.com/timotei/bamlparser) from Confuser.

© 2017-2019 Broken Event. [brokenevent.com](https://brokenevent.com)

### NuGet

There is no nuget package for now, but we'll make it if the community require.

## Download
[BrokenEvent.ILStrip](https://ci.appveyor.com/api/projects/BrokenEvent/ilstrip/artifacts/BrokenEvent.ILStrip.zip)

[BrokenEvent.ILStrip.CLI](https://ci.appveyor.com/api/projects/BrokenEvent/ilstrip/artifacts/BrokenEvent.ILStrip.CLI.zip)
