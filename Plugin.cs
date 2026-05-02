using EnhancedMonsters.Networking;
using EnhancedMonsters.Utils;
using System.Diagnostics.CodeAnalysis;
using NetworkPrefabs = LethalLib.Modules.NetworkPrefabs;

namespace EnhancedMonsters;

[BepInPlugin(PluginInfo.GUID, PluginInfo.DisplayName, PluginInfo.Version)]
[BepInDependency(LethalConfig.PluginInfo.Guid, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.willis.lc.lethalsettings", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("evaisa.lethallib", BepInDependency.DependencyFlags.HardDependency)]
[BepInIncompatibility("Entity378.sellbodies")]
public class Plugin : BaseUnityPlugin
{
    [NotNull] internal static Plugin Singleton { get; private set; }
    private static readonly Harmony harmony = new(PluginInfo.GUID);
    [NotNull] public static ManualLogSource? logger;
    public static GameObject EnemyToPropPrefab { get; private set; }
    public static GameObject NetworkHandlerPrefab { get; private set; }

    private void Awake()
    {
        Singleton = this;
        logger = Logger;
        new LocalConfig(Config);
        logger.LogInfo("Local config initialized.");

        logger.LogInfo("This is (maybe) NOT an RPG mod. And hi btw.");
        EnemiesDataManager.LoadEnemiesData();
        new SyncedConfig(Config);
        logger.LogInfo("Enemies data loaded and ready to be synchronized.");
        ApplyAllPatches();
        logger.LogDebug("All harmony patches have been applied (energistics).");
        CreateThePrefab();
        logger.LogDebug("The Prefab have been created correctly");
        CreateNetworkHandlerPrefab();
        logger.LogDebug("EnhancedMonstersNetworkHandler network prefab registered");

        if (FastResourcesManager.EnemyScrapIcon == null)
        {
            logger.LogError("EnemyScrapIcon didn't load yet. Caution advised.");
        }
        else
        {
            logger.LogInfo("EnemyScrapIcon loaded correctly and is ready to use !");
        }
    }

    private static void ApplyAllPatches()
    {
        if (logger is null)
        {
            throw new NullReferenceException($"{nameof(logger)} is null. Cannot process further because it means that the mod was not initialized yet.");
        }
        logger.LogDebug("Applying patches...");
        harmony.PatchAll(typeof(EnemyAI_Patches));
        harmony.PatchAll(typeof(MenuManager_Patches));
        harmony.PatchAll(typeof(PlayerControllerB_Patches));
        harmony.PatchAll(typeof(GameNetworkManager_Patches));
        harmony.PatchAll(typeof(StartOfRound_Patches));
        logger.LogDebug("All Harmony patches applied.");
    }

    private static void CreateThePrefab()
    {
        var prefab = NetworkPrefabs.CreateNetworkPrefab("EnemyAsProp");
        prefab.tag = "PhysicsProp";
        prefab.layer = 6;
        var ngo = prefab.GetComponent<NetworkObject>();
        ngo.AutoObjectParentSync = false;
        ngo.DontDestroyWithOwner = true;
        var collider = prefab.AddComponent<BoxCollider>();
        collider.enabled = true;
        collider.isTrigger = true;
        collider.size = new(1.5f, 1.5f, 1.5f);
        prefab.AddComponent<EnemyScrap>();
        prefab.AddComponent<AudioSource>();
        prefab.AddComponent<OccludeAudio>();
        prefab.AddComponent<AudioLowPassFilter>();

        NetworkPrefabs.RegisterNetworkPrefab(prefab);
        EnemyToPropPrefab = prefab;
    }

    private static void CreateNetworkHandlerPrefab()
    {
        var prefab = NetworkPrefabs.CreateNetworkPrefab("EnhancedMonstersNetworkHandler");
        var ngo = prefab.GetComponent<NetworkObject>();
        ngo.AutoObjectParentSync = false;
        ngo.DontDestroyWithOwner = true;
        prefab.AddComponent<EnhancedMonstersNetworkHandler>();

        NetworkPrefabs.RegisterNetworkPrefab(prefab);
        NetworkHandlerPrefab = prefab;
    }
}
