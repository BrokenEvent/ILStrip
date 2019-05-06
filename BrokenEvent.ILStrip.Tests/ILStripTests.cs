using BrokenEvent.Shared;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  [TestFixture]
  class ILStripTests
  {
    [Test]
    public void NoChangeTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.RegularClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.EmptyClass2");
      asserts.AssertClass("ILStripTest.Form1");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertClass("ILStripTest.Form1");
      asserts.AssertClass("ILStripTest.ControlOfForm1");
      asserts.AssertClass("ILStripTest.UnusedForm");
      asserts.AssertClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertClass("ILStripTest.ClassWithGeneric");
      asserts.AssertClass("ILStripTest.IInterface");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass2");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.UnusedForm");
      asserts.AssertNoClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.UnusedPrivateClass");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedResourcesTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.RegularClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.EmptyClass2");
      asserts.AssertClass("ILStripTest.Form1");
      asserts.AssertClass("ILStripTest.ControlOfForm1");
      asserts.AssertClass("ILStripTest.UnusedForm");
      asserts.AssertClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoResource("ILStripTest.Form1.resources");
      asserts.AssertNoResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedReferencesTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass2");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.UnusedForm");
      asserts.AssertNoClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib"); // can't be removed
      asserts.AssertNoReference("System.Drawing");
      asserts.AssertNoReference("System.Windows.Forms");
    }

    [Test]
    public void EntryPointsTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.RegularClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.RegularClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.EmptyClass2");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.UnusedForm");
      asserts.AssertNoClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric");
      asserts.AssertNoResource("ILStripTest.Form1.resources");
      asserts.AssertNoResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertNoReference("System.Drawing");
      asserts.AssertNoReference("System.Windows.Forms");
    }

    [Test]
    public void WinFormsEntryPointsTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.Form1");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.Form1");
      asserts.AssertClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass2");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.UnusedForm");
      asserts.AssertNoClass("ILStripTest.ControlOfUnusedForm");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertNoResource("ILStripTest.UnusedForm.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void MakeInternalTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.Form1");
      strip.EntryPoints.Add("ILStripTest.RegularClass");
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass");
      strip.MakeInternalExclusions.Add("ILStripTest.EmptyClass");
      strip.MakeInternalExclusions.Add("ILStripTest.ClassWithNestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();
      strip.MakeInternal();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.RegularClass", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.EmptyClass", ClassModifier.Public);
      asserts.AssertClass("ILStripTest.EmptyClass2", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.Form1", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.ControlOfForm1", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.ClassWithNestedClass", ClassModifier.Public);
      asserts.AssertClass("ILStripTest.CustomAttribute", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.IInterface", ClassModifier.Internal);
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedNestedClassesTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass");
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass2");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesButUseNestedTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2/NestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass2");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesWithGenericsTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithGeneric");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.RegularClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.EmptyClass2");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.Form1");
      asserts.AssertNoClass("ILStripTest.ControlOfForm1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertResource("ILStripTest.Form1.resources");
      asserts.AssertReference("mscorlib");
      asserts.AssertReference("System.Drawing");
      asserts.AssertReference("System.Windows.Forms");
    }
  }
}
