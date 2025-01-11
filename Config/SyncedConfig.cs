namespace EnhancedMonsters.Config;

[Serializable]
public class SyncedConfig : Synchronizable<SyncedConfig>
{
    public Dictionary<string, EnemyData> EnemiesData;
    
    [NonSerialized]
    public ConfigFile BepInConfigs;

    public SyncedConfig(ConfigFile cfg)
    {
        InitInstance(this);
        BepInConfigs = cfg;
        EnemiesData = EnemiesDataManager.EnemiesData;
    }

    public static void BroadcastSync()
    {
        if (!IsHost) return;

        Plugin.logger.LogDebug($"Host is broadcasting its config.");

        byte[] data = Serialize(Instance);
        var trueLength = data.Length;
        var fbwLength = FastBufferWriter.GetWriteSize(data) + IntSize;

        using FastBufferWriter writer = new(fbwLength, Allocator.Temp);
        try
        {
            writer.WriteValueSafe(in trueLength, default);
            writer.WriteBytesSafe(data);

            MessagingManager.SendNamedMessageToAll("EnhancedMonster_OnReceiveConfigSync", writer, NetworkDelivery.ReliableFragmentedSequenced);
        }
        catch(Exception e)
        {
            Plugin.logger.LogError($"The config sync broadcast lamentably failed lmao; here's the error: {e}");
        }
    }

    public static void RequestSync()
    {
        if (!IsClient) return;

        using FastBufferWriter stream = new(IntSize, Allocator.Temp);
        MessagingManager.SendNamedMessage("EnhancedMonsters_OnRequestConfigSync", 0uL, stream);
        Plugin.logger.LogInfo("Asking host for their EnemyData config");
    }

    // Registered in the Synchronizable class.
    public static void OnRequestSync(ulong clientId, FastBufferReader _)
    {
        if(!IsHost) return;

        Plugin.logger.LogDebug($"Config sync request received from client {clientId}");

        byte[] data = Serialize(Instance);
        var trueLength = data.Length;
        var fbwLength = FastBufferWriter.GetWriteSize(data);

        using FastBufferWriter stream = new(fbwLength + IntSize, Allocator.Temp);

        try
        {
            stream.WriteValueSafe(in trueLength);
            stream.WriteBytesSafe(data);

            MessagingManager.SendNamedMessage("EnhancedMonsters_OnReceiveConfigSync", clientId, stream, NetworkDelivery.ReliableFragmentedSequenced);
        }
        catch (Exception ex)
        {
            Plugin.logger.LogError($"Coudln't sync configs between clients. Error: {ex}");
        }
    }
    
    // Registered in the Synchronizable class.
    public static void OnReceiveSync(ulong _, FastBufferReader reader)
    {
        if(!reader.TryBeginRead(IntSize))
        {
            Plugin.logger.LogError("Config sync error: could not begin reading buffer.");
            return;
        }

        reader.ReadValueSafe(out int length);
        if(!reader.TryBeginRead(length))
        {
            Plugin.logger.LogError($"Config sync error: Host could not sync. Error: The chunk of the buffer to read is larger than the remaining bytes chunk. {length} bytes to read, {reader.Length - reader.Position} remaining bytes");
            return;
        }

        var data = new byte[length];
        reader.ReadBytesSafe(ref data, length);

        SyncInstance(data);

        Plugin.logger.LogInfo("Successfully synced config with host");
    }
}
