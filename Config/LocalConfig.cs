namespace EnhancedMonsters.Config;

public class LocalConfig
{
    public static LocalConfig Singleton { get; private set; }

    private readonly bool SeamlessDungeonExists;

    public readonly ConfigEntry<bool> dungeonPreview;
    public readonly ConfigEntry<int> dungeonPreviewResolution;
    public readonly ConfigEntry<bool> dungeonSoundExchange;
    public readonly ConfigEntry<int> dungeonSoundExchangeVolume;

    public LocalConfig(ConfigFile cfg)
    {
        Singleton = this;

        SeamlessDungeonExists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("tsundrella.seamlessdungeon");

        dungeonPreview = cfg.Bind(
            "Dungeon Preview",
            "EM_DungeonPreviewBoolText",
            SeamlessDungeonExists ? false : true,
            "Wether or not to enable interior/exterior preview through Dungeon's doors windows. [NOTE: This may not work on some modded moons and interiors.]"
        );
        dungeonPreviewResolution = cfg.Bind(
            "Dungeon Preview Resolution",
            "EM_DungeonPreviewResIntText",
            512,
            "The dungeon preview uses cameras to render the exterior/interior. Reducing this will reduce preview quality but increase performance. [NOTE: Must be a multiple of 2.]"
        );
        dungeonSoundExchange = cfg.Bind(
            "Dungeon Sound Exchange",
            "EM_DungeonSoundExchangeBoolText",
            true,
            "Wether to enable or not sound exchange between interior and exterior of the dungeon, next to entrance and fire exits. Disabling it can enhance performance on weakest configurations."
        );
        dungeonSoundExchangeVolume = cfg.Bind(
            "Dungeon Sound Exchange Volume",
            "EM_DungeonSoundExchangeVolumeIntText",
            70,
            "Volume level of sounds emitted from the interior/exterior when player is on the other side of the door."
        );

        // LETHAL CONFIG

        LethalConfigManager.SetModDescription("Enhanced Monsters aims at enhancing experience towards monsters, but also has various QOL improvements.");

        var lcdungeonPrev = new BoolCheckBoxConfigItem(dungeonPreview, new BoolCheckBoxOptions { Section = "Entrance Improvements", RequiresRestart = false, CanModifyCallback = () => !SeamlessDungeonExists });
        var lcdungeonPrevQlt = new IntSliderConfigItem(dungeonPreviewResolution, new IntSliderOptions { Section = "Entrance Improvements", Min = 16, Max = 1024, RequiresRestart = true, CanModifyCallback = () => dungeonPreview.Value });
        var lcdungeonSnd = new BoolCheckBoxConfigItem(dungeonSoundExchange, new BoolCheckBoxOptions { Section = "Entrance Improvements", RequiresRestart = false });
        var lcungeonSndVol = new IntSliderConfigItem(dungeonSoundExchangeVolume, new IntSliderOptions { Section = "Entrance Improvements", Min = 0, Max = 100, RequiresRestart = false, CanModifyCallback = () => dungeonSoundExchange.Value });

        LethalConfigManager.AddConfigItem(lcdungeonPrev);
        LethalConfigManager.AddConfigItem(lcdungeonPrevQlt);
        LethalConfigManager.AddConfigItem(lcdungeonSnd);
        LethalConfigManager.AddConfigItem(lcungeonSndVol);
    }
}
