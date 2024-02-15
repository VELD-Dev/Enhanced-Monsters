using ES3Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters.Utils.Utils;

public static class EnemiesDataManager
{
    public static Dictionary<string, EnemyValue> EnemiesData = [];
    public static string EnemiesDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EnemiesData.json");
    public static readonly Dictionary<string, string> CreaturesRank = new()
    {
        ["Hoarding bug"]    = "E",
        ["Spore Lizard"]    = "E",
        ["Baboon hawk"]     = "E",
        ["Hydrogere"]       = "D",
        ["Centipede"]       = "D",
        ["MouthDog"]        = "C",
        ["Bunker Spider"]   = "C",
        ["Crawler"]         = "B",
        ["Flowerman"]       = "B",
        ["SandWorm"]        = "B",
        ["CoilHead"]        = "A",
        ["Masked"]          = "A",
        ["Nutcracker"]      = "A",
        ["ForestKeeper"]    = "A",
        ["RedBees"]         = "A",
        ["Jester"]          = "S+",
        ["DressGirl"]       = "?",
    };

    public static void LoadEnemiesData()
    {
        if (!File.Exists(EnemiesDataFile))
        {
            SaveEnemiesData();
            return;
        }

        var filetext = File.ReadAllText(EnemiesDataFile);
        var parsed = JsonConvert.DeserializeObject<Dictionary<string, EnemyValue>>(filetext);
        if (parsed is null)
        {
            EnemiesData = [];
            return;
        }
        EnemiesData = parsed;
    }

    public static void RegisterEnemy(string enemyName, EnemyValue enemyData)
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
