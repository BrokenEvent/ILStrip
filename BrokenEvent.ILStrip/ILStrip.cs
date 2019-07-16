using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

using Confuser.Renamer.BAML;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace BrokenEvent.ILStrip
{
  public partial class ILStrip: IDisposable
  {
    private readonly AssemblyDefinition definition;
    private readonly string wpfRootResourceName;
    private readonly string wpfPathPrefix;
    private readonly string resourceManagerResourceName;

    private HashSet<string> entryPoints = new HashSet<string>();
    private HashSet<string> entryPointBamls = new HashSet<string>();

    private HashSet<TypeDefinition> usedTypesCache = new HashSet<TypeDefinition>();
    private Queue<TypeDefinition> typesToScan = new Queue<TypeDefinition>();

    private List<TypeDefinition> unusedTypes = new List<TypeDefinition>();

    private HashSet<ResourcePart> usedBamlsCache = new HashSet<ResourcePart>();
    private Queue<ResourcePart> bamlsToScan = new Queue<ResourcePart>();
    private HashSet<string> usedWpfResources = new HashSet<string>();

    private Dictionary<string, TypeDefinition> typesMap = new Dictionary<string, TypeDefinition>();
    private Dictionary<TypeDefinition, ResourcePart> bamlMap = new Dictionary<TypeDefinition, ResourcePart>();

    private HashSet<ModuleDefinition> usedReferences = new HashSet<ModuleDefinition>();
    private HashSet<ModuleReference> usedUnmanagedReferences = new HashSet<ModuleReference>();
    private HashSet<string> makeInternalExclusions = new HashSet<string>();
    private HashSet<string> unusedResourceExclusions = new HashSet<string>();
    private HashSet<string> unusedWpfResourceExclusions = new HashSet<string>();
    private HashSet<string> removeAttributesNamespaces = new HashSet<string>();
    private bool removeUnknownResources = false;

    private EmbeddedResource wpfRootResource;
    private Dictionary<string, ResourcePart> wpfRootParts;

    /// <summary>
    /// Creates ILStrip instance from the Mono.Cecil's <see cref="AssemblyDefinition"/>.
    /// </summary>
    /// <param name="definition">Assembly definition to open.</param>
    public ILStrip(AssemblyDefinition definition)
    {
      this.definition = definition;

      wpfRootResourceName = $"{definition.Name.Name}.g.resources";
      wpfPathPrefix = $"/{definition.Name.Name};component/";
      resourceManagerResourceName = $"{definition.Name.Name}.Properties.Resources.resources";
    }

    /// <summary>
    /// Creates ILStrip instance and load the assembly from file.
    /// </summary>
    /// <param name="filename">File to load assembly from.</param>
    public ILStrip(string filename)
      : this(AssemblyDefinition.ReadAssembly(filename))
    {
    }

    /// <summary>
    /// Creates ILStrip instance and load the assembly from stream.
    /// </summary>
    /// <param name="stream">Stream to load assembly from.</param>
    public ILStrip(Stream stream)
      : this(AssemblyDefinition.ReadAssembly(stream))
    {
    }

    private void Log(string msg)
    {
      if (Logger != null)
        Logger.LogMessage(msg);
    }

    private void WalkCustomAttributes(IList<CustomAttribute> customAttributes)
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

          foreach (CustomAttributeArgument argument in attribute.ConstructorArguments)
          {
            AddUsedType(argument.Type);
            TypeReference valueRef = argument.Value as TypeReference;
            if (valueRef != null)
              AddUsedType(valueRef);
          }

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

      // properties code will be walked as get_%PropName/set_%PropName methods
      foreach (PropertyDefinition property in type.Properties)
        WalkCustomAttributes(property.CustomAttributes);

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
        if (!usedUnmanagedReferences.Contains(module))
        {
          Log("Native reference used: " + module.Name);
          usedUnmanagedReferences.Add(module);
        }
      }

      foreach (GenericParameter parameter in method.GenericParameters)
      {
        WalkCustomAttributes(parameter.CustomAttributes);
        AddUsedType(parameter.DeclaringType);
      }

      foreach (ParameterDefinition parameter in method.Parameters)
      {
        WalkCustomAttributes(parameter.CustomAttributes);
        AddUsedType(parameter.ParameterType);
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

    private void WalkBaml(ResourcePart part)
    {
      Dictionary<string, string> namespaces = new Dictionary<string, string>();

      foreach (BamlRecord record in part.Baml)
      {
        if (record.Type == BamlRecordType.TypeInfo)
        {
          TypeInfoRecord typeInfo = (TypeInfoRecord)record;

          TypeDefinition type;
          if (typesMap.TryGetValue(typeInfo.TypeFullName, out type))
            AddUsedType(type);

          continue;
        }

        if (record.Type == BamlRecordType.XmlnsProperty)
        {
          XmlnsPropertyRecord ns = (XmlnsPropertyRecord)record;
          const string CLR_NAMESPACE = "clr-namespace:";
          if (ns.XmlNamespace.StartsWith(CLR_NAMESPACE))
            namespaces.Add(ns.Prefix, ns.XmlNamespace.Substring(CLR_NAMESPACE.Length));

          continue;
        }

        if (record.Type == BamlRecordType.Text)
        {
          TextRecord textRecord = (TextRecord)record;

          int index = textRecord.Value.IndexOf(':');
          if (index == -1)
            continue; // not ns reference

          string name;
          if (!namespaces.TryGetValue(textRecord.Value.Substring(0, index), out name))
            continue;

          name += "." + textRecord.Value.Substring(index + 1);

          TypeDefinition type;

          // type as string?
          if (typesMap.TryGetValue(name, out type))
          {
            AddUsedType(type);
            continue;
          }

          index = name.LastIndexOf(".", StringComparison.InvariantCulture);

          // member?
          if (typesMap.TryGetValue(name.Substring(0, index), out type))
            AddUsedType(type);

          continue;
        }

        if (record.Type == BamlRecordType.PropertyWithConverter)
        {
          PropertyWithConverterRecord propertyInfo = (PropertyWithConverterRecord)record;

          string resourceName = propertyInfo.Value;

          if (resourceName.StartsWith(wpfPathPrefix))
            resourceName = resourceName.Substring(wpfPathPrefix.Length, resourceName.Length - wpfPathPrefix.Length);

          if (resourceName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
            resourceName = resourceName.Substring(0, resourceName.Length - 4) + "baml";

          resourceName = resourceName.ToLower();

          ResourcePart resourcePart;
          if (!wpfRootParts.TryGetValue(resourceName, out resourcePart))
            continue;

          if (resourcePart.Baml != null)
            AddUsedBaml(resourcePart);
          else
          {
            Log($"WPF resource used: {resourceName}");
            usedWpfResources.Add(resourceName);
          }
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
        if (!usedReferences.Contains(typeDef.Module))
          usedReferences.Add(typeDef.Module);
        return;
      }

      WalkCustomAttributes(typeDef.CustomAttributes);

      if (usedTypesCache.Contains(typeDef))
        return;

      Log($"Type used: {typeDef}");
      typesToScan.Enqueue(typeDef);
      usedTypesCache.Add(typeDef);
      AddUsedBaml(typeDef);
    }

    private void AddUsedBaml(TypeDefinition typeDef)
    {
      ResourcePart resourcePart;
      if (bamlMap.TryGetValue(typeDef, out resourcePart))
        AddUsedBaml(resourcePart);
    }

    private void AddUsedBaml(ResourcePart resourcePart)
    {
      if (usedBamlsCache.Contains(resourcePart))
        return;

      Log($"BAML used: {resourcePart.Name}");
      bamlsToScan.Enqueue(resourcePart);
      usedBamlsCache.Add(resourcePart);
    }

    private static void AssertAction(bool result, string message)
    {
      if (!result)
        throw new Exception(message);
    }

    private void BuildTypesMap()
    {
      Log("Building types map...");
      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        typesMap.Add(type.FullName, type);

        foreach (TypeDefinition nestedType in type.NestedTypes)
          typesMap.Add(nestedType.FullName, nestedType);
      }
    }

    private void ScanEntryPoints()
    {
      if (definition.EntryPoint != null && definition.EntryPoint.DeclaringType != null)
      {
        Log($"Found module entry point: {definition.EntryPoint.DeclaringType}");
        AddUsedType(definition.EntryPoint.DeclaringType);
      }

      foreach (string entryPoint in entryPoints)
      {
        TypeDefinition type;

        if (!typesMap.TryGetValue(entryPoint, out type))
          throw new ArgumentException($"Unable to resolve class entry point: {entryPoint}");

        usedTypesCache.Add(type);
        typesToScan.Enqueue(type);
        AddUsedBaml(type);
      }

      foreach (string entryPoint in entryPointBamls)
      {
        ResourcePart resourcePart;
        if (wpfRootParts == null || !wpfRootParts.TryGetValue(entryPoint, out resourcePart))
          throw new ArgumentException($"Unable to reslove BAML entry point: {entryPoint}");

        usedBamlsCache.Add(resourcePart);
        bamlsToScan.Enqueue(resourcePart);
      }
    }

    private void ScanWpfRoot()
    {
      foreach (Resource resource in definition.MainModule.Resources)
      {
        if (resource.Name == wpfRootResourceName)
          wpfRootResource = resource as EmbeddedResource;

        if (wpfRootResource != null)
          break;
      }

      if (wpfRootResource == null)
        return;

      Log($"Found WPF root resource: {wpfRootResourceName}");

      wpfRootParts = new Dictionary<string, ResourcePart>();

      using (Stream stream = wpfRootResource.GetResourceStream())
      {
        using (ResourceReader reader = new ResourceReader(stream))
          foreach (DictionaryEntry entry in reader)
          {
            ResourcePart part = new ResourcePart(entry);

            if (part.TypeName != null)
            {
              TypeDefinition type;
              if (typesMap.TryGetValue(part.TypeName, out type))
              {
                part.TypeDef = type;
                bamlMap.Add(type, part);
              }
            }

            wpfRootParts.Add(part.Name, part);
          }
      }
    }

    private Resource RebuildWpfRoot()
    {
      using (MemoryStream stream = new MemoryStream())
      {
        using (ResourceWriter writer = new ResourceWriter(stream))
        {
          int count = 0;
          foreach (KeyValuePair<string, ResourcePart> pair in wpfRootParts)
          {
            if (!unusedWpfResourceExclusions.Contains(pair.Value.Name))
            {
              if (pair.Value.Baml != null)
              {
                if (!usedBamlsCache.Contains(pair.Value))
                {
                  Log($"Cleaned up unused BAML: {pair.Key}");
                  continue;
                }
              }
              else if (!usedWpfResources.Contains(pair.Value.Name) && removeUnknownResources)
              {
                Log($"Cleaned up unused WPF resource: {pair.Key}");
                continue;
              }
            }

            writer.AddResource(pair.Key, pair.Value.Stream, false);
            count++;
          }

          if (count > 0)
            writer.Generate();
        }

        return new EmbeddedResource(wpfRootResource.Name, wpfRootResource.Attributes, stream.ToArray());
      }
    }

    private bool CheckIfResourceShouldRemain(Resource resource, int i)
    {
      // exclusion
      if (unusedResourceExclusions.Contains(resource.Name))
      {
        Log($"Resource used: {resource.Name} (exclusion)");
        return true;
      }

      // WPF root resource
      if (resource == wpfRootResource)
      {
        definition.MainModule.Resources[i] = RebuildWpfRoot();
        return true;
      }

      // Resource manager resource
      if (resource.Name == resourceManagerResourceName)
        return true;

      const string RES_SUFFIX = ".resources";

      // class resource?
      if (resource.Name.EndsWith(RES_SUFFIX))
      {
        string typeName = resource.Name.Substring(0, resource.Name.Length - RES_SUFFIX.Length);

        TypeDefinition type;
        if (typesMap.TryGetValue(typeName, out type))
        {
          // used?
          if (usedTypesCache.Contains(type))
          {
            Log($"Resource used: {resource.Name} (used class)");
            return true;
          }

          // surely unused?
          if (unusedTypes.Contains(type))
            return false;
        }
      }

      return !removeUnknownResources;
    }

    /// <summary>
    /// Gets or sets the logger to log ILStrip activity.
    /// </summary>
    public ILStripLogger Logger { get; set; }

    /// <summary>
    /// Gets the list of entry points to start the used classes search.
    /// </summary>
    /// <example>
    /// <para>Use namespace-qualified names for classes.</para>
    /// <para>Example of class: <c>MyNamespace.MyClass</c></para>
    /// <para>Example of nested class: <c>MyNamespace.MyClass/MyNestedClass</c></para>
    /// </example>
    public HashSet<string> EntryPoints
    {
      get { return entryPoints; }
    }

    /// <summary>
    /// Gets the list of exclusions to remain public if <see cref="MakeInternal"/> is used.
    /// </summary>
    /// <example>
    /// <para>Use namespace-qualified names for classes.</para>
    /// <para>Example of class: <c>MyNamespace.MyClass</c></para>
    /// </example>
    public HashSet<string> MakeInternalExclusions
    {
      get { return makeInternalExclusions; }
    }

    /// <summary>
    /// Gets the list of exclusions in resources to be cleaned up with <see cref="CleanupUnusedResources"/>.
    /// </summary>
    public HashSet<string> UnusedResourceExclusions
    {
      get { return unusedResourceExclusions; }
    }

    /// <summary>
    /// Gets the list of exlcusions in WPF resources.
    /// </summary>
    public HashSet<string> UnusedWpfResourceExclusions
    {
      get { return unusedWpfResourceExclusions; }
    }

    /// <summary>
    /// Gets the list of namespaces of the custom attributes to be removed.
    /// </summary>
    /// <remarks>Removal occurs in <see cref="ScanUsedClasses"/> so the list should be filled before this call.</remarks>
    public HashSet<string> RemoveAttributesNamespaces
    {
      get { return removeAttributesNamespaces; }
    }

    /// <summary>
    /// Gets the list of BAML entry points used in search.
    /// </summary>
    /// <example>
    /// <para>Use paths instead of namespaces and the lowecase. BAML name is the same as XAML in project but with different extension.</para>
    /// <para>Example of BAML name: <c>ui/mainwindow.baml</c></para>
    /// </example>
    public HashSet<string> EntryPointBamls
    {
      get { return entryPointBamls; }
    }

    /// <summary>
    /// Gets the result list of unused types after <see cref="ScanUnusedClasses"/> call.
    /// </summary>
    public IList<TypeDefinition> UnusedTypes
    {
      get { return unusedTypes; }
    }

    /// <summary>
    /// Gets or sets the value indicating whether to remove all resources which usage is unknown.
    /// </summary>
    /// <remarks>
    /// <para>Be very careful when enabling this option. In this case ILStrip will
    /// remove any resource (embedded resources or WPF resources) which it doesn't know.</para>
    /// <para>When using this option you should be certain that <see cref="UnusedWpfResourceExclusions"/>
    /// and <see cref="UnusedResourceExclusions"/> are set properly.</para>
    /// </remarks>
    public bool RemoveUnknownResources
    {
      get { return removeUnknownResources; }
      set { removeUnknownResources = value; }
    }

    /// <summary>
    /// Analyzes the classes and methods to build list of used classes.
    /// Beginning from the entry points.
    /// </summary>
    public void ScanUsedClasses()
    {
      BuildTypesMap();
      ScanWpfRoot();
      ScanEntryPoints();

      Log("Scanning for used classes...");

      while (typesToScan.Count > 0 || bamlsToScan.Count > 0)
      {
        if (typesToScan.Count > 0)
          WalkClass(typesToScan.Dequeue());

        if (bamlsToScan.Count > 0)
          WalkBaml(bamlsToScan.Dequeue());
      }
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
          if (usedTypesCache.Contains(nestedType))
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

        if (usedTypesCache.Contains(type))
          continue;

        Log("Type unused: " + type);
        unusedTypes.Add(type);
      }
    }

    /// <summary>
    /// Cleans all unused resources.
    /// </summary>
    public void CleanupUnusedResources()
    {
      Log("Cleaning up unused resources...");

      int i = 0;
      while (i < definition.MainModule.Resources.Count)
      {
        Resource resource = definition.MainModule.Resources[i];

        if (CheckIfResourceShouldRemain(resource, i))
        {
          i++;
          continue;
        }

        Log("Resource removed: " + resource.Name);
        definition.MainModule.Resources.RemoveAt(i);
      }
    }

    /// <summary>
    /// Makes all types, except the <see cref="MakeInternalExclusions"/> list, internal.
    /// </summary>
    public void MakeInternal()
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
    /// so be aware to call it before.
    /// </summary>
    public void CleanupUnusedReferences()
    {
      Log("Cleaning up unused references...");

      int i = 0;
      while (i < definition.MainModule.AssemblyReferences.Count)
      {
        bool shouldClean = true;
        AssemblyNameReference assemblyRef = definition.MainModule.AssemblyReferences[i];

        foreach (ModuleDefinition reference in usedReferences)
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

        foreach (ModuleReference reference in usedUnmanagedReferences)
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
    /// Save the result assembly to file.
    /// </summary>
    /// <param name="filename">Path of file to save to.</param>
    public void Save(string filename)
    {
      definition.Write(filename);
    }

    /// <summary>
    /// Save the result assembly to stream.
    /// </summary>
    /// <param name="stream">Stream to save to.</param>
    public void Save(Stream stream)
    {
      definition.Write(stream);
    }

    #region IDisposable

    public void Dispose()
    {
      if (wpfRootParts != null)
      {
        foreach (KeyValuePair<string, ResourcePart> part in wpfRootParts)
          part.Value.Dispose();

        wpfRootParts.Clear();
      }
    }

    #endregion
  }
}
