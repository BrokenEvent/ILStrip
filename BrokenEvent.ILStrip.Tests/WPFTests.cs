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

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
    }

    [Test]
    public void MainWindowCsEntryPoint()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPoints.Add("ILStripWPFTestLib.UI.MainWindow");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
    }

    [Test]
    public void MainWindowBamlEntryPoint()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));
      strip.EntryPointBamls.Add("ui/mainwindow.baml");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWPFTestLib.App");
      asserts.AssertClass("ILStripWPFTestLib.UI.MainWindow");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.UsedViewModel");
      asserts.AssertClass("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter");
      asserts.AssertNoClass("ILStripWPFTestLib.UI.UnusedWindow");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.UnusedViewModel");
      asserts.AssertNoClass("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter");
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

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClassPublic("ILStripWPFTestLib.App", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.UI.MainWindow", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.ViewModel.UsedViewModel", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.ViewModel.Converters.UsedValueConverter", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.UI.UnusedWindow", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.ViewModel.UnusedViewModel", false);
      asserts.AssertClassPublic("ILStripWPFTestLib.ViewModel.Converters.UnusedValueConverter", false);
    }

  }
}
