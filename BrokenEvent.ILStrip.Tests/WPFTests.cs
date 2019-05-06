using BrokenEvent.Shared;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  [TestFixture]
  class WPFTests
  {
    [Test]
    public void NoEntryPoints()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
      asserts.AssertNoWpfResource("ui/mainwindow.baml");
      asserts.AssertNoWpfResource("ui/unusedwindow.baml");
      asserts.AssertNoWpfResource("testdictionary.baml");
    }

    [Test]
    public void MainWindowCsEntryPoint()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.MainWindow");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
      asserts.AssertWpfResource("ui/mainwindow.baml");
      asserts.AssertNoWpfResource("ui/unusedwindow.baml");
      asserts.AssertNoWpfResource("testdictionary.baml");
    }

    [Test]
    public void MainWindowBamlEntryPoint()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPointBamls.Add("ui/mainwindow.baml");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
      asserts.AssertWpfResource("ui/mainwindow.baml");
      asserts.AssertNoWpfResource("ui/unusedwindow.baml");
      asserts.AssertNoWpfResource("testdictionary.baml");
    }

    [Test]
    public void MainWindowMixedEntryPoints()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPointBamls.Add("ui/mainwindow.baml");
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.UnusedWindow");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.MakeInternal();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.UI.UnusedWindow", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UnusedViewModel", ClassModifier.Internal);
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter", ClassModifier.Internal);
      asserts.AssertWpfResource("ui/mainwindow.baml");
      asserts.AssertWpfResource("ui/unusedwindow.baml");
      asserts.AssertWpfResource("testdictionary.baml");
    }

    [Test]
    public void ManualBamlExclusion()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPointBamls.Add("ui/mainwindow.baml");
      strip.EntryPointBamls.Add("testdictionary.baml");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
      asserts.AssertWpfResource("ui/mainwindow.baml");
      asserts.AssertNoWpfResource("ui/unusedwindow.baml");
      asserts.AssertWpfResource("testdictionary.baml");
    }

    [Test]
    public void RemoveUnknownResourcesUsed()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.UnusedWindow");
      strip.RemoveUnknownResources = true;

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertWpfResource("resources/brokeneventlogo.png");
    }

    [Test]
    public void RemoveUnknownResourcesUnused()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.MainWindow");
      strip.RemoveUnknownResources = true;

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoWpfResource("resources/brokeneventlogo.png");
    }

    [Test]
    public void RemoveUnknownResourcesExcluded()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.MainWindow");
      strip.UnusedWpfResourceExclusions.Add("resources/brokeneventlogo.png");
      strip.RemoveUnknownResources = true;

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertWpfResource("resources/brokeneventlogo.png");
    }
  }
}
