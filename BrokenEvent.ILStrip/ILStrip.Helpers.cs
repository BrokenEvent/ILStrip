using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

namespace BrokenEvent.ILStrip
{
  partial class ILStrip
  {
    /// <summary>
    /// Gets string list of all the type names in currently loaded assembly.
    /// </summary>
    /// <param name="separator">Separator string between types.</param>
    /// <returns>List of all the typenames.</returns>
    public string GetAllTypesList(string separator = "\r\n")
    {
      return GetAllTypesList(definition, separator);
    }

    /// <summary>
    /// Gets string list of names all used types in currently loaded assembly.
    /// </summary>
    /// <param name="separator">Separator string between types.</param>
    /// <returns>List of all uses typenames.</returns>
    public string GetUsedTypesList(string separator = "\r\n")
    {
      return string.Join(separator, usedTypesCache.Select(t => t.FullName));
    }

    /// <summary>
    /// Gets string list of all used references in assembly.
    /// </summary>
    /// <param name="separator">Separator string between types.</param>
    /// <returns>List of all used references.</returns>
    public string GetUsedReferences(string separator = "\r\n")
    {
      return string.Join(separator, usedReferences.Select(r => r.Name));
    }

    /// <summary>
    /// Gets string list of all types in assembly.
    /// </summary>
    /// <param name="definition">Assembly to list types.</param>
    /// <param name="separator">Separator string between types.</param>
    /// <returns>List of all the typenames.</returns>
    public static string GetAllTypesList(AssemblyDefinition definition, string separator = "\r\n")
    {
      List<TypeDefinition> types = new List<TypeDefinition>();
      foreach (TypeDefinition type in definition.MainModule.Types)
      {
        types.Add(type);
        foreach (TypeDefinition nestedType in type.NestedTypes)
          types.Add(nestedType);
      }

      return string.Join(separator, types.Select(t => t.FullName));
    }
  }
}
