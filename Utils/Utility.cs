namespace EnhancedMonsters.Utils;

public static class Utility
{
    public const float KgToLb = 1f/0.45359237f;

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
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="T">Value type</typeparam>
    /// <typeparam name="TI">Enumerable</typeparam>
    /// <param name="target">Where the objects will be merged</param>
    /// <param name="source">Objects to be merged</param>
    /// <param name="key">Key accessor</param>
    /// <param name="selector">Value accessor</param>
    /// <param name="set">Wether it is a SetRange or an AddRange operation</param>
    public static void AddRange<K, T, TI>(this IDictionary<K, T> target, IEnumerable<TI> source, Func<TI, K> key, Func<TI, T> selector, bool set = true)
    {
        foreach (var item in source)
        {
            var dkey = key(item);
            var dval = selector(item);
            if(set)
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
    /// Instead of returning a new dict resulting of the merge, it directly merges into the existing dictionary.<br/>
    /// Above this, duplicated keys are ignored. Basically a "BetterUnion" as well
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="T">Value</typeparam>
    /// <param name="target">Where the objects will be merged</param>
    /// <param name="source">Objects to be merged/unioned/concatenated</param>
    public static void ProperConcat<K, T>(this IDictionary<K, T> target, IDictionary<K, T> source)
    {
        foreach(var kvp in source)
        {
            if (target.ContainsKey(kvp.Key))
                continue;
            target.Add(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Removes and destroy all the components in the children of the gameobject.
    /// </summary>
    /// <typeparam name="T">Type of the components to remove</typeparam>
    /// <param name="go">GameObject whose children to remove the components from</param>
    public static void RemoveComponentsInChildren<T>(this GameObject go) where T : Component
    {
        var components = go.GetComponentsInChildren<T>();
        foreach(T comp in components)
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

    public static UnityEngine.Vector3 ToUnityVec(this System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);
    public static System.Numerics.Vector3 ToSystemVec(this UnityEngine.Vector3 v) => new(v.x, v.y, v.z);
}
