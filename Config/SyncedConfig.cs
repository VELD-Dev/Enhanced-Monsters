namespace EnhancedMonsters.Config;

public class SyncedConfig
{
    public Dictionary<string, EnemyData> EnemiesData;

    public ConfigFile BepInConfigs;

    public SyncedConfig(ConfigFile cfg)
    {
        BepInConfigs = cfg;
        EnemiesData = EnemiesDataManager.EnemiesData;
        ConfigSyncService.Initialize(this);
    }

    public static SyncedConfig Default => ConfigSyncService.Default;
    public static SyncedConfig Instance => ConfigSyncService.Instance;
    public static bool Synced => ConfigSyncService.Synced;
    public static bool IsHost => ConfigSyncService.IsHost;
    public static bool IsClient => ConfigSyncService.IsClient;

    public static void BroadcastSync() => ConfigSyncService.BroadcastSync();
    public static void RequestSync() => ConfigSyncService.RequestSync();
    public static void RevertSync() => ConfigSyncService.RevertSync();
}
