using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace BrokenEvent.ILStrip
{
  public class ILStrip
  {
    private readonly AssemblyDefinition definition;
    private List<string> entryPoints = new List<string>();
    private List<TypeDefinition> entryPointTypes = new List<TypeDefinition>();

    private List<TypeDefinition> usedTypes = new List<TypeDefinition>();
    private List<ModuleDefinition> references = new List<ModuleDefinition>();
    private List<ModuleReference> unmanagedReferences = new List<ModuleReference>();
    private List<TypeDefinition> unusedTypes = new List<TypeDefinition>();
    private List<string> makeInternalExclusions = new List<string>();
    private List<string> unusedResourceExclusions = new List<string>();
    private List<string> removeAttributesNamespaces = new List<string>();

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

    /// <summary>
    /// List of exclusions in resources to be cleaned up with <see cref="CleanupUnusedResources"/>.
    /// </summary>
    public IList<string> UnusedResourceExclusions
    {
      get { return unusedResourceExclusions; }
    }

    /// <summary>
    /// List of namespaces of the custom attributes to be removed.
    /// </summary>
    /// <remarks>Removal occurs in <see cref="ScanUsedClasses"/> so the list should be filled before this call.</remarks>
    public IList<string> RemoveAttributesNamespaces
    {
      get { return removeAttributesNamespaces; }
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

      for (int i = 0; i < usedTypes.Count; i++)
        WalkClass(usedTypes[i]);
    }

    private void WalkCustomAttributes(Collection<CustomAttribute> customAttributes)
    {
      int i = 0;
      while (i < customAttributes.Count)
      {
        CustomAttribute attribute = customAttributes[i];
        if (removeAttributesNamespaces.Contains(attribute.AttributeType.Namespace))
          customAttributes.RemoveAt(i);
        else
        {
          AddUsedType(attribute.AttributeType);
          i++;
        }
      }
    }

    private void WalkClass(TypeDefinition type)
    {
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

      WalkCustomAttributes(type.CustomAttributes);

      foreach (TypeReference iface in type.Interfaces)
        AddUsedType(iface.Resolve());

      foreach (FieldDefinition field in type.Fields)
      {
        WalkCustomAttributes(field.CustomAttributes);
        AddUsedType(field.FieldType);
      }
    }

    private void WalkMethod(MethodDefinition method)
    {
      WalkCustomAttributes(method.CustomAttributes);

      if ((method.Attributes & MethodAttributes.PInvokeImpl) != 0)
      {
        ModuleReference module = method.PInvokeInfo.Module;
        if (!unmanagedReferences.Contains(module))
        {
          Log("Native reference used: " + module.Name);
          unmanagedReferences.Add(module);
        }
      }

      foreach (GenericParameter parameter in method.GenericParameters)
        AddUsedType(parameter.DeclaringType);

      foreach (ParameterDefinition parameter in method.Parameters)
      {
        AddUsedType(parameter.ParameterType);
        WalkCustomAttributes(parameter.CustomAttributes);
      }

      AddUsedType(method.ReturnType);

      if (!method.HasBody)
        return;

      foreach (VariableDefinition variable in method.Body.Variables)
        AddUsedType(variable.VariableType);

      foreach (Instruction instruction in method.Body.Instructions)
      {
        MethodReference methodRef = instruction.Operand as MethodReference;
        if (methodRef != null)
        {
          AddUsedType(methodRef.DeclaringType);
          AddUsedType(methodRef.ReturnType);

          GenericInstanceMethod genericMethod = methodRef as GenericInstanceMethod;
          if (genericMethod != null)
            foreach (TypeReference parameter in genericMethod.GenericArguments)
              AddUsedType(parameter);

          foreach (ParameterDefinition parameter in methodRef.Parameters)
            AddUsedType(parameter.ParameterType);
          continue;
        }

        TypeReference typeRef = instruction.Operand as TypeReference;
        if (typeRef != null)
        {
          foreach (GenericParameter genericParameter in typeRef.GenericParameters)
            AddUsedType(genericParameter);
          AddUsedType(typeRef.DeclaringType);
          continue;
        }

        FieldReference fieldRef = instruction.Operand as FieldReference;
        if (fieldRef != null)
        {
          AddUsedType(fieldRef.FieldType);
          AddUsedType(fieldRef.DeclaringType);
        }
      }
    }

    private void AddUsedType(TypeReference typeRef)
    {
      if (typeRef == null)
        return;

      GenericInstanceType genericType = typeRef as GenericInstanceType;
      if (genericType != null)
        foreach (TypeReference reference in genericType.GenericArguments)
          AddUsedType(reference);

      TypeDefinition typeDef = typeRef.Resolve();
      if (typeDef == null)
        return;

      foreach (GenericParameter parameter in typeDef.GenericParameters)
        AddUsedType(parameter);

      if (typeDef.Module != definition.MainModule)
      {
        if (!references.Contains(typeDef.Module))
        {
          Log("Reference used: " + typeDef.Module.FullyQualifiedName);
          references.Add(typeDef.Module);
        }
        return;
      }

      foreach (CustomAttribute attribute in typeDef.CustomAttributes)
        AddUsedType(attribute.AttributeType);

      if (usedTypes.Contains(typeDef))
        return;

      Log("Type used: " + typeDef);
      usedTypes.Add(typeDef);
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

        if (type.Name == "<PrivateImplementationDetails>")
          continue; // double hash

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
    /// Cleans all unused resources. Resource will remain in assembly if it is class-related resource (MyClassName.resource) of the used class
    /// (be aware to run <see cref="ScanUsedClasses"/> before) or is in <see cref="UnusedResourceExclusions"/> list.
    /// </summary>
    public void CleanupUnusedResources()
    {
      Log("Cleaning up unused resources...");

      int i = 0;
      while (i < definition.MainModule.Resources.Count)
      {
        Resource resource = definition.MainModule.Resources[i];

        bool shouldClean = true;
        foreach (TypeDefinition type in usedTypes)
          if (resource.Name == type.FullName + ".resources")
          {
            shouldClean = false;
            Log("Resource used: " + resource.Name + " (used class)");
            break;
          }
        if (shouldClean)
        {
          if (unusedResourceExclusions.Contains(resource.Name))
          {
            shouldClean = false;
            Log("Resource used: " + resource.Name + " (exclusion)");
          }
        }

        if (shouldClean)
        {
          Log("Resource unused: " + resource.Name);
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
          type.Attributes &= ~(TypeAttributes.Public | TypeAttributes.NestedPublic);
          Log("Adjusted access modifier: " + type.FullName);
        }
      }
    }

    private static void AssertAction(bool result, string message)
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
        AssemblyNameReference assemblyRef = definition.MainModule.AssemblyReferences[i];

        foreach (ModuleDefinition reference in references)
          if (reference.Assembly.FullName == assemblyRef.FullName)
          {
            shouldClean = false;
            break;
          }

        if (shouldClean)
        {
          Log("Reference unused: " + assemblyRef.Name);
          definition.MainModule.AssemblyReferences.RemoveAt(i);
        }
        else
          i++;
      }

      i = 0;
      while (i < definition.MainModule.ModuleReferences.Count)
      {
        bool shouldClean = true;
        ModuleReference moduleRef = definition.MainModule.ModuleReferences[i];

        foreach (ModuleReference reference in unmanagedReferences)
          if (reference.Name == moduleRef.Name)
          {
            shouldClean = false;
            break;
          }

        if (shouldClean)
        {
          Log("Module reference unused: " + moduleRef.Name);
          definition.MainModule.ModuleReferences.RemoveAt(i);
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

    /// <summary>
    /// Get string list of all the type names in currently loaded assembly
    /// </summary>
    /// <param name="separator">Separator string between types</param>
    /// <returns>List of all the typenames</returns>
    public string GetAllTypesList(string separator = "\r\n")
    {
      return GetAllTypesList(definition, separator);
    }

    /// <summary>
    /// Get string list of names all used types in currently loaded assembly
    /// </summary>
    /// <param name="separator">Separator string between types</param>
    /// <returns>List of all uses typenames</returns>
    public string GetUsedTypesList(string separator = "\r\n")
    {
      return BuildTypesList(unusedTypes, separator);
    }

    /// <summary>
    /// Get string list of all types in assembly
    /// </summary>
    /// <param name="definition">Assembly to list types</param>
    /// <param name="separator">Separator string between types</param>
    /// <returns>List of all the typenames</returns>
    public static string GetAllTypesList(AssemblyDefinition definition, string separator = "\r\n")
    {
      List<TypeDefinition> types = new List<TypeDefinition>();
      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        types.Add(type);
        foreach (TypeDefinition nestedType in type.NestedTypes)
          types.Add(nestedType);
      }

      return BuildTypesList(types, separator);
    }

    private static string BuildTypesList(IEnumerable<TypeDefinition> list, string separator = "\r\n")
    {
      List<string> typeNames = new List<string>();
      foreach (TypeDefinition type in list)
        typeNames.Add(type.FullName);

      typeNames.Sort();

      return string.Join(separator, typeNames);
    }
  }
}
