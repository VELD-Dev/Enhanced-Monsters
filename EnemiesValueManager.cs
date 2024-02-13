using ES3Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters;

public static class EnemiesValueManager
{
    public static Dictionary<string, EnemyValue> EnemiesData = [];
    public static string EnemiesDataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EnemiesData.json");
    
    public static void LoadEnemiesData()
    {
        if(!File.Exists(EnemiesDataFile))
        {
            var output = JsonConvert.SerializeObject(EnemiesData);
            File.WriteAllText(EnemiesDataFile, output);
            return;
        }

        var filetext = File.ReadAllText(EnemiesDataFile);
        var parsed = JsonConvert.DeserializeObject<Dictionary<string, EnemyValue>>(filetext);
        if(parsed is null)
        {
            EnemiesData = [];
            return;
        }
        EnemiesData = parsed;
    }

    public static void RegisterEnemy(string enemyName, EnemyValue enemyData)
    {
        if(EnemiesData.TryAdd(enemyName, enemyData))
        {
            Plugin.logger.LogDebug("EnemyData '{0}' already exists!", enemyName);
        }
        if (!File.Exists(EnemiesDataFile))
        {
            var output = JsonConvert.SerializeObject(EnemiesData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(output, EnemiesDataFile);
        }
    }
}
