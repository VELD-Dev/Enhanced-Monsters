namespace EnhancedMonsters.Utils;

internal static class FarmingAndCookingSupport
{
    private const string FarmingAndCookingGuid = "MelanieMelicious.FarmAndCook";

    private static bool? farmingAndCookingLoaded;

    public static bool FarmingAndCookingLoaded
    {
        get
        {
            farmingAndCookingLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(FarmingAndCookingGuid);

            return (bool)farmingAndCookingLoaded;
        }
    }

    public static void RegisterFarmingAndCookingBodies(List<Item> bodies)
    {
        foreach (var body in bodies)
        {
            MelanieMeliciousCooked.Plugin.bodyHash.Add(body);
            Plugin.logger.LogDebug($"Registered {body.itemName} for MelanieMeliciousCooked plugin.");
        }

        Plugin.logger.LogInfo($"Successfully registered all the corpses as grindable for the Farming & Cooking mod.");
    }
}
