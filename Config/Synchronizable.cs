namespace EnhancedMonsters.Config;

[Serializable]
public class Synchronizable<T>
{
    internal static CustomMessagingManager MessagingManager => NetworkManager.Singleton.CustomMessagingManager;
    internal static bool IsClient => NetworkManager.Singleton?.IsClient ?? false;
    internal static bool IsHost => NetworkManager.Singleton?.IsHost ?? false;

    [NonSerialized]
    protected static int IntSize = 4;

    public static T Default { get; set; }
    public static T Instance { get; set; }

    public static bool Synced { get; internal set; }

    protected void InitInstance(T instance)
    {
        Default = instance;
        Instance = instance;

        IntSize = sizeof(int);
    }

    internal static void SyncInstance(byte[] data)
    {
        Instance = Deserialize(data);
        Synced = true;
    }

    internal static void RevertSync()
    {
        Instance = Default;
        Synced = false;
    }

    public static byte[] Serialize(T val)
    {
        BinaryFormatter bf = new();
        using MemoryStream stream = new();

        try
        {
            bf.Serialize(stream, val);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            Plugin.logger.LogError($"Error while serializing Config instance: {ex}");
            return null;
        }
    }

    public static T Deserialize(byte[] data)
    {
        BinaryFormatter bf = new();
        using MemoryStream stream = new(data);

        try
        {
            return (T)bf.Deserialize(stream);
        }
        catch (Exception ex)
        {
            Plugin.logger.LogError($"Couldn't deserialize Config instance: {ex}");
            return default;
        }
    }
}
