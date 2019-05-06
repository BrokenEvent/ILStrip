using BrokenEvent.Shared;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  [TestFixture]
  class WinFormsTests
  {
    [Test]
    public void RemoveUnusedForm()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWinFormsTestLib.exe"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWinFormsTestLib.UsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUsedForm");
      asserts.AssertNoClass("ILStripWinFormsTestLib.UnusedForm");
      asserts.AssertNoClass("ILStripWinFormsTestLib.ControlOfUnusedForm");
      asserts.AssertResource("ILStripWinFormsTestLib.UsedForm.resources");
      asserts.AssertNoResource("ILStripWinFormsTestLib.UnusedForm.resources");
      asserts.AssertResource("ILStripWinFormsTestLib.Resources.BrokenEventLogo.png");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void EntryPointForm()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWinFormsTestLib.exe"));
      strip.EntryPoints.Add("ILStripWinFormsTestLib.UnusedForm");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWinFormsTestLib.UsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.UnusedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUnusedForm");
      asserts.AssertResource("ILStripWinFormsTestLib.UsedForm.resources");
      asserts.AssertResource("ILStripWinFormsTestLib.UnusedForm.resources");
      asserts.AssertResource("ILStripWinFormsTestLib.Resources.BrokenEventLogo.png");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void RemoveUnknownResources()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWinFormsTestLib.exe"));
      strip.EntryPoints.Add("ILStripWinFormsTestLib.UnusedForm");
      strip.RemoveUnknownResources = true;

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWinFormsTestLib.UsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.UnusedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUnusedForm");
      asserts.AssertResource("ILStripWinFormsTestLib.UsedForm.resources");
      asserts.AssertResource("ILStripWinFormsTestLib.UnusedForm.resources");
      asserts.AssertNoResource("ILStripWinFormsTestLib.Resources.BrokenEventLogo.png");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void RemoveUnknownResourcesExcluded()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWinFormsTestLib.exe"));
      strip.UnusedResourceExclusions.Add("ILStripWinFormsTestLib.Resources.BrokenEventLogo.png");
      strip.RemoveUnknownResources = true;

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripWinFormsTestLib.UsedForm");
      asserts.AssertClass("ILStripWinFormsTestLib.ControlOfUsedForm");
      asserts.AssertNoClass("ILStripWinFormsTestLib.UnusedForm");
      asserts.AssertNoClass("ILStripWinFormsTestLib.ControlOfUnusedForm");
      asserts.AssertResource("ILStripWinFormsTestLib.UsedForm.resources");
      asserts.AssertNoResource("ILStripWinFormsTestLib.UnusedForm.resources");
      asserts.AssertResource("ILStripWinFormsTestLib.Resources.BrokenEventLogo.png");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }
  }
}
