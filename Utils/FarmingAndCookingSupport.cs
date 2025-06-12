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
        foreach(var body in bodies)
        {
            // Hashsets don't need to be checked for duplicates. They do it on their own.
            MelanieMeliciousCooked.Plugin.bodyHash.Add(body);
            Plugin.logger.LogDebug($"Registered {body.itemName} for MelanieMeliciousCooked plugin.");
        }

        Plugin.logger.LogInfo($"Successfully registered all the corpses as grindable for the Farming & Cooking mod.");
    }
}
