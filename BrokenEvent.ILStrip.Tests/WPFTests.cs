using System;
using System.Collections.Generic;

using BrokenEvent.Shared;
using BrokenEvent.Shared.ILStripTests;

using Mono.Cecil;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  [TestFixture]
  class WPFTests
  {
    [Test]
    public void CleanupUnusedClassesTest()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripWPFTestLib.exe"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedReferences();
      strip.CleanupUnusedResources();

      AssemblyDefinition def = AssemblyAsserts.SaveAssembly(strip);
      /*AssemblyAsserts.AssertNoClass(def, "ILStripTest.RegularClass");
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
      AssemblyAsserts.AssertReference(def, "System.Windows.Forms");*/
    }

  }
}
