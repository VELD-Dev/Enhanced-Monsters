namespace EnhancedMonsters.Config;

public class LocalConfig
{
    public static LocalConfig Singleton { get; private set; }

    internal const string ModDesc = "Enhanced Monsters aims at enhancing experience towards monsters, but also has various QOL improvements.";

    internal readonly bool SeamlessDungeonExists;

    public readonly ConfigEntry<bool> synchronizeRanks;
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
        dungeonSoundExchange = cfg.Bind(
            "Entrance Improvements",
            "[NOT AVAILABLE YET] Dungeon Sound Exchange",
            true,
            "[NOT AVAILABLE YET] Wether to enable or not sound exchange between interior and exterior of the dungeon, next to entrance and fire exits. Disabling it can enhance performance on weakest configurations."
        );
        dungeonSoundExchangeVolume = cfg.Bind(
            "Entrance Improvements",
            "[NOT AVAILABLE YET] Dungeon Sound Exchange Volume",
            70,
            "[NOT AVAILABLE YET] Volume level of sounds emitted from the interior/exterior when player is on the other side of the door."
        );

        if (LethalConfigSupport.LethalConfigLoaded)
        {
            Plugin.logger.LogInfo("Loading LethalConfig settings");
            LethalConfigSupport.RegisterLethalConfig(this);
        }
        if (LethalSettingsSupport.LethalSettingsLoaded)
        {
            Plugin.logger.LogInfo("Loading LethalSettings settings");
            LethalSettingsSupport.RegisterLethalSettings(this);
        }
        Plugin.logger.LogInfo("Registering all settings.");
    }
}
