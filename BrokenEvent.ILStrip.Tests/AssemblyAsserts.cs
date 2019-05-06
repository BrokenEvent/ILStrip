using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

using Mono.Cecil;

using NUnit.Framework;

namespace BrokenEvent.ILStrip.Tests
{
  class AssemblyAsserts
  {
    private readonly AssemblyDefinition definition;
    private Dictionary<string, TypeDefinition> types = new Dictionary<string, TypeDefinition>();
    private HashSet<string> resources = new HashSet<string>();

    public AssemblyAsserts(ILStrip strip)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        strip.Save(ms);
        ms.Position = 0;
        definition = AssemblyDefinition.ReadAssembly(ms);
      }

      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        types.Add(type.FullName, type);

        foreach (TypeDefinition nestedType in type.NestedTypes)
          types.Add(nestedType.FullName, nestedType);
      }

      string targetName = $"{definition.Name.Name}.g.resources";
      EmbeddedResource wpfRoot = null;
      foreach (Resource resource in definition.MainModule.Resources)
      {
        if (resource.Name == targetName)
          wpfRoot = resource as EmbeddedResource;

        if (wpfRoot != null)
          break;
      }

      if (wpfRoot == null)
        return;

      using (Stream stream = wpfRoot.GetResourceStream())
      {
        using (ResourceReader reader = new ResourceReader(stream))
          foreach (DictionaryEntry entry in reader)
            resources.Add((string)entry.Key);
      }
    }

    public void AssertClass(string className, ClassModifier modifier = ClassModifier.DontCare)
    {
      TypeDefinition type;
      Assert.IsTrue(types.TryGetValue(className, out type), "Class " + className + " not found");

      switch (modifier)
      {
        case ClassModifier.Public:
          Assert.True((type.Attributes & (TypeAttributes.Public | TypeAttributes.NestedPublic)) != 0, className + " should be public");
          break;

        case ClassModifier.Internal:
          Assert.True((type.Attributes & (TypeAttributes.Public | TypeAttributes.NestedPublic)) == 0, className + " should not be public");
          break;
      }
    }

    public void AssertNoClass(string className)
    {
      Assert.IsTrue(!types.ContainsKey(className), "Class " + className + " found");
    }

    public void AssertResource(string name)
    {
      foreach (Resource res in definition.MainModule.Resources)
        if (res.Name == name)
          return;

      Assert.Fail("Resource " + name + " not found");
    }

    public void AssertNoResource(string name)
    {
      foreach (Resource res in definition.MainModule.Resources)
        if (res.Name == name)
          Assert.Fail("Resource " + name + " found");
    }

    public void AssertReference(string name)
    {
      foreach (AssemblyNameReference @ref in definition.MainModule.AssemblyReferences)
        if (@ref.Name == name)
          return;

      Assert.Fail("Reference " + name + " not found");
    }

    public void AssertNoReference(string name)
    {
      foreach (AssemblyNameReference @ref in definition.MainModule.AssemblyReferences)
        if (@ref.Name == name)
          Assert.Fail("Reference " + name + " found");
    }

    public void AssertWpfResource(string name)
    {
      Assert.True(resources.Contains(name), "WPF resource " + name + " not found");
    }

    public void AssertNoWpfResource(string name)
    {
      Assert.True(!resources.Contains(name), "WPF resource " + name + " found");
    }
  }

  enum ClassModifier
  {
    DontCare,
    Public,
    Internal
  }
}
