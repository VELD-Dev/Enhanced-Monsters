﻿using EnhancedMonsters.Utils;
using System.Runtime.CompilerServices;

namespace EnhancedMonsters;

[BepInPlugin(PluginInfo.GUID, PluginInfo.DisplayName, PluginInfo.Version)]
[BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.willis.lc.lethalsettings", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("evaisa.lethallib", BepInDependency.DependencyFlags.HardDependency)]
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
        Plugin.logger.LogInfo("Local config initialized.");
        NetcodePatcher();
    }

    private void Start()
    {
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
        var prefab = LethalLib.Modules.NetworkPrefabs.CreateNetworkPrefab("EnemyAsProp");
        prefab.tag = "PhysicsProp";
        prefab.layer = 6;
        var ngo = prefab.GetComponent<NetworkObject>();
        ngo.AutoObjectParentSync = false;
        var collider = prefab.AddComponent<BoxCollider>();
        collider.enabled = true;
        collider.isTrigger = true;
        collider.size = new(1.5f, 1.5f, 1.5f);
        prefab.AddComponent<EnemyScrap>();

        EnemyToPropPrefab = prefab;
    }

    private static void NetcodePatcher()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}
