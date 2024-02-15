namespace EnhancedMonsters;

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
        EnemiesDataManager.LoadEnemiesData();
        logger.LogInfo("Enemies data loaded.");
        ApplyAllPatches();
        logger.LogDebug("All harmony patches have been applied (energistics).");
    }

    private static void ApplyAllPatches()
    {
        if (logger is null)
        {
            throw new NullReferenceException($"{nameof(logger)} is null. Cannot process further because it means that the mod was not initialized yet.");
        }
        logger.LogDebug("Applying patches...");
        harmony.PatchAll(typeof(EnemyAI_Patches));
        logger.LogDebug("Enemy patches applied.");
    }
}
