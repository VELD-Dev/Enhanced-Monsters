using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters.Utils;

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
}
