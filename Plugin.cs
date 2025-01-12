using EnhancedMonsters.Utils;

namespace EnhancedMonsters;

[BepInPlugin(PluginInfo.GUID, PluginInfo.DisplayName, PluginInfo.Version)]
[BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(StaticNetcodeLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInIncompatibility("Entity378.sellbodies")]
public class Plugin : BaseUnityPlugin 
{
    internal static Plugin Singleton { get; private set; }
    private static readonly Harmony harmony = new(PluginInfo.GUID);
    public static ManualLogSource? logger;
    public static GameObject EnemyToPropPrefab { get; private set; }

    private void Awake()
    {
        Singleton = this;
        logger = Logger;
        new LocalConfig(Config);
    }

    private void Start()
    {
        logger ??= Logger;  // This should never happen.

        logger.LogInfo("This is (maybe) NOT an RPG mod. And hi btw.");
        EnemiesDataManager.LoadEnemiesData();
        new SyncedConfig(Config);
        logger.LogInfo("Enemies data loaded and ready to be synchronized.");
        ApplyAllPatches();
        logger.LogDebug("All harmony patches have been applied (energistics).");
        CreateThePrefab();
        logger.LogDebug("The Prefab have been created correctly");
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
        //harmony.PatchAll(typeof(MaskedPlayerEnemy_Patches));
        logger.LogDebug("MaskedEnemy patches applied.");
        harmony.PatchAll(typeof(MenuManager_Patches));
        logger.LogDebug("MenuManager patches applied.");
        harmony.PatchAll(typeof(PlayerControllerB_Patches));
        logger.LogDebug("PlayerController patches applied.");
        harmony.PatchAll(typeof(GameNetworkManager_Patches));
        logger.LogDebug("GameNetworkManager patches applied.");
    }

    private static void CreateThePrefab()
    {
        var prefab = new GameObject("EnemyAsProp");
        prefab.hideFlags = HideFlags.HideAndDontSave;
        prefab.tag = "PhysicsProp";
        prefab.layer = 6;
        GameObject.DontDestroyOnLoad(prefab);
        var networkObject = prefab.AddComponent<NetworkObject>();
        networkObject.ActiveSceneSynchronization = false;
        networkObject.AlwaysReplicateAsRoot = false;
        networkObject.AutoObjectParentSync = false;
        networkObject.SynchronizeTransform = true;
        networkObject.SceneMigrationSynchronization = true;
        networkObject.SpawnWithObservers = true;
        networkObject.DontDestroyWithOwner = true;
        var collider = prefab.AddComponent<BoxCollider>();
        collider.enabled = true;
        var physProp = prefab.AddComponent<PhysicsProp>();
        physProp.grabbable = true;
        physProp.scrapValue = 0;
        physProp.propColliders = [ collider ];
        physProp.enabled = true;
        physProp.customGrabTooltip = "Grab";
        physProp.itemProperties = ScriptableObject.CreateInstance<Item>();
        physProp.itemProperties.itemId = 0;
        physProp.itemProperties.allowDroppingAheadOfPlayer = true;
        physProp.itemProperties.creditsWorth = 0;
        physProp.itemProperties.grabAnim = "HoldLung";
        physProp.itemProperties.isScrap = false;
        physProp.itemProperties.itemSpawnsOnGround = false;
        physProp.itemProperties.twoHanded = true;
        physProp.itemProperties.twoHandedAnimation = true;
        physProp.itemProperties.meshOffset = false;
        physProp.itemProperties.pocketAnim = "";
        physProp.itemProperties.throwAnim = "";
        physProp.itemProperties.useAnim = "";
        physProp.itemProperties.meshVariants = [];
        physProp.itemProperties.materialVariants = [];
        physProp.itemProperties.requiresBattery = false;

        EnemyToPropPrefab = prefab;
    }
}
