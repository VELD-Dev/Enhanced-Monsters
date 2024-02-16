using ES3Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Utils.Utils;

public static class EnemiesDataManager
{
    public static Dictionary<string, EnemyData> EnemiesData = [];
    public static Dictionary<string, EnemyData> DefaultEnemiesData = new()
    {
        // Lootable
        ["Baboon hawk"]     = new EnemyData(true, 50, 80, 61, "E"),
        ["Hoarding bug"]    = new EnemyData(true, 80, 110, 38, "E"),
        ["Centipede"]       = new EnemyData(true, 90, 120, 23, "D"),
        ["Bunker Spider"]   = new EnemyData(true, 140, 180, 75, "C"),
        ["MouthDog"]        = new EnemyData(true, 170, 210, 88, "C"),
        ["Crawler"]         = new EnemyData(true, 210, 270, 66, "B"),
        ["Flowerman"]       = new EnemyData(true, 250, 290, 55, "B"),
        ["Nutcracker"]      = new EnemyData(true, 300, 340, 44, "A"),

        // Invincible
        ["Spore Lizard"]    = new EnemyData(false, 0, 0, 0, "E"),
        ["Hydrogere"]       = new EnemyData(false, 0, 0, 0, "D"),
        ["RedBees"]         = new EnemyData(false, 0, 0, 0, "C"),
        ["SandWorm"]        = new EnemyData(false, 0, 0, 0, "B"),
        ["CoilHead"]        = new EnemyData(false, 0, 0, 0, "A"),
        ["ForestKeeper"]    = new EnemyData(false, 0, 0, 0, "S"),
        ["Jester"]          = new EnemyData(false, 0, 0, 0, "S+"),
        ["DressGirl"]       = new EnemyData(false, 0, 0, 0, "?")
    };
    public static string EnemiesDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EnemiesData.json");

    public static void LoadEnemiesData()
    {
        if (!File.Exists(EnemiesDataFile))
        {
            EnemiesData = DefaultEnemiesData;
            SaveEnemiesData();
            return;
        }

        var filetext = File.ReadAllText(EnemiesDataFile);
        var parsed = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>(filetext);
        if (parsed is null)
        {
            EnemiesData = DefaultEnemiesData;
            SaveEnemiesData();
            return;
        }
        EnemiesData = parsed;
        EnemiesData.Union(DefaultEnemiesData);
        SaveEnemiesData();
    }

    public static void RegisterEnemy(string enemyName, EnemyData enemyData)
    {
        if (!EnemiesData.TryAdd(enemyName, enemyData))
        {
            Plugin.logger.LogDebug($"EnemyData '{enemyName}' already exists!");
            return;
        }

        SaveEnemiesData();
    }

    public static void SaveEnemiesData()
    {
        var output = JsonConvert.SerializeObject(EnemiesData, Newtonsoft.Json.Formatting.Indented);
        Plugin.logger.LogDebug(output);
        Plugin.logger.LogDebug(EnemiesDataFile);
        File.WriteAllText(EnemiesDataFile, output);
        Plugin.logger.LogInfo("Saved enemies data.");
    }
}
