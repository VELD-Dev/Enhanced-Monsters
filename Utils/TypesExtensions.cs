using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Utils;

public static class TypesExtensions
{
    public static string ToStringObj(this object obj, string prefix = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine(prefix + $"{obj.GetType().FullName}: {{");
        foreach (var fieldinfo in obj.GetType().GetFields())
        {
            var val = fieldinfo.GetValue(obj) ?? null;
            sb.AppendLine(prefix + $"\t{fieldinfo.FieldType.Name} {fieldinfo.Name}: {val}");
        }
        sb.AppendLine(prefix + "}");
        return sb.ToString();
    }

    /// <summary>
    /// Verifies if a component exists on the <see cref="GameObject"/>.
    /// </summary>
    /// <typeparam name="T">Class of the component inheriting from <see cref="Component"/>.</typeparam>
    /// <param name="go">Instance of a <see cref="GameObject"/>.</param>
    /// <returns>If the component exists, returns the existing component, otherwise returns a newly created component.</returns>
    /// <exception cref="NullReferenceException">Thrown when <see cref="GameObject"/>instance is a null reference.</exception>
    public static T EnsureComponent<T>(this GameObject go) where T : Component
    {
        if (go is null) throw new NullReferenceException("The gameObject is a null reference.");

        if (!go.TryGetComponent<T>(out var component))
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
}
