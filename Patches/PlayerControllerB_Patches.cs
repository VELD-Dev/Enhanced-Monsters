namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerB_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    public static void InitializeLocalPlayerPrefabs()
    {
        var enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();
        Plugin.logger.LogInfo($"Double checking {enemies.Length} enemies. The list may have changed.");
        foreach (var enemy in enemies)
        {
            if (SyncedConfig.Instance.EnemiesData.ContainsKey(enemy.enemyType.enemyName))
                continue;

            EnemiesDataManager.RegisterEnemy(enemy.enemyType.enemyName, new(true, metadata: new(new(0, 0, 0), new(0, 0, 0), new(0, 0, 0), true)));
            Plugin.logger.LogInfo($"Mob was not registered. Registered it with name '{enemy.enemyType.enemyName}'");
        }
        EnemiesDataManager.SaveEnemiesData();

        EnemiesDataManager.EnsureEnemy2PropPrefabs();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    public static void InitializeLocalPlayer()
    {
        if(SyncedConfig.IsHost)
        {
            SyncedConfig.MessagingManager.RegisterNamedMessageHandler("EnhancedMonsters_OnRequestConfigSync", SyncedConfig.OnRequestSync);
            SyncedConfig.Synced = true;

            return;
        }

        SyncedConfig.Synced = false;
        SyncedConfig.MessagingManager.RegisterNamedMessageHandler("EnhancedMonsters_OnReceiveConfigSync", SyncedConfig.OnReceiveSync);
        SyncedConfig.RequestSync();
    }
}
