using System.Runtime.CompilerServices;
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

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    internal static void RegisterLethalConfig(LocalConfig config)
    {
        LethalConfigManager.SetModDescription(LocalConfig.ModDesc);

        var lcSyncRanks = new BoolCheckBoxConfigItem(config.synchronizeRanks, new BoolCheckBoxOptions { Name = "Synchronize Ranks", Section = "General", RequiresRestart = false, CanModifyCallback = () => !GameNetworkManager.Instance.gameHasStarted || !NetworkManager.Singleton.IsListening });
        //var lcdungeonPrev = new BoolCheckBoxConfigItem(config.dungeonPreview, new BoolCheckBoxOptions { Name = "Dungeon Preview", Section = "Entrance Improvements", RequiresRestart = false, CanModifyCallback = () => !config.SeamlessDungeonExists });
        //var lcdungeonPrevQlt = new IntSliderConfigItem(config.dungeonPreviewResolution, new IntSliderOptions { Name = "Dungeon Preview Quality", Section = "Entrance Improvements", Min = 16, Max = 1024, RequiresRestart = true, CanModifyCallback = () => config.dungeonPreview.Value });
        var lcdungeonSnd = new BoolCheckBoxConfigItem(config.dungeonSoundExchange, new BoolCheckBoxOptions { Name = "Dungeon Sound Exchange", Section = "Entrance Improvements", RequiresRestart = false });
        var lcungeonSndVol = new IntSliderConfigItem(config.dungeonSoundExchangeVolume, new IntSliderOptions { Name = "Dungeon Sound Exchange Volume", Section = "Entrance Improvements", Min = 0, Max = 100, RequiresRestart = false, CanModifyCallback = () => config.dungeonSoundExchange.Value });

        var lcAccessEnemiesDataFile = new GenericButtonConfigItem(
            "General",
            "Enemies Data",
            "All the enemies data are stored inside the file. You have a few rules to follow when editing the file or it will not work. Look at the github or thunderstore page for more info.",
            "Copy path",
            () => { GUIUtility.systemCopyBuffer = EnemiesDataManager.EnemiesDataFile; }
        );

        LethalConfigManager.AddConfigItem(lcSyncRanks);
        LethalConfigManager.AddConfigItem(lcAccessEnemiesDataFile);
        //LethalConfigManager.AddConfigItem(lcdungeonPrev);
        //LethalConfigManager.AddConfigItem(lcdungeonPrevQlt);
        LethalConfigManager.AddConfigItem(lcdungeonSnd);
        LethalConfigManager.AddConfigItem(lcungeonSndVol);
    }
}
