namespace EnhancedMonsters.Config;

public class LocalConfig
{
    public static LocalConfig Singleton { get; private set; }

    internal const string ModDesc = "Enhanced Monsters aims at enhancing experience towards monsters, but also has various QOL improvements.";

    public readonly ConfigEntry<bool> synchronizeRanks;

    public LocalConfig(ConfigFile cfg)
    {
        Singleton = this;

        synchronizeRanks = cfg.Bind(
            "General",
            "Synchronize Ranks",
            true,
            "Wether or not to synchronize ranks of enemies from host's configs. It is recommended to keep it enabled. [NOTE: This setting can only be edited from the main menu.]"
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
