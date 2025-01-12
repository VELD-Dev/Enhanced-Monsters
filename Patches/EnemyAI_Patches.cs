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

        Plugin.logger.LogDebug("Mob should now be grabbable for all users.");
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

        if(!EnemiesDataManager.Enemies2Props.TryGetValue(enemy.enemyType.enemyName, out var enemy2prop))
        {
            Plugin.logger.LogWarning($"Mob {enemy.enemyType.enemyName} has no enemy2prop prefab.");
        }

        var enemyToPropInstance = NetworkManager.Instantiate(enemy2prop);
        enemyToPropInstance.GetComponent<NetworkObject>().Spawn();
        var e2propInstRef = new NetworkObjectReference(enemyToPropInstance);

        SynchronizeMobClientRpc(netref, e2propInstRef, mobValue);
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

        var enemy2PropGO = enemy2PropNetObj.gameObject;
        enemy2PropGO.transform.position = enemy.transform.position;
        var enemyData = SyncedConfig.Instance.EnemiesData[enemy.enemyType.enemyName];

        if (!enemy2PropGO.TryGetComponent(out PhysicsProp physPropComponent))
        {
            Plugin.logger.LogError("Physic Prop Object did not have the PhysicsProp component. Make sure the component was correctly initialized before the game start.");
            return;
        }

        physPropComponent.grabbable = enemyData.Pickupable;
        physPropComponent.scrapValue = mobValue;

        var scanNodeGo = enemy2PropGO.transform.Find("ScanNode").gameObject;
        var scanNode = scanNodeGo.GetComponent<ScanNodeProperties>();
        scanNode.scrapValue = physPropComponent.scrapValue;
        scanNode.subText = $"Rank:{enemyData.Rank}\nValue: ${scanNode.scrapValue}";

        enemy.gameObject.GetComponent<NetworkObject>().Despawn(true);

        Plugin.logger.LogDebug("Mob successfully synchronized among all clients ! It is now grabbable and sellable !");
    }
}
