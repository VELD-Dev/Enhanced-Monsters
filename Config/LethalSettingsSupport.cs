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
            ],
        });
        Plugin.logger.LogInfo("Registered Mod Info");
    }
}
