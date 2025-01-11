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
            SynchronizeMobClientRpc(netref, mobValue);
        }

        Plugin.logger.LogInfo("Mob should now be grabbable for all users.");
    }

    //////////
    // RPCs //
    //////////

    [ClientRpc]
    public static void SynchronizeMobClientRpc(NetworkBehaviourReference enemyNetRef, int mobValue)
    {
        if(!enemyNetRef.TryGet(out EnemyAI enemy, NetworkManager.Singleton))
        {
            Plugin.logger.LogError("Couldn't synchronize the enemy among network: The network reference was invalid. Critical synchronization error. Can you even see that dead enemy ?");
            return;
        }

        var enemyData = SyncedConfig.Instance.EnemiesData[enemy.enemyType.enemyName];

        Plugin.logger.LogInfo($"Synchronizing mob data between players: {enemy.enemyType.enemyName}, value: {mobValue} scraps");

        enemy.meshRenderers[0].GetComponent<Collider>().enabled = true;
        if (!enemy.gameObject.TryGetComponent(out PhysicsProp physPropComponent))
        {
            Plugin.logger.LogError("Mob did not have the PhysicsProp component. Make sure the component was correctly initialized before the game start.");
            return;
        }

        Plugin.logger.LogInfo("Enabling GrabbableObject component on the mob.");
        physPropComponent.mainObjectRenderer = null;
        physPropComponent.grabbable = enemyData.Pickupable;
        physPropComponent.scrapValue = mobValue;
        physPropComponent.enabled = true;
        physPropComponent.itemProperties.isScrap = enemyData.Pickupable;

        var scanNode = enemy.gameObject.transform.Find("ScanNode").gameObject.GetComponent<ScanNodeProperties>();
        scanNode.enabled = true;
        enemy.gameObject.transform.Find("ScanNode").gameObject.GetComponent<Collider>().enabled = true;
        scanNode.scrapValue = physPropComponent.scrapValue;
        scanNode.subText = $"Rank:{enemyData.Rank}\nValue: ${scanNode.scrapValue}";
        scanNode.maxRange = 13;
        scanNode.minRange = 1;
        scanNode.nodeType = 2;
        scanNode.requiresLineOfSight = true;
        scanNode.tag = "";

        Plugin.logger.LogInfo("Mob successfully synchronized among all clients ! It is now grabbable and sellable !");
    }
}
