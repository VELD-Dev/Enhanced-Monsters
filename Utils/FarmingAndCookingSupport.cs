using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EnhancedMonsters.Utils;

internal static class FarmingAndCookingSupport
{
    private static bool? farmingAndCookingLoaded;

    public static bool FarmingAndCookingLoaded
    {
        get
        {
            farmingAndCookingLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(MelanieMeliciousCooked.Plugin.GUID);

            return (bool)farmingAndCookingLoaded;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void RegisterFarmingAndCookingBodies(List<Item> bodies)
    {
        var meatGrindConfig = MelanieCookedConfig.meatGrind.Value;
        string[] prefabNames = new string[bodies.Count];
        for(int i = 0; i < bodies.Count; i++)
        {
            var body = bodies[i];
            if (meatGrindConfig.Contains(body.name))
                continue;
            prefabNames[i] = body.spawnPrefab.name;
        }

        var concatenatedNames = string.Join(",", prefabNames.Where(s => s != string.Empty && s != null));
        MelanieCookedConfig.meatGrind.Value += meatGrindConfig == string.Empty ? concatenatedNames : $",{concatenatedNames}";
        ;
    }
}
