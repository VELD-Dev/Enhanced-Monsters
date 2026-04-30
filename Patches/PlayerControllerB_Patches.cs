namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerB_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    public static void InitializeLocalPlayerPrefabs()
    {
        EnemiesDataManager.ScanAndRegisterUnknownEnemies();
        EnemiesDataManager.EnsureEnemy2PropPrefabs();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    public static void InitializeLocalPlayer()
    {
        if (SyncedConfig.IsHost)
        {
            Plugin.logger.LogInfo("Local player is host, no sync request needed");
            return;
        }

        SyncedConfig.RequestSync();
    }
}
