using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void Start(GameNetworkManager __instance)
    {
        if (!NetworkManager.Singleton.NetworkConfig.Prefabs.Contains(Plugin.EnemyToPropPrefab))
        {
            NetworkManager.Singleton.AddNetworkPrefab(Plugin.EnemyToPropPrefab);
        }

        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        foreach (var enemy in enemies)
        {
            if (!SyncedConfig.Instance.EnemiesData.ContainsKey(enemy.enemyType.enemyName))
            {
                EnemiesDataManager.RegisterEnemy(enemy.enemyType.enemyName, new());
                Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{enemy.enemyType.enemyName}'");
            }
        }
        EnemiesDataManager.SaveEnemiesData();
    }

    [HarmonyPostfix]
    [HarmonyPatch("StartDisconnect")]
    public static void StartDisconnect()
    {
        SyncedConfig.RevertSync();
    }
}
