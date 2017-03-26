using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace BrokenEvent.ILStrip
{
  public class ILStrip
  {
    private readonly AssemblyDefinition definition;
    private List<string> entryPoints = new List<string>();
    private List<TypeDefinition> entryPointTypes = new List<TypeDefinition>();

    private List<TypeDefinition> usedTypes = new List<TypeDefinition>();
    private List<ModuleDefinition> references = new List<ModuleDefinition>();
    private int scanningIndex = 0;
    private List<TypeDefinition> unusedTypes = new List<TypeDefinition>();
    private List<string> makeInternalExclusions = new List<string>();

    public ILStripLogger Logger { get; set; }

    private void Log(string msg)
    {
      if (Logger != null)
        Logger.LogMessage(msg);
    }

    /// <summary>
    /// Create ILStrip object from the Mono.Cecil's <see cref="AssemblyDefinition"/>
    /// </summary>
    /// <param name="definition">Assembly definition to open</param>
    public ILStrip(AssemblyDefinition definition)
    {
      this.definition = definition;
    }

    /// <summary>
    /// Create ILStrip object and load the assembly from file.
    /// </summary>
    /// <param name="filename">File to load assembly from</param>
    public ILStrip(string filename)
    {
      definition = AssemblyDefinition.ReadAssembly(filename);
    }

    /// <summary>
    /// Create ILStrip object and load the assembly from stream
    /// </summary>
    /// <param name="stream">Stream to load assembly from</param>
    public ILStrip(Stream stream)
    {
      definition = AssemblyDefinition.ReadAssembly(stream);
    }

    /// <summary>
    /// List of entry points to start the used classes search
    /// </summary>
    public IList<string> EntryPoints
    {
      get { return entryPoints; }
    }

    /// <summary>
    /// List of exclusions to remain public if <see cref="MakeNotPublic"/> is used
    /// </summary>
    public IList<string> MakeInternalExclusions
    {
      get { return makeInternalExclusions; }
    }

    private void ScanEntryPoints()
    {
      if (definition.EntryPoint != null && definition.EntryPoint.DeclaringType != null)
      {
        Log("Found module entry point: " + definition.EntryPoint.DeclaringType);
        entryPointTypes.Add(definition.EntryPoint.DeclaringType);
      }

      foreach (string entryPoint in entryPoints)
      {
        bool found = false;
        foreach (TypeDefinition type in definition.MainModule.Types)
        {
          if (type.FullName == entryPoint)
          {
            entryPointTypes.Add(type);
            found = true;
            break;
          }

          foreach (TypeDefinition nestedType in type.NestedTypes)
          {
            if (nestedType.FullName == entryPoint)
            {
              entryPointTypes.Add(nestedType);
              found = true;
              break;
            }
          }

          if (found)
            break;
        }

        if (!found)
          throw new ArgumentException("Unable to resolve entry point: " + entryPoint);
      }
    }

    /// <summary>
    /// Analyzes the classes and methods to build list of used classes.
    /// Beginning from the entry points.
    /// </summary>
    public void ScanUsedClasses()
    {
      ScanEntryPoints();

      Log("Scanning for used classes...");

      foreach (TypeDefinition type in entryPointTypes)
        WalkClass(type);

      TypeDefinition def;
      while ((def = PeekUsedType()) != null)
        WalkClass(def);
    }

    private void WalkClass(TypeDefinition type)
    {
      if (!type.HasMethods)
        return;

      TypeDefinition current = type;
      while (true)
      {
        TypeReference baseRef = current.BaseType;
        if (baseRef == null)
          break;
        TypeDefinition baseDef = baseRef.Resolve();
        if (baseDef == null)
          break;

        AddUsedType(baseDef);
        current = baseDef;
      }

      // no properties walk behavior: they're walked as get_%PropName/set_%PropName methods

      foreach (MethodDefinition method in type.Methods)
        WalkMethod(method);

      foreach (GenericParameter parameter in type.GenericParameters)
        AddUsedType(parameter);

      foreach (CustomAttribute attribute in type.CustomAttributes)
        AddUsedType(attribute.AttributeType);

      foreach (TypeReference iface in type.Interfaces)
        WalkClass(iface.Resolve());
    }

    private void WalkMethod(MethodDefinition method)
    {
      if (!method.HasBody)
        return;

      foreach (ParameterDefinition parameter in method.Parameters)
        AddUsedType(parameter.ParameterType);

      AddUsedType(method.ReturnType);

      foreach (Instruction instruction in method.Body.Instructions)
      {
        MethodReference methodRef = instruction.Operand as MethodReference;
        if (methodRef == null)
          continue;

        AddUsedType(methodRef.DeclaringType);
        AddUsedType(methodRef.ReturnType);
        foreach (GenericParameter parameter in methodRef.GenericParameters)
          AddUsedType(parameter.DeclaringType);
        foreach (ParameterDefinition parameter in methodRef.Parameters)
          AddUsedType(parameter.ParameterType);
      }
    }

    private void AddUsedType(TypeReference typeRef)
    {
      if (typeRef == null)
        return;
      
      TypeDefinition typeDef = typeRef.Resolve();
      if (typeDef == null)
        return;

      if (typeDef.Module != definition.MainModule)
      {
        if (!references.Contains(typeDef.Module))
        {
          Log("Reference used: " + typeDef.Module.FullyQualifiedName);
          references.Add(typeDef.Module);
        }
        return;
      }

      if (usedTypes.Contains(typeDef))
        return;

      Log("Type used: " + typeDef);
      usedTypes.Add(typeDef);
    }

    private TypeDefinition PeekUsedType()
    {
      if (scanningIndex >= usedTypes.Count - 1)
        return null;

      return usedTypes[++scanningIndex];
    }

    /// <summary>
    /// Result list of unused types after <see cref="ScanUnusedClasses"/> call
    /// </summary>
    public IList<TypeDefinition> UnusedTypes
    {
      get { return unusedTypes; }
    }

    /// <summary>
    /// Scans all the types and build <see cref="UnusedTypes"/> list of all of them,
    /// except types, found with <see cref="ScanUsedClasses"/>, so be aware to call it before.
    /// </summary>
    public void ScanUnusedClasses()
    {
      Log("Scanning for unused classes...");

      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        if (type.Name == "<Module>")
          continue; // hack

        bool isNestedUsed = false;
        foreach (TypeDefinition nestedType in type.NestedTypes)
          if (usedTypes.Contains(nestedType) || entryPointTypes.Contains(nestedType))
          {
            isNestedUsed = true;
            break;
          }
          else
          {
            Log("Type unused: " + nestedType);
            unusedTypes.Add(nestedType);
          }

        if (isNestedUsed)
          continue;

        if (entryPointTypes.Contains(type))
          continue;

        if (usedTypes.Contains(type))
          continue;

        // greater hack
        bool isCompilerGenerated = false;
        foreach (CustomAttribute attribute in type.CustomAttributes)
          if (attribute.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")
          {
            isCompilerGenerated = true;
            break;
          }

        if (isCompilerGenerated)
          continue;

        Log("Type unused: " + type);
        unusedTypes.Add(type);
      }
    }

    /// <summary>
    /// Cleans all class-related resources. Uses <see cref="UnusedTypes"/> list, so be
    /// aware to call <see cref="ScanUnusedClasses"/> before.
    /// </summary>
    public void CleanupUnusedResources()
    {
      Log("Cleaning up unused resources...");

      int i = 0;
      while (i < definition.MainModule.Resources.Count)
      {
        Resource resource = definition.MainModule.Resources[i];
        bool shouldClean = false;
        foreach (TypeDefinition type in unusedTypes)
          if (resource.Name == type.FullName + ".resources")
          {
            shouldClean = true;
            break;
          }

        if (shouldClean)
        {
          Log("Resource unused: " + definition.MainModule.Resources[i].Name);
          definition.MainModule.Resources.RemoveAt(i);
        }
        else
          i++;
      }
    }

    /// <summary>
    /// Makes all types, except the <see cref="MakeInternalExclusions"/> list, internal
    /// </summary>
    public void MakeNotPublic()
    {
      Log("Adjusting access modifiers...");

      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        if (type.Name == "<Module>")
          continue; // hack

        // greater hack
        bool isCompilerGenerated = false;
        foreach (CustomAttribute attribute in type.CustomAttributes)
          if (attribute.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")
          {
            isCompilerGenerated = true;
            break;
          }

        if (isCompilerGenerated)
          continue;

        if ((type.Attributes & TypeAttributes.Public) != 0 &&
            (type.Attributes & TypeAttributes.NestedPublic) == 0 &&
            !makeInternalExclusions.Contains(type.FullName))
        {
          type.Attributes &= ~TypeAttributes.Public;
          Log("Adjusted access modifier: " + type.FullName);
        }
      }
    }

    private void AssertAction(bool result, string message)
    {
      if (!result)
        throw new Exception(message);
    }

    /// <summary>
    /// Remove all unused classes from the assembly. Uses <see cref="UnusedTypes"/> list,
    /// so be aware to call <see cref="ScanUnusedClasses"/> before.
    /// </summary>
    public void CleanupUnusedClasses()
    {
      Log("Cleaning up unused classes...");

      List<TypeDefinition> nestedUnusedTypes = new List<TypeDefinition>();

      foreach (TypeDefinition type in unusedTypes)
        if (type.IsNested)
          nestedUnusedTypes.Add(type);
        else
          AssertAction(definition.MainModule.Types.Remove(type), "Failed to remove type: " + type);

      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        List<TypeDefinition> nestedTypesToRemove = null;
        foreach (TypeDefinition nestedType in type.NestedTypes)
          if (nestedUnusedTypes.Contains(nestedType))
          {
            if (nestedTypesToRemove == null)
              nestedTypesToRemove = new List<TypeDefinition>();
            nestedTypesToRemove.Add(nestedType);
          }

        if (nestedTypesToRemove != null)
          foreach (TypeDefinition nestedType in nestedTypesToRemove)
            AssertAction(type.NestedTypes.Remove(nestedType), "Failed to remove nested type: " + nestedType);
      }
    }

    /// <summary>
    /// Remove all unused references from the assembly. Usage list is built during <see cref="ScanUsedClasses"/>,
    /// so be aware to call it before
    /// </summary>
    public void CleanupUnusedReferences()
    {
      Log("Cleaning up unused references...");

      int i = 0;
      while (i < definition.MainModule.AssemblyReferences.Count)
      {
        bool shouldClean = true;

        foreach (ModuleDefinition reference in references)
          if (reference.Assembly.FullName == definition.MainModule.AssemblyReferences[i].FullName)
          {
            shouldClean = false;
            break;
          }

        if (shouldClean)
        {
          Log("Reference unused: " + definition.MainModule.AssemblyReferences[i].Name);
          definition.MainModule.AssemblyReferences.RemoveAt(i);
        }
        else
          i++;
      }
    }

    /// <summary>
    /// Save the result assembly to file
    /// </summary>
    /// <param name="filename">Path of file to save to</param>
    public void Save(string filename)
    {
      definition.Write(filename);
    }

    /// <summary>
    /// Save the result assembly to stream
    /// </summary>
    /// <param name="stream">Stream to save to</param>
    public void Save(Stream stream)
    {
      definition.Write(stream);
    }
  }
}
