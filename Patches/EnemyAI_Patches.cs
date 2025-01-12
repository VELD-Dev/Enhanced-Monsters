using EnhancedMonsters.Utils;


namespace EnhancedMonsters.Patches;

[StaticNetcode]
[HarmonyPatch(typeof(EnemyAI))]
public class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        string creatureRank = SyncedConfig.Instance.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";
        var scanData = __instance.gameObject.transform.Find("ScanNode").gameObject.EnsureComponent<ScanNodeProperties>();
        scanData.subText = $"Rank {creatureRank}";
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        if (__instance == null) return;  // Should never happen

        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} died.");

        if (destroy) return;

        if (!__instance.isEnemyDead)
        {
            __instance.isEnemyDead = true;
        }

        if (!SyncedConfig.Instance.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, new(true));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        if (NetworkManager.Singleton.IsHost)
        {
            var enemyData = SyncedConfig.Instance.EnemiesData[__instance.enemyType.enemyName];
            int mobValue = RoundManager.Instance.AnomalyRandom.Next(enemyData.MinValue, enemyData.MaxValue);
            var netref = new NetworkBehaviourReference(__instance);
            Plugin.logger.LogInfo("Synchronizing the mob data and scrap value with clients...");
            InstantiatePhysicPrefabServerRpc(netref, mobValue);
        }

        Plugin.logger.LogInfo("Mob should now be grabbable for all users.");
    }

    //////////
    // RPCs //
    //////////

    [ServerRpc]
    public static void InstantiatePhysicPrefabServerRpc(NetworkBehaviourReference netref, int mobValue)
    {
        if(!netref.TryGet<EnemyAI>(out var enemy))
        {
            Plugin.logger.LogError("Coudln't instantiate physic prop prefab: Invalid EnemyAI net behaviour reference");
            return;
        }

        var enemyToPropInstance = NetworkManager.Instantiate(Plugin.EnemyToPropPrefab);
        enemyToPropInstance.GetComponent<NetworkObject>().Spawn();

        SynchronizeMobClientRpc(netref, new NetworkObjectReference(enemyToPropInstance), mobValue);
    }

    [ClientRpc]
    public static void SynchronizeMobClientRpc(NetworkBehaviourReference enemyNetRef, NetworkObjectReference enemy2PropNetRef, int mobValue)
    {
        if(!enemyNetRef.TryGet(out EnemyAI enemy, NetworkManager.Singleton))
        {
            Plugin.logger.LogError("Couldn't synchronize the enemy among network: The network reference was invalid. Critical synchronization error. Can you even see that dead enemy ?");
            return;
        }
        
        if(!enemy2PropNetRef.TryGet(out NetworkObject enemy2PropNetObj))
        {
            Plugin.logger.LogError("Couldn't synchronize the enemy among network: The network object reference for Physic Props was invalid. Maybe it wasn't spawned correctly at first.");
            return;
        }

        Plugin.logger.LogDebug("SyncMobBreak1");

        var enemy2PropGO = enemy2PropNetObj.gameObject;

        Plugin.logger.LogDebug("SyncMobBreak2");
        enemy2PropGO.name = enemy.enemyType.enemyName + " propized";
        Plugin.logger.LogDebug("SyncMobBreak3");
        enemy2PropGO.transform.position = enemy.transform.position;
        enemy.transform.parent = enemy2PropGO.transform;
        enemy.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);

        Plugin.logger.LogDebug("SyncMobBreak4");
        var boxCollider = enemy2PropGO.GetComponent<BoxCollider>();
        Plugin.logger.LogDebug("SyncMobBreak5");
        var capsuleCollider = enemy.GetComponentInChildren<Collider>();
        boxCollider.center = capsuleCollider.bounds.center;

        Plugin.logger.LogDebug("SyncMobBreak6");
        var enemyData = SyncedConfig.Instance.EnemiesData[enemy.enemyType.enemyName];

        Plugin.logger.LogInfo($"Synchronizing mob data between players: {enemy.enemyType.enemyName}, value: {mobValue} scraps");

        if (!enemy2PropGO.TryGetComponent(out PhysicsProp physPropComponent))
        {
            Plugin.logger.LogError("Physic Prop Object did not have the PhysicsProp component. Make sure the component was correctly initialized before the game start.");
            return;
        }

        Plugin.logger.LogInfo("Enabling GrabbableObject component on the mob.");
        physPropComponent.grabbable = enemyData.Pickupable;
        physPropComponent.scrapValue = mobValue;
        physPropComponent.itemProperties.isScrap = enemyData.Pickupable;

        Plugin.logger.LogInfo("Updating ScanNode...");
        var scanNodeGo = enemy.gameObject.transform.Find("ScanNode").gameObject;
        scanNodeGo.transform.parent = enemy2PropGO.transform;
        scanNodeGo.GetComponent<Collider>().enabled = true;

        var scanNode = scanNodeGo.GetComponent<ScanNodeProperties>();
        Plugin.logger.LogInfo("Setting ScanNode scrap value...");
        scanNode.scrapValue = physPropComponent.scrapValue;
        scanNode.subText = $"Rank:{enemyData.Rank}\nValue: ${scanNode.scrapValue}";
        scanNode.maxRange = 13;
        scanNode.minRange = 1;
        scanNode.nodeType = 2;
        scanNode.requiresLineOfSight = true;

        Plugin.logger.LogInfo("Mob successfully synchronized among all clients ! It is now grabbable and sellable !");
    }
}
