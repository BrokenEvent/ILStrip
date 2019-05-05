using System.IO;

using Mono.Cecil;

using NUnit.Framework;

namespace BrokenEvent.Shared.ILStripTests
{
  [TestFixture]
  class ILStripTests
  {
    [Test]
    public void NoChangeTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass/NestedClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass2/NestedClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass/NestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2/NestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.UnusedPrivateClass");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedResourcesTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedResources();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertNoResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedReferencesTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedReferences();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib"); // can't be removed
      AssemblyAsserts.AssertNoReference(def, "System.Drawing");
      AssemblyAsserts.AssertNoReference(def, "System.Windows.Forms");
    }

    [Test]
    public void EntryPointsTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.RegularClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertNoResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertNoResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertNoReference(def, "System.Drawing");
      AssemblyAsserts.AssertNoReference(def, "System.Windows.Forms");
    }

    [Test]
    public void WinFormsEntryPointsTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.Form1");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedResources();
      strip.CleanupUnusedReferences();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.UnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfUnusedForm");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertNoResource(def, "ILStripTest.UnusedForm.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void MakeInternalTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
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

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.RegularClass", false);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass", true);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.EmptyClass2", false);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.Form1", false);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ControlOfForm1", false);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.ClassWithNestedClass", true);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.CustomAttribute", false);
      AssemblyAsserts.AssertClassPublic(def, "ILStripTest.IInterface", false);
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedNestedClassesTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass");
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertClass(def, "ILStripTest.ClassWithNestedClass/NestedClass");
      AssemblyAsserts.AssertClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2/NestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesButUseNestedTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2/NestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass/NestedClass");
      AssemblyAsserts.AssertClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertClass(def, "ILStripTest.ClassWithNestedClass2/NestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithGeneric");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.IInterface");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }

    [Test]
    public void CleanupUnusedClassesWithGenericsTest()
    {
      ILStrip.ILStrip strip = new ILStrip.ILStrip(TestHelper.TranslatePath("ILStripTest.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithGeneric");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
      AssemblyAsserts.AssertClass(def, "ILStripTest.EmptyClass");
      AssemblyAsserts.AssertClass(def, "ILStripTest.EmptyClass2");
      AssemblyAsserts.AssertClass(def, "ILStripTest.CustomAttribute");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.Form1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ControlOfForm1");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass/NestedClass");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2");
      AssemblyAsserts.AssertNoClass(def, "ILStripTest.ClassWithNestedClass2/NestedClass");
      AssemblyAsserts.AssertResource(def, "ILStripTest.Form1.resources");
      AssemblyAsserts.AssertReference(def, "mscorlib");
      AssemblyAsserts.AssertReference(def, "System.Drawing");
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");
    }
  }
}
