namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerB_Patches
{

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
