using Mono.Cecil;

using NUnit.Framework;

namespace BrokenEvent.Shared.ILStripTests
{
  class AssemblyAsserts
  {
    private static TypeDefinition FindType(AssemblyDefinition def, string className)
    {
      foreach (TypeDefinition type in def.MainModule.Types)
      {
        if (type.FullName == className)
          return type;

        foreach (TypeDefinition nestedType in type.NestedTypes)
          if (nestedType.FullName == className)
            return nestedType;
      }

      return null;
    }

    public static void AssertClass(AssemblyDefinition def, string className)
    {
      Assert.IsNotNull(FindType(def, className), "Class " + className + " not found");
    }

    public static void AssertClassPublic(AssemblyDefinition def, string className, bool isPublic = true)
    {
      TypeDefinition type = FindType(def, className);
      Assert.IsNotNull(type, "Class " + className + " not found");

      if (isPublic)
        Assert.True((type.Attributes & (TypeAttributes.Public | TypeAttributes.NestedPublic)) != 0, className + " should be public");
      else
        Assert.True((type.Attributes & (TypeAttributes.Public | TypeAttributes.NestedPublic)) == 0, className + " should not be public");
    }

    public static void AssertNoClass(AssemblyDefinition def, string className)
    {
      Assert.IsNull(FindType(def, className), "Class " + className + " found");
    }

    public static void AssertResource(AssemblyDefinition def, string name)
    {
      foreach (Resource res in def.MainModule.Resources)
        if (res.Name == name)
          return;

      Assert.Fail("Resource " + name + " not found");
    }

    public static void AssertNoResource(AssemblyDefinition def, string name)
    {
      foreach (Resource res in def.MainModule.Resources)
        if (res.Name == name)
          Assert.Fail("Resource " + name + " found");
    }

    public static void AssertReference(AssemblyDefinition def, string name)
    {
      foreach (AssemblyNameReference @ref in def.MainModule.AssemblyReferences)
        if (@ref.Name == name)
          return;

      Assert.Fail("Reference " + name + " not found");
    }

    public static void AssertNoReference(AssemblyDefinition def, string name)
    {
      foreach (AssemblyNameReference @ref in def.MainModule.AssemblyReferences)
        if (@ref.Name == name)
          Assert.Fail("Reference " + name + " found");
    }
  }
}
