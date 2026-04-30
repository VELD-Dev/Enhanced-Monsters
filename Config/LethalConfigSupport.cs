using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;

namespace EnhancedMonsters.Config;

internal static class LethalConfigSupport
{
    private static bool? lethalConfigLoaded;
    public static bool LethalConfigLoaded
    {
        get
        {
            lethalConfigLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LethalConfig.PluginInfo.Guid);

            return (bool)lethalConfigLoaded;
        }
    }

    internal static void RegisterLethalConfig(LocalConfig config)
    {
        LethalConfigManager.SetModDescription(LocalConfig.ModDesc);

        var lcSyncRanks = new BoolCheckBoxConfigItem(config.synchronizeRanks, new BoolCheckBoxOptions { Name = "Synchronize Ranks", Section = "General", RequiresRestart = false, CanModifyCallback = () => !GameNetworkManager.Instance.gameHasStarted || !NetworkManager.Singleton.IsListening });

        var lcAccessEnemiesDataFile = new GenericButtonConfigItem(
            "General",
            "Enemies Data",
            "All the enemies data are stored inside the file. You have a few rules to follow when editing the file or it will not work. Look at the github or thunderstore page for more info.",
            "Copy path",
            () => { GUIUtility.systemCopyBuffer = EnemiesDataManager.EnemiesDataFile; }
        );

        LethalConfigManager.AddConfigItem(lcSyncRanks);
        LethalConfigManager.AddConfigItem(lcAccessEnemiesDataFile);
    }
}
