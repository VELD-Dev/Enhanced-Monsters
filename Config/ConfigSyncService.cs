using EnhancedMonsters.Networking;
using EnhancedMonsters.Utils;

namespace EnhancedMonsters.Config;

internal static class ConfigSyncService
{
    public static SyncedConfig Default { get; private set; }
    public static SyncedConfig Instance { get; private set; }
    public static bool Synced { get; internal set; }

    public static bool IsHost => NetworkManager.Singleton?.IsHost ?? false;
    public static bool IsClient => NetworkManager.Singleton?.IsClient ?? false;

    public static void Initialize(SyncedConfig defaults)
    {
        Default = defaults;
        Instance = defaults;
        Synced = false;
    }

    public static void RevertSync()
    {
        Instance = Default;
        Synced = false;
        Plugin.logger.LogInfo("Reverting sync, Instance=Default, Synced=false");
    }

    public static void RequestSync()
    {
        if (!IsClient || IsHost) return;

        if (EnhancedMonstersNetworkHandler.Instance != null)
        {
            EnhancedMonstersNetworkHandler.Instance.RequestConfigSyncServerRpc();
            Plugin.logger.LogInfo("Requesting config sync via ServerRpc");
            return;
        }

        Plugin.logger.LogInfo("EnhancedMonstersNetworkHandler not yet spawned, scheduling retry");
        var so = StartOfRound.Instance;
        if (so != null)
            so.StartCoroutine(WaitForHandlerAndRequest());
        else
            Plugin.logger.LogWarning("StartOfRound.Instance is null, cannot schedule sync retry");
    }

    private static IEnumerator WaitForHandlerAndRequest()
    {
        const float timeoutSeconds = 10f;
        const float pollIntervalSeconds = 0.25f;
        float waited = 0f;

        while (EnhancedMonstersNetworkHandler.Instance == null && waited < timeoutSeconds)
        {
            yield return new WaitForSeconds(pollIntervalSeconds);
            waited += pollIntervalSeconds;
        }

        if (EnhancedMonstersNetworkHandler.Instance != null)
        {
            EnhancedMonstersNetworkHandler.Instance.RequestConfigSyncServerRpc();
            Plugin.logger.LogInfo($"Config sync request sent after {waited:0.00}s wait");
        }
        else
        {
            Plugin.logger.LogWarning($"Config sync handler did not appear within {timeoutSeconds}s, giving up");
        }
    }

    public static void BroadcastSync()
    {
        if (!IsHost) return;

        var handler = EnhancedMonstersNetworkHandler.Instance;
        if (handler == null)
        {
            Plugin.logger.LogDebug("BroadcastSync: handler not spawned yet, skipping");
            return;
        }

        byte[] payload = SerializeCurrent();
        if (payload.Length == 0) return;

        handler.BroadcastConfigSyncClientRpc(payload);
        Plugin.logger.LogInfo($"Host broadcast config sync, {payload.Length} bytes");
    }

    public static byte[] SerializeCurrent()
    {
        if (Instance == null)
        {
            Plugin.logger.LogError("SerializeCurrent: Instance is null");
            return Array.Empty<byte>();
        }

        try
        {
            string json = JsonConvert.SerializeObject(Instance.EnemiesData);
            return Encoding.UTF8.GetBytes(json);
        }
        catch (Exception e)
        {
            Plugin.logger.LogError($"Failed to serialize sync payload: {e}");
            return Array.Empty<byte>();
        }
    }

    public static void ApplyReceivedSync(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            Plugin.logger.LogError("ApplyReceivedSync: empty payload");
            return;
        }

        if (Instance == null)
        {
            Plugin.logger.LogError("ApplyReceivedSync: Instance is null, cannot apply");
            return;
        }

        try
        {
            string json = Encoding.UTF8.GetString(data);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>(json);
            if (dict == null)
            {
                Plugin.logger.LogError("ApplyReceivedSync: deserialization returned null");
                return;
            }

            EnemiesDataManager.EnemiesData.Clear();
            foreach (var kvp in dict)
                EnemiesDataManager.EnemiesData[kvp.Key] = kvp.Value;

            Synced = true;
            Plugin.logger.LogInfo($"Applied sync, {dict.Count} entries, Synced=true");
        }
        catch (Exception e)
        {
            Plugin.logger.LogError($"Failed to apply sync payload: {e}");
        }
    }
}
