using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalSettings;
using LethalSettings.UI;
using System.Runtime.CompilerServices;

namespace EnhancedMonsters.Config;

public class LocalConfig
{
    public static LocalConfig Singleton { get; private set; }

    private static bool? lethalConfigLoaded;
    private static bool? lethalSettingsLoaded;
    public static bool LethalConfigLoaded
    {
        get
        {
            lethalConfigLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ainavt.lc.lethalconfig");

            return (bool)lethalConfigLoaded;
        }
    }
    public static bool LethalSettingsLoaded
    {
        get
        {
            lethalSettingsLoaded ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.willis.lc.lethalsettings");

            return (bool)lethalSettingsLoaded;
        }
    }

    private const string ModDesc = "Enhanced Monsters aims at enhancing experience towards monsters, but also has various QOL improvements.";

    private readonly bool SeamlessDungeonExists;

    public readonly ConfigEntry<bool> synchronizeRanks;
    public readonly ConfigEntry<bool> dungeonPreview;
    public readonly ConfigEntry<int> dungeonPreviewResolution;
    public readonly ConfigEntry<bool> dungeonSoundExchange;
    public readonly ConfigEntry<int> dungeonSoundExchangeVolume;

    public LocalConfig(ConfigFile cfg)
    {
        Singleton = this;

        SeamlessDungeonExists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("tsundrella.seamlessdungeon");

        synchronizeRanks = cfg.Bind(
            "General",
            "Synchronize Ranks",
            true,
            "Wether or not to synchronize ranks of enemies from host's configs. It is recommended to keep it enabled. [NOTE: This setting can only be edited from the main menu.]"
        );
        dungeonPreview = cfg.Bind(
            "Entrance Improvements",
            "Dungeon Preview",
            SeamlessDungeonExists ? false : true,
            "Wether or not to enable interior/exterior preview through Dungeon's doors windows. [NOTE: This may not work on some modded moons and interiors.]"
        );
        dungeonPreviewResolution = cfg.Bind(
            "Entrance Improvements",
            "Dungeon Preview Resolution",
            512,
            "The dungeon preview uses cameras to render the exterior/interior. Reducing this will reduce preview quality but increase performance. [NOTE: Must be a multiple of 2.]"
        );
        dungeonSoundExchange = cfg.Bind(
            "Entrance Improvements",
            "Dungeon Sound Exchange",
            true,
            "Wether to enable or not sound exchange between interior and exterior of the dungeon, next to entrance and fire exits. Disabling it can enhance performance on weakest configurations."
        );
        dungeonSoundExchangeVolume = cfg.Bind(
            "Entrance Improvements",
            "Dungeon Sound Exchange Volume",
            70,
            "Volume level of sounds emitted from the interior/exterior when player is on the other side of the door."
        );

        if(LethalConfigLoaded) RegisterLethalConfig();
        if(LethalSettingsLoaded) RegisterLethalSettings();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void RegisterLethalConfig()
    {
        LethalConfigManager.SetModDescription(ModDesc);

        var lcSyncRanks = new BoolCheckBoxConfigItem(synchronizeRanks, new BoolCheckBoxOptions { Name = "Synchronize Ranks", Section = "General", RequiresRestart = false, CanModifyCallback = () => !GameNetworkManager.Instance.gameHasStarted || !NetworkManager.Singleton.IsListening });
        var lcdungeonPrev = new BoolCheckBoxConfigItem(dungeonPreview, new BoolCheckBoxOptions { Name = "Dungeon Preview", Section = "Entrance Improvements", RequiresRestart = false, CanModifyCallback = () => !SeamlessDungeonExists });
        var lcdungeonPrevQlt = new IntSliderConfigItem(dungeonPreviewResolution, new IntSliderOptions { Name = "Dungeon Preview Quality", Section = "Entrance Improvements", Min = 16, Max = 1024, RequiresRestart = true, CanModifyCallback = () => dungeonPreview.Value });
        var lcdungeonSnd = new BoolCheckBoxConfigItem(dungeonSoundExchange, new BoolCheckBoxOptions { Name = "Dungeon Sound Exchange", Section = "Entrance Improvements", RequiresRestart = false });
        var lcungeonSndVol = new IntSliderConfigItem(dungeonSoundExchangeVolume, new IntSliderOptions { Name = "Dungeon Sound Exchange Volume", Section = "Entrance Improvements", Min = 0, Max = 100, RequiresRestart = false, CanModifyCallback = () => dungeonSoundExchange.Value });

        var lcAccessEnemiesDataFile = new GenericButtonConfigItem(
            "General",
            "Enemies Data",
            "All the enemies data are stored inside the file. You have a few rules to follow when editing the file or it will not work. Look at the github or thunderstore page for more info.",
            "Edit",
            () => { Process.Start(new ProcessStartInfo("explorer", "\"" + EnemiesDataManager.EnemiesDataFile + "\"")); }
        );

        LethalConfigManager.AddConfigItem(lcSyncRanks);
        LethalConfigManager.AddConfigItem(lcAccessEnemiesDataFile);
        LethalConfigManager.AddConfigItem(lcdungeonPrev);
        LethalConfigManager.AddConfigItem(lcdungeonPrevQlt);
        LethalConfigManager.AddConfigItem(lcdungeonSnd);
        LethalConfigManager.AddConfigItem(lcungeonSndVol);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void RegisterLethalSettings()
    {
        var lsSyncRanks = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = true,
            Text = "Synchronize Ranks",
            Value = true,
            OnValueChanged = (comp, value) => { synchronizeRanks.Value = value; },
            OnInitialize = (comp) => { comp.Value = synchronizeRanks.Value; comp.Enabled = !GameNetworkManager.Instance.gameHasStarted || !NetworkManager.Singleton.IsListening; }
        };
        var lsDungeonPrevQltComp = new LethalSettings.UI.Components.SliderComponent
        {
            Enabled = SeamlessDungeonExists,
            Text = "Dungeon Preview Quality",
            Value = 256,
            MinValue = 16,
            MaxValue = 1024,
            ShowValue = true,
            WholeNumbers = true,
            OnValueChanged = (comp, value) => { dungeonPreviewResolution.Value = (int)value; },
            OnInitialize = (comp) => { comp.Value = dungeonPreviewResolution.Value; }
        };
        var lsDungeonPrevComponent = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = SeamlessDungeonExists,
            Text = "Dungeon Preview",
            Value = !SeamlessDungeonExists,
            OnValueChanged = (comp, value) => { dungeonPreview.Value = value; lsDungeonPrevQltComp.Enabled = value; },
            OnInitialize = (comp) => { comp.Value = dungeonPreview.Value; lsDungeonPrevQltComp.Enabled = dungeonPreview.Value; }
        };
        var lsDungeonSndVolComponent = new LethalSettings.UI.Components.SliderComponent
        {
            Enabled = true,
            Text = "Dungeon Sound Exchange Volume",
            Value = 100,
            MinValue = 0,
            MaxValue = 100,
            ShowValue = true,
            WholeNumbers = true,
            OnValueChanged = (comp, value) => { dungeonSoundExchangeVolume.Value = (int)value; },
            OnInitialize = (comp) => { comp.Value = dungeonSoundExchangeVolume.Value; }
        };
        var lsDungeonSndComponent = new LethalSettings.UI.Components.ToggleComponent
        {
            Enabled = true,
            Text = "Dungeon Sound Exchange",
            Value = true,
            OnValueChanged = (comp, value) => { dungeonSoundExchange.Value = value; lsDungeonSndVolComponent.Enabled = value; },
            OnInitialize = (comp) => { comp.Value = dungeonSoundExchange.Value; lsDungeonSndVolComponent.Enabled = dungeonSoundExchange.Value; }
        };
        var lsAccessEnemiesDataFile = new LethalSettings.UI.Components.ButtonComponent
        {
            Enabled = true,
            ShowCaret = true,
            Text = "Open Enemies Data file",
            OnClick = (comp) => { Process.Start(new ProcessStartInfo("explorer", "\"" + EnemiesDataManager.EnemiesDataFile + "\"")); }
        };

        ModMenu.RegisterMod(new()
        {
            Id = PluginInfo.GUID,
            Name = PluginInfo.DisplayName,
            Version = PluginInfo.Version,
            Description = ModDesc,
            MenuComponents = [
                lsSyncRanks,
                lsAccessEnemiesDataFile,
                lsDungeonPrevComponent,
                lsDungeonPrevQltComp,
                lsDungeonSndComponent,
                lsDungeonSndVolComponent
            ],
        });
    }
}
