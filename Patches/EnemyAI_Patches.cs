using EnhancedMonsters.Utils;


namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(EnemyAI))]
public class EnemyAI_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Start))]
    private static void Start(EnemyAI __instance)
    {
        string creatureRank;
        if(LocalConfig.Singleton.synchronizeRanks.Value)
            creatureRank = SyncedConfig.Instance.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";
        else
            creatureRank = SyncedConfig.Default.EnemiesData[__instance.enemyType.enemyName].Rank ?? "?";

        var scanNode = __instance.gameObject.transform.Find("ScanNode");
        if(scanNode)
        {
            var scanData = scanNode.gameObject.EnsureComponent<ScanNodeProperties>();
            scanData.subText = $"Rank {creatureRank}";
        }
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
            __instance.GetComponent<NetworkObject>().Despawn(true);
        }

        Plugin.logger.LogDebug("Mob should now be grabbable for all users.");
    }
}
