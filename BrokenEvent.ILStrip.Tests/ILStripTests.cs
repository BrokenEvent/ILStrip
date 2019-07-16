using BrokenEvent.Shared;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  /*
   * EmptyClass - no usages
   * AttributedClass - uses CustomAttribute
   * ClassWithAttributedField - uses CustomAttribute
   * ClassWithAttributedProperty - uses CustomAttribute
   * ClassWithTypeRefAttributeProperty - uses TypeRefAttribute and EmptyClass
   * UserClass - uses EmptyClass, AttributedClass and IInterface
   * ClassWithGeneric - uses AttributedClass and EmptyClass
   * ClassWithNestedClass - uses ClassWithNestedClass/NestedClass
   * ClassWithNestedClass - no usages
   * ClassWithEvents - uses EmptyClass
   * IInterface - no usages
   * CustomAttribute - no usages
   * TypeRefAttribute - no usages
   * XmlUsingClass - no usages, but uses System.Xml reference
   */
  [TestFixture]
  class ILStripTests
  {
    [Test]
    public void NoChange()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.AttributedClass");
      asserts.AssertClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertClass("ILStripTest.UserClass");
      asserts.AssertClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithEvents");
      asserts.AssertClass("ILStripTest.IInterface");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void CleanupUnusedClassesAll()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void EntryPoints()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.UserClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.UserClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertClass("ILStripTest.IInterface");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void AttributeToField()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithAttributedField");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void AttributeToProperty()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithAttributedProperty");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void TypeRefAttribute()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithTypeRefAttributeProperty");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void Events()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithEvents");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void CleanupUnusedReferencesAll()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
      asserts.AssertReference("mscorlib"); // can't be removed
      asserts.AssertNoReference("System.Xml");
    }

    [Test]
    public void CleanupUnusedReferencesUsed()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.XmlUsingClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.CleanupUnusedReferences();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertClass("ILStripTest.XmlUsingClass");
      asserts.AssertReference("mscorlib"); // can't be removed
      asserts.AssertReference("System.Xml");
    }

    [Test]
    public void CleanupUusedNestedClasses()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void CleanupUnusedNestedClasses()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void CleanupUnusedClassesButUseNested()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass2/NestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertNoClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void CleanupUnusedClassesWithGenerics()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithGeneric`1");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.AttributedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void MakeInternal()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.UserClass");
      strip.EntryPoints.Add("ILStripTest.ClassWithNestedClass");
      strip.MakeInternalExclusions.Add("ILStripTest.EmptyClass");
      strip.MakeInternalExclusions.Add("ILStripTest.ClassWithNestedClass");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();
      strip.MakeInternal();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertClass("ILStripTest.UserClass", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.EmptyClass", ClassModifier.Public);
      asserts.AssertClass("ILStripTest.AttributedClass", ClassModifier.Internal);
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertNoClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertClass("ILStripTest.ClassWithNestedClass", ClassModifier.Public);
      asserts.AssertClass("ILStripTest.ClassWithNestedClass/NestedClass", ClassModifier.Public);
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertClass("ILStripTest.IInterface", ClassModifier.Internal);
      asserts.AssertClass("ILStripTest.CustomAttribute", ClassModifier.Internal);
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }

    [Test]
    public void RemoveAttributes()
    {
      ILStrip strip = new ILStrip(TestHelper.TranslatePath("ILStripTestLib.dll"));
      strip.EntryPoints.Add("ILStripTest.ClassWithAttributedField");
      strip.EntryPoints.Add("ILStripTest.ClassWithAttributedProperty");
      strip.EntryPoints.Add("ILStripTest.AttributedClass");
      strip.RemoveAttributesNamespaces.Add("ILStripTest");

      strip.ScanUsedClasses();
      strip.ScanUnusedClasses();
      strip.CleanupUnusedClasses();

      AssemblyAsserts asserts = new AssemblyAsserts(strip);
      asserts.AssertNoClass("ILStripTest.UserClass");
      asserts.AssertNoClass("ILStripTest.EmptyClass");
      asserts.AssertClass("ILStripTest.AttributedClass");
      asserts.AssertClass("ILStripTest.ClassWithAttributedField");
      asserts.AssertClass("ILStripTest.ClassWithAttributedProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithTypeRefAttributeProperty");
      asserts.AssertNoClass("ILStripTest.ClassWithGeneric`1");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2");
      asserts.AssertNoClass("ILStripTest.ClassWithNestedClass2/NestedClass");
      asserts.AssertNoClass("ILStripTest.ClassWithEvents");
      asserts.AssertNoClass("ILStripTest.IInterface");
      asserts.AssertNoClass("ILStripTest.CustomAttribute");
      asserts.AssertNoClass("ILStripTest.XmlUsingClass");
    }
  }
}
