using ES3Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Utils;

public static class EnemiesDataManager
{
    public static Dictionary<string, EnemyData> EnemiesData = [];
    public static Dictionary<string, EnemyData> DefaultEnemiesData = new()
    {
        // Lootable
        ["Baboon hawk"]     = new EnemyData(true, 40, 70, 61, "E"),
        ["Hoarding bug"]    = new EnemyData(true, 30, 60, 38, "E"),
        ["Centipede"]       = new EnemyData(true, 55, 95, 23, "D"),
        ["Bunker Spider"]   = new EnemyData(true, 140, 180, 75, "C"),
        ["MouthDog"]        = new EnemyData(true, 170, 210, 88, "C"),
        ["Crawler"]         = new EnemyData(true, 210, 270, 66, "B"),
        ["Nutcracker"]      = new EnemyData(true, 300, 340, 44, "A"),

        // Invincible
        ["Manticoil"]       = new EnemyData(false, 0, 0, 0, "F"),
        ["PinkGiant"]       = new EnemyData(false, 0, 0, 0, "F"),
        ["Puffer"]          = new EnemyData(false, 0, 0, 0, "E"),
        ["Blob"]            = new EnemyData(false, 0, 0, 0, "D"),
        ["Red Locust Bees"] = new EnemyData(false, 0, 0, 0, "C"),
        ["SandWorm"]        = new EnemyData(false, 0, 0, 0, "B"),
        ["Flowerman"]       = new EnemyData(false, 0, 0, 0, "B"),
        ["Locker"]          = new EnemyData(false, 0, 0, 0, "B"),
        ["CoilHead"]        = new EnemyData(false, 0, 0, 0, "A"),
        ["Football"]        = new EnemyData(false, 0, 0, 0, "A"),
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
        EnemiesData = EnemiesData.Union(DefaultEnemiesData).ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
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
