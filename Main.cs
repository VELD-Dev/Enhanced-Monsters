namespace LootableMonsters;

[BepInPlugin(PluginInfo.GUID, PluginInfo.DisplayName, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin   
{
    private static readonly Harmony harmony = new(PluginInfo.GUID);
    public static ManualLogSource? logger;

    private void Awake()
    {
        logger = Logger;
    }

    private void Start()
    {
        logger ??= Logger;  // This should never happen.

        logger.LogInfo("This is NOT an RPG mod. And hi btw.");
        harmony.PatchAll();
        logger.LogDebug("All harmony patches have been applied (energistics).");
    }
}
