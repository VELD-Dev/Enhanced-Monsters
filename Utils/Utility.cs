namespace EnhancedMonsters.Utils;

public static class Utility
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


    /// <summary>
    /// Allows adding a range of values inside a Dictionary. If a value is in conflict, it skips it.
    /// </summary>
    public static void AddRange<K, T, TI>(this IDictionary<K, T> target, IEnumerable<TI> source, Func<TI, K> key, Func<TI, T> selector, bool set = true)
    {
        foreach (var item in source)
        {
            var dkey = key(item);
            var dval = selector(item);
            if (set)
            {
                target[dkey] = dval;
            }
            else
            {
                target.TryAdd(key(item), selector(item));
            }
        }
    }

    /// <summary>
    /// Instead of returning a new dict resulting of the merge, it directly merges into the existing dictionary.
    /// Duplicated keys are ignored.
    /// </summary>
    public static void ProperConcat<K, T>(this IDictionary<K, T> target, IDictionary<K, T> source)
    {
        foreach (var kvp in source)
        {
            if (target.ContainsKey(kvp.Key))
                continue;
            target.Add(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Removes and destroy all the components in the children of the gameobject.
    /// </summary>
    public static void RemoveComponentsInChildren<T>(this GameObject go) where T : Component
    {
        var components = go.GetComponentsInChildren<T>();
        foreach (T comp in components)
        {
            Plugin.logger.LogDebug($"Destroying component {comp.GetType().Name} on {comp.name} (child of {go.name})");
            Component.Destroy(comp);
        }
    }

    internal static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null);
        }
    }
}
