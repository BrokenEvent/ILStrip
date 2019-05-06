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
      asserts.AssertNoBaml("ui/mainwindow.baml");
      asserts.AssertNoBaml("ui/unusedwindow.baml");
      asserts.AssertNoBaml("testdictionary.baml");
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
      asserts.AssertBaml("ui/mainwindow.baml");
      asserts.AssertNoBaml("ui/unusedwindow.baml");
      asserts.AssertNoBaml("testdictionary.baml");
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
      asserts.AssertBaml("ui/mainwindow.baml");
      asserts.AssertNoBaml("ui/unusedwindow.baml");
      asserts.AssertNoBaml("testdictionary.baml");
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
      asserts.AssertBaml("ui/mainwindow.baml");
      asserts.AssertBaml("ui/unusedwindow.baml");
      asserts.AssertBaml("testdictionary.baml");
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
      asserts.AssertBaml("ui/mainwindow.baml");
      asserts.AssertNoBaml("ui/unusedwindow.baml");
      asserts.AssertBaml("testdictionary.baml");
    }

  }
}
