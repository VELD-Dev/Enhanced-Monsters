using System.Runtime.CompilerServices;
using LethalSettings.UI;

namespace EnhancedMonsters.Config;

internal static class LethalSettingsSupport
{
    private static bool? lethalSettingsLoaded;

    public static bool LethalSettingsLoaded
    {
        get
        {
            lethalSettingsLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.willis.lc.lethalsettings");

            return (bool)lethalSettingsLoaded;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    internal static void RegisterLethalSettings(LocalConfig config)
    {
        Plugin.logger.LogInfo("Registering lsSyncRank");
        var lsSyncRanks = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = true,
            Text = "Synchronize Ranks",
            Value = true,
            OnValueChanged = (comp, value) => { config.synchronizeRanks.Value = value; },
            OnInitialize = (comp) => { comp.Value = config.synchronizeRanks.Value; comp.Enabled = !GameNetworkManager.Instance.gameHasStarted || !NetworkManager.Singleton.IsListening; }
        };
        Plugin.logger.LogInfo("Registering lsDungeonPrevQltComp");
        /*
        var lsDungeonPrevQltComp = new LethalSettings.UI.Components.SliderComponent
        {
            Enabled = !config.SeamlessDungeonExists,
            Text = "Dungeon Preview Quality",
            Value = 256,
            MinValue = 16,
            MaxValue = 1024,
            ShowValue = true,
            WholeNumbers = true,
            OnValueChanged = (comp, value) => { config.dungeonPreviewResolution.Value = (int)value; },
            OnInitialize = (comp) => { comp.Value = config.dungeonPreviewResolution.Value; }
        };
        */
        Plugin.logger.LogInfo("Registering lsDungeonPrevComponent");
        /*var lsDungeonPrevComponent = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = config.SeamlessDungeonExists,
            Text = "Dungeon Preview",
            Value = !config.SeamlessDungeonExists,
            OnValueChanged = (comp, value) => { config.dungeonPreview.Value = value; lsDungeonPrevQltComp.Enabled = value; },
            OnInitialize = (comp) => { comp.Value = config.dungeonPreview.Value; lsDungeonPrevQltComp.Enabled = config.dungeonPreview.Value; }
        };
        */
        Plugin.logger.LogInfo("Registering lsDungeonSndVolComponent");
        var lsDungeonSndVolComponent = new LethalSettings.UI.Components.SliderComponent
        {
            Enabled = false,
            Text = "[UNAVAILABLE] Dungeon Sound Exchange Volume",
            Value = 100,
            MinValue = 0,
            MaxValue = 100,
            ShowValue = true,
            WholeNumbers = true,
            OnValueChanged = (comp, value) => { config.dungeonSoundExchangeVolume.Value = (int)value; },
            OnInitialize = (comp) => { comp.Value = config.dungeonSoundExchangeVolume.Value; }
        };
        Plugin.logger.LogInfo("Registering lsDungeonSndComponent");
        var lsDungeonSndComponent = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = false,
            Text = "[UNAVAILABLE] Dungeon Sound Exchange",
            Value = true,
            OnValueChanged = (comp, value) => { config.dungeonSoundExchange.Value = value; lsDungeonSndVolComponent.Enabled = value; },
            OnInitialize = (comp) => { comp.Value = config.dungeonSoundExchange.Value; lsDungeonSndVolComponent.Enabled = config.dungeonSoundExchange.Value; }
        };
        Plugin.logger.LogInfo("Registering lsAccessEnemiesDataFile");
        var lsAccessEnemiesDataFile = new LethalSettings.UI.Components.ButtonComponent
        {
            Enabled = true,
            ShowCaret = true,
            Text = "Copy path to Enemies Data file",
            OnClick = (comp) => { GUIUtility.systemCopyBuffer = EnemiesDataManager.EnemiesDataFile; }
        };

        Plugin.logger.LogInfo("Registering mod info");
        ModMenu.RegisterMod(new()
        {
            Id = PluginInfo.GUID,
            Name = PluginInfo.DisplayName,
            Version = PluginInfo.Version,
            Description = LocalConfig.ModDesc,
            MenuComponents = [
                lsSyncRanks,
                lsAccessEnemiesDataFile,
                //lsDungeonSndComponent,
                //lsDungeonSndVolComponent
            ],
        });
        Plugin.logger.LogInfo("Registered Mod Info");
    }
}
