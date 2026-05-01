using EnhancedMonsters.Networking;

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

        SyncedConfig TargetConfig = LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance : SyncedConfig.Default;

        if (!TargetConfig.EnemiesData.TryGetValue(__instance.enemyType.enemyName, out var enemyData))
        {
            Plugin.logger.LogWarning($"Enemy {__instance.enemyType.enemyName} is not registered in the enemies data. Registering it now.");
            enemyData = new(true);
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, enemyData);
        }

        if (enemyData.Rank == string.Empty)
            return;

        var scanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
        if (scanNode)
        {
            scanNode.subText = $"Rank {enemyData.Rank}";
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
    private static void KillEnemy(EnemyAI __instance, bool destroy)
    {
        if (__instance == null) return;

        if (__instance.enemyType == null) return;

        Plugin.logger.LogDebug($"Mob {__instance.enemyType.enemyName} died.");

        if (destroy) return;

        if (!SyncedConfig.Instance.EnemiesData.ContainsKey(__instance.enemyType.enemyName))
        {
            EnemiesDataManager.RegisterEnemy(__instance.enemyType.enemyName, new(true));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{__instance.enemyType.enemyName}'");
        }

        if (!NetworkManager.Singleton.IsServer) return;

        Plugin.logger.LogDebug($"Spawning EnemyScrap for enemy {__instance.enemyType.enemyName}.");
        if (!EnemiesDataManager.Enemies2Props.TryGetValue(__instance.enemyType.enemyName, out var enemy2prop))
        {
            Plugin.logger.LogWarning($"Mob {__instance.enemyType.enemyName} has no enemy2prop prefab.");
            return;
        }

        // Spawn the pickupable corpse server-side. NGO replicates the spawn to all clients
        // automatically through its NetworkObject pipeline (the prefab's GlobalObjectIdHash
        // matches between host and client because BuildScrapPrefab clones it using the
        // canonical enemyType.enemyName).
        var enemyToPropInstance = UnityEngine.Object.Instantiate(enemy2prop);
        enemyToPropInstance.hideFlags = HideFlags.None;
        enemyToPropInstance.transform.position = __instance.transform.position;
        enemyToPropInstance.GetComponent<NetworkObject>().Spawn();

        // Move the original LC body off-map so the new pickupable corpse is the only thing
        // visible. Update both transform.position and serverPosition: clients lerp toward
        // serverPosition in EnemyAI.Update, so leaving it at the old value would cause them
        // to snap back. Fire the custom ClientRpc to apply the same change on every client.
        Vector3 hidePosition = new(-10000, -10000, -10000);
        __instance.transform.position = hidePosition;
        __instance.serverPosition = hidePosition;

        var enemyNetworkObject = __instance.GetComponent<NetworkObject>();
        var handler = EnhancedMonstersNetworkHandler.Instance;
        if (handler != null && enemyNetworkObject != null)
        {
            handler.HideOriginalEnemyBodyClientRpc(enemyNetworkObject, hidePosition);
        }
        else
        {
            Plugin.logger.LogWarning("Network handler or enemy NetworkObject unavailable; falling back to SyncPositionToClients()");
            __instance.SyncPositionToClients();
        }

        Plugin.logger.LogDebug("Mob should now be grabbable for all users.");
    }
}
