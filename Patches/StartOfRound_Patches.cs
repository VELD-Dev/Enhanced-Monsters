namespace EnhancedMonsters.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void StartPatch()
    {
        EnemiesDataManager.ScanAndRegisterUnknownEnemies();
        EnemiesDataManager.EnsureEnemy2PropPrefabs();

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer && Plugin.NetworkHandlerPrefab != null)
        {
            var go = UnityEngine.Object.Instantiate(Plugin.NetworkHandlerPrefab);
            go.GetComponent<NetworkObject>().Spawn(true);
            Plugin.logger.LogInfo("EnhancedMonstersNetworkHandler spawned by server in StartOfRound");
        }
    }
}
