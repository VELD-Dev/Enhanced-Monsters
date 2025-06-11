using EnhancedMonsters.Utils;


namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
public class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        if (__instance.enemyType is null)
            return;

        string creatureRank;
        SyncedConfig TargetConfig = LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance : SyncedConfig.Default;

        if(!TargetConfig.EnemiesData.TryGetValue(__instance.enemyType.enemyName, out var enemyData))
        {
            Plugin.logger.LogWarning($"Enemy {__instance.enemyType.enemyName} is not registered in the enemies data. Registering it now.");
            enemyData = new(true);
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, enemyData);
        }

        if (enemyData.Rank == string.Empty)
            return;

        creatureRank = enemyData.Rank;

        var scanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
        if(scanNode)
        {
            scanNode.subText = $"Rank {creatureRank}";
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        if (__instance == null) return;  // Should never happen

        if (__instance.enemyType == null) return; // This should never happen but some mods have this issue.

        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} died.");

        if (destroy) return;

        if (!SyncedConfig.Instance.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, new(true));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            Plugin.logger.LogDebug($"Spawning EnemyScrap for enemy {__instance.enemyType.enemyName}.");
            if (!EnemiesDataManager.Enemies2Props.TryGetValue(__instance.enemyType.enemyName, out var enemy2prop))
            {
                Plugin.logger.LogWarning($"Mob {__instance.enemyType.enemyName} has no enemy2prop prefab.");
                return;
            }

            var enemyToPropInstance = NetworkManager.Instantiate(enemy2prop);
            enemyToPropInstance.hideFlags = HideFlags.None;
            enemyToPropInstance.transform.position = __instance.transform.position;
            enemyToPropInstance.GetComponent<NetworkObject>().Spawn();

            //move the original body away for all players
            __instance.transform.position = new(-10000, -10000, -10000);
            __instance.SyncPositionToClients();
        }
        
        Plugin.logger.LogDebug("Mob should now be grabbable for all users.");
    }
}
